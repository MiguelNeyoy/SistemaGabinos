// CobroExpresViewModel.cs
// ViewModel especializado para la Ventanilla de Cobro Exprés con control financiero realista.
// Soporta 3 botones: Cancelar (sin BD), Imprimir Recibo (100% manual sin BD), y Pagado (guardado en SQLite y retorno automático al Expediente).
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Application.Interfaces;
using SistemaGabinos.Domain.Enums;
using SistemaGabinos.Domain.Entities;
using SistemaGabinos.Domain.Interfaces;
using SistemaGabinos.Infrastructure.Hardware;
using SistemaGabinos.Models;

namespace SistemaGabinos.ViewModels;

public partial class CobroExpresViewModel : ObservableObject
{
    private readonly IAlumnoRepository _alumnoRepo;
    private readonly IDeudaRepository _deudaRepo;
    private readonly IRegistrarPagoUseCase _registrarPagoUseCase;
    private readonly ITicketPrinter _ticketPrinter;
    private readonly IPdfRenderService _pdfRenderService;
    private readonly IBuscarAlumnosSugerenciasUseCase _buscarAlumnosUseCase;
    private readonly ICursoRepository _cursoRepo;
    private readonly IObtenerPreciosConfiguracionUseCase _obtenerPreciosUseCase;

    // --- Propiedades para el Buscador ---
    [ObservableProperty]
    private string _criterioBusqueda = string.Empty;

    [ObservableProperty]
    private bool _dropdownAbierto;

    public ObservableCollection<AlumnoSugerenciaDto> ResultadosBusqueda { get; } = new();

    [ObservableProperty]
    private AlumnoSugerenciaDto? _alumnoSeleccionado;

    [ObservableProperty]
    private bool _tieneAlumnoCargado;

    // --- Propiedades del Alumno y Cobro ---
    [ObservableProperty]
    private int _alumnoId;

    [ObservableProperty]
    private string _nombreCompleto = string.Empty;

    [ObservableProperty]
    private string _curp = string.Empty;

    [ObservableProperty]
    private bool _tieneBeca;

    [ObservableProperty]
    private MetodoPago _metodoPagoSeleccionado = MetodoPago.Efectivo;

    [ObservableProperty]
    private decimal _montoRecibido;

    [ObservableProperty]
    private decimal _totalACobrar;

    [ObservableProperty]
    private decimal _cambioAEntregar;

    [ObservableProperty]
    private bool _esAbonoParcial;

    [ObservableProperty]
    private string _mensaje = string.Empty;

    [ObservableProperty]
    private string _mensajeError = string.Empty;

    [ObservableProperty]
    private bool _esProcesando;

    public ObservableCollection<ConceptoCobroItem> ConceptosPendientes { get; } = new();
    public MetodoPago[] MetodosPago => Enum.GetValues<MetodoPago>();
    public ObservableCollection<Curso> CursosDisponibles { get; } = new();

    public event Action? CobroCompletado;
    public event Action? CancelarSolicitado;

    public CobroExpresViewModel(
        IAlumnoRepository alumnoRepo,
        IDeudaRepository deudaRepo,
        IRegistrarPagoUseCase registrarPagoUseCase,
        ITicketPrinter ticketPrinter,
        IPdfRenderService pdfRenderService,
        IBuscarAlumnosSugerenciasUseCase buscarAlumnosUseCase,
        ICursoRepository cursoRepo,
        IObtenerPreciosConfiguracionUseCase obtenerPreciosUseCase)
    {
        _alumnoRepo = alumnoRepo;
        _deudaRepo = deudaRepo;
        _registrarPagoUseCase = registrarPagoUseCase;
        _ticketPrinter = ticketPrinter;
        _pdfRenderService = pdfRenderService;
        _buscarAlumnosUseCase = buscarAlumnosUseCase;
        _cursoRepo = cursoRepo;
        _obtenerPreciosUseCase = obtenerPreciosUseCase;
    }

