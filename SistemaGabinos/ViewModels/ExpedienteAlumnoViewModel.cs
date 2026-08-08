using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Application.Interfaces;
using SistemaGabinos.Infrastructure.Hardware;

namespace SistemaGabinos.ViewModels;

public partial class ExpedienteAlumnoViewModel : ObservableObject
{
    private readonly IObtenerExpedienteAlumnoUseCase _expedienteUseCase;
    private readonly IPdfRenderService _pdfRenderService;
    private readonly ICambiarEstadoAlumnoUseCase _cambiarEstadoUseCase;
    private readonly IGestionarBecaUseCase _gestionarBecaUseCase;

    [ObservableProperty]
    private ExpedienteAlumnoDto? _alumno;

    [ObservableProperty]
    private decimal _totalPendiente;

    [ObservableProperty]
    private bool _esDatosPersonales = true;

    [ObservableProperty]
    private bool _esHistorialFinanciero = false;

    [ObservableProperty]
    private bool _esActivo;

    [ObservableProperty]
    private bool _tieneBeca;

    [ObservableProperty]
    private string _mensajeExpediente = string.Empty;

    public ObservableCollection<PagoItem> Pagos { get; } = new();

    public ExpedienteAlumnoViewModel(
        IObtenerExpedienteAlumnoUseCase expedienteUseCase,
        IPdfRenderService pdfRenderService,
        ICambiarEstadoAlumnoUseCase cambiarEstadoUseCase,
        IGestionarBecaUseCase gestionarBecaUseCase)
    {
        _expedienteUseCase = expedienteUseCase;
        _pdfRenderService = pdfRenderService;
        _cambiarEstadoUseCase = cambiarEstadoUseCase;
        _gestionarBecaUseCase = gestionarBecaUseCase;
    }

    public void CargarAlumno(int alumnoId)
    {
        MensajeExpediente = string.Empty;
        var expediente = _expedienteUseCase.Ejecutar(alumnoId);
        if (expediente is null)
            return;

        Alumno = expediente;
        TotalPendiente = expediente.TotalPendiente;
        EsActivo = expediente.Estado == "Activo";
        TieneBeca = expediente.TieneBeca;

        Pagos.Clear();
        foreach (var pago in expediente.Pagos)
        {
            Pagos.Add(pago);
        }
    }

    partial void OnEsDatosPersonalesChanged(bool value)
    {
        if (value && EsHistorialFinanciero)
        {
            EsHistorialFinanciero = false;
        }
    }

    partial void OnEsHistorialFinancieroChanged(bool value)
    {
        if (value && EsDatosPersonales)
        {
            EsDatosPersonales = false;
        }
    }

    public event Action<int, int?>? NavegarACobroSolicitado;

    [RelayCommand]
    private void Pagar(int? deudaId = null)
    {
        if (Alumno is null) return;
        NavegarACobroSolicitado?.Invoke(Alumno.Id, deudaId);
    }

    [RelayCommand]
    private void DarDeBaja()
    {
        if (Alumno is null) return;
        try
        {
            MensajeExpediente = _cambiarEstadoUseCase.DarDeBaja(Alumno.Id);
            CargarAlumno(Alumno.Id);
        }
        catch (Exception ex)
        {
            MensajeExpediente = ex.Message;
        }
    }

    [RelayCommand]
    private void Reactivar()
    {
        if (Alumno is null) return;
        try
        {
            MensajeExpediente = _cambiarEstadoUseCase.Reactivar(Alumno.Id);
            CargarAlumno(Alumno.Id);
        }
        catch (Exception ex)
        {
            MensajeExpediente = ex.Message;
        }
    }

    [RelayCommand]
    private void ToggleBeca()
    {
        if (Alumno is null) return;
        try
        {
            MensajeExpediente = Alumno.TieneBeca
                ? _gestionarBecaUseCase.QuitarBeca(Alumno.Id)
                : _gestionarBecaUseCase.AsignarBeca(Alumno.Id);
            CargarAlumno(Alumno.Id);
        }
        catch (Exception ex)
        {
            MensajeExpediente = ex.Message;
        }
    }

    [RelayCommand]
    private void Editar()
    {
        if (Alumno is null) return;

        var modal = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<Views.EditarAlumnoModal>(App.Services);
        modal.Owner = System.Windows.Application.Current.MainWindow;
        modal.ViewModel.CargarAlumno(Alumno);

        if (modal.ShowDialog() == true)
        {
            CargarAlumno(Alumno.Id);
        }
    }

    [RelayCommand]
    private void ExportarPdf()
    {
        if (Alumno is null) return;

        var itemsData = Pagos.Select(p => new TicketItemData(p.Concepto, p.Monto)).ToList();
        if (itemsData.Count == 0)
        {
            itemsData.Add(new TicketItemData("Estado de Cuenta - Sin Registros", 0));
        }

        var ticketData = new TicketData(
            Alumno.NombreCompleto,
            Alumno.Curp,
            itemsData,
            TotalPendiente,
            0,
            0,
            Domain.Enums.MetodoPago.Efectivo,
            DateTime.Now
        );

        byte[] pdfBytes = _pdfRenderService.RenderizarReciboPdf(ticketData);

        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            FileName = $"Expediente_{Alumno.NombreCompleto.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.pdf",
            DefaultExt = ".pdf",
            Filter = "Archivos PDF (.pdf)|*.pdf"
        };

        if (dialog.ShowDialog() == true)
        {
            System.IO.File.WriteAllBytes(dialog.FileName, pdfBytes);
        }
    }
}