    public void PrecargarDatos(int alumnoId, int? deudaIdInicial = null)
    {
        Mensaje = string.Empty;
        MensajeError = string.Empty;
        EsProcesando = false;
        AlumnoId = alumnoId;

        var alumno = _alumnoRepo.ObtenerPorId(alumnoId);
        if (alumno is null)
        {
            MensajeError = "No se encontró el alumno especificado.";
            TieneAlumnoCargado = false;
            return;
        }

        TieneAlumnoCargado = true;
        NombreCompleto = alumno.NombreCompleto;
        Curp = alumno.CURP;
        TieneBeca = alumno.TieneBeca;

        ConceptosPendientes.Clear();
        var deudas = _deudaRepo.ObtenerPorAlumno(alumnoId)
            .Where(d => !d.EstaPagada)
            .ToList();

        foreach (var deuda in deudas)
        {
            decimal saldoPendiente = deuda.MontoTotal - deuda.MontoPagado;
            bool esSeleccionado = deudaIdInicial.HasValue 
                ? deuda.Id == deudaIdInicial.Value 
                : true;

            var item = new ConceptoCobroItem
            {
                DeudaId = deuda.Id,
                Descripcion = $"{deuda.Concepto} (Saldo: ${saldoPendiente:N2})",
                Monto = saldoPendiente,
                Seleccionado = esSeleccionado
            };

            item.PropertyChanged += (_, _) => RecalcularTotales();
            ConceptosPendientes.Add(item);
        }

        CargarCursos();
        RecalcularTotales();
        MontoRecibido = TotalACobrar;
    }

    private void LimpiarDatosAlumno()
    {
        TieneAlumnoCargado = false;
        AlumnoId = 0;
        NombreCompleto = string.Empty;
        Curp = string.Empty;
        TieneBeca = false;
        ConceptosPendientes.Clear();
        TotalACobrar = 0;
        CambioAEntregar = 0;
        MontoRecibido = 0;
        EsAbonoParcial = false;
    }

    private void CargarCursos()
    {
        if (CursosDisponibles.Count == 0)
        {
            var config = _obtenerPreciosUseCase.Ejecutar();
            decimal precioLibro = config?.CostoLibro ?? 350m;

            for (int i = 1; i <= 6; i++)
            {
                CursosDisponibles.Add(new Curso($"Book {i}", precioLibro));
            }
        }
    }

    partial void OnCriterioBusquedaChanged(string value)
    {
        ResultadosBusqueda.Clear();
        if (string.IsNullOrWhiteSpace(value) || value.Trim().Length < 2)
        {
            DropdownAbierto = false;
            return;
        }

        var resultados = _buscarAlumnosUseCase.Ejecutar(value.Trim(), 10);
        foreach (var r in resultados)
        {
            ResultadosBusqueda.Add(r);
        }

        DropdownAbierto = ResultadosBusqueda.Count > 0;
    }

    partial void OnAlumnoSeleccionadoChanged(AlumnoSugerenciaDto? value)
    {
        if (value is not null)
        {
            PrecargarDatos(value.Id);
        }
        else
        {
            LimpiarDatosAlumno();
        }
    }

    // --- Agregar Libro Dinámicamente ---
    [RelayCommand]
    private void AgregarLibro(Curso curso)
    {
        if (curso == null || AlumnoId == 0) return;

        // Crear una nueva deuda por el libro de inmediato en la base de datos
        var nuevaDeuda = new Deuda(AlumnoId, ConceptoDeuda.Libro, curso.PrecioLibro);
        _deudaRepo.Guardar(nuevaDeuda);
        
        // Cargar el concepto en la UI para cobrarlo
        var item = new ConceptoCobroItem
        {
            DeudaId = nuevaDeuda.Id,
            Descripcion = $"{nuevaDeuda.Concepto} {curso.Nombre} (Saldo: ${curso.PrecioLibro:N2})",
            Monto = curso.PrecioLibro,
            Seleccionado = true // Lo marcamos automáticamente porque lo acaba de agregar para pagarlo
        };

        item.PropertyChanged += (_, _) => RecalcularTotales();
        ConceptosPendientes.Add(item);
        
        RecalcularTotales();
        MontoRecibido = TotalACobrar;
        Mensaje = $"Se ha agregado {curso.Nombre} a los conceptos pendientes.";
    }

    partial void OnMetodoPagoSeleccionadoChanged(MetodoPago value)
    {
        RecalcularTotales();
    }

    partial void OnMontoRecibidoChanged(decimal value)
    {
        RecalcularTotales();
    }

    private void RecalcularTotales()
    {
        TotalACobrar = 0;
        foreach (var item in ConceptosPendientes)
        {
            if (item.Seleccionado)
            {
                TotalACobrar += item.Monto;
            }
        }

        if (MetodoPagoSeleccionado == MetodoPago.Efectivo)
        {
            CambioAEntregar = Math.Max(0, MontoRecibido - TotalACobrar);
        }
        else
        {
            CambioAEntregar = 0;
        }

        EsAbonoParcial = MontoRecibido < TotalACobrar && TotalACobrar > 0;
    }

    [RelayCommand]
    private void Cancelar()
    {
        // Limpiamos todo al cancelar
        CriterioBusqueda = string.Empty;
        LimpiarDatosAlumno();
        CancelarSolicitado?.Invoke();
    }

    [RelayCommand]
    private void ImprimirRecibo()
    {
        if (AlumnoId == 0)
        {
            MensajeError = "Debe buscar o cargar un alumno primero.";
            return;
        }

        var itemsSeleccionados = ConceptosPendientes
            .Where(c => c.Seleccionado)
            .Select(c => new TicketItemData(c.Descripcion, c.Monto))
            .ToList();

        if (itemsSeleccionados.Count == 0)
        {
            MensajeError = "Debe seleccionar al menos un concepto para imprimir el recibo.";
            return;
        }

        var ticketData = new TicketData(
            NombreCompleto,
            Curp,
            itemsSeleccionados,
            TotalACobrar,
            MontoRecibido,
            CambioAEntregar,
            MetodoPagoSeleccionado,
            DateTime.Now
        );

        var result = _ticketPrinter.ImprimirRecibo(ticketData);

        if (result.Exito)
        {
            Mensaje = "Recibo enviado a la impresora.";
            MensajeError = string.Empty;
        }
        else
        {
            MensajeError = result.Mensaje;
            OfrecerGuardarPdf(ticketData);
        }
    }

    private void OfrecerGuardarPdf(TicketData ticketData)
    {
        byte[] pdfBytes = _pdfRenderService.RenderizarReciboPdf(ticketData);
        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            FileName = $"Recibo_{NombreCompleto.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmm}.pdf",
            DefaultExt = ".pdf",
            Filter = "Archivos PDF (.pdf)|*.pdf"
        };

        if (dialog.ShowDialog() == true)
        {
            System.IO.File.WriteAllBytes(dialog.FileName, pdfBytes);
            Mensaje = $"Recibo guardado como PDF en: {System.IO.Path.GetFileName(dialog.FileName)}";
        }
    }

    [RelayCommand]
    private void Pagado()
    {
        if (AlumnoId == 0)
        {
            MensajeError = "Debe buscar o cargar un alumno primero.";
            return;
        }

        var deudasSeleccionadasIds = ConceptosPendientes
            .Where(c => c.Seleccionado)
            .Select(c => c.DeudaId)
            .ToList();

        if (deudasSeleccionadasIds.Count == 0)
        {
            MensajeError = "Debe seleccionar al menos un concepto a pagar.";
            return;
        }

        if (MontoRecibido <= 0)
        {
            MensajeError = "Ingrese un monto recibido válido mayor a $0.00.";
            return;
        }

        try
        {
            EsProcesando = true;
            MensajeError = string.Empty;

            var request = new RegistrarPagoRequest(
                AlumnoId,
                deudasSeleccionadasIds,
                MontoRecibido,
                MetodoPagoSeleccionado);

            var response = _registrarPagoUseCase.Ejecutar(request);

            Mensaje = response.Mensaje;
            CobroCompletado?.Invoke();
        }
        catch (FluentValidation.ValidationException vex)
        {
            MensajeError = string.Join(Environment.NewLine, vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (Exception ex)
        {
            MensajeError = ex.Message;
        }
        finally
        {
            EsProcesando = false;
        }
    }
}
