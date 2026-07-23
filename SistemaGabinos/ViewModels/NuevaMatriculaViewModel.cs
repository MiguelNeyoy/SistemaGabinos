using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Application.Interfaces;
using SistemaGabinos.Models;
using SistemaGabinos.Domain.Entities;
using SistemaGabinos.Domain.Enums;
using SistemaGabinos.Domain.Interfaces;

namespace SistemaGabinos.ViewModels;

public partial class NuevaMatriculaViewModel : ObservableObject
{
    private readonly IRegistrarAlumnoUseCase _useCase;
    private readonly ICursoRepository _cursoRepo;

    [ObservableProperty]
    private string _nombreCompleto = string.Empty;

    [ObservableProperty]
    private string _curp = string.Empty;

    [ObservableProperty]
    private DateTime _fechaNacimiento = DateTime.Today.AddYears(-18);

    [ObservableProperty]
    private string _telefono = string.Empty;

    [ObservableProperty]
    private string? _nombreTutor;

    [ObservableProperty]
    private string? _parentescoTutor;

    [ObservableProperty]
    private string? _telefonoTutor;

    [ObservableProperty]
    private int? _alumnoId;

    [ObservableProperty]
    private Curso? _cursoSeleccionado;

    [ObservableProperty]
    private string _mensaje = string.Empty;

    [ObservableProperty]
    private string _mensajeError = string.Empty;

    [ObservableProperty]
    private System.Windows.Visibility _visibleTutor = System.Windows.Visibility.Collapsed;

    [ObservableProperty]
    private System.Windows.Visibility _cobroVisible = System.Windows.Visibility.Collapsed;

    [ObservableProperty]
    private System.Windows.Visibility _ticketVisible = System.Windows.Visibility.Collapsed;

    [ObservableProperty]
    private System.Windows.Visibility _mensajeExitoVisible = System.Windows.Visibility.Collapsed;

    [ObservableProperty]
    private System.Windows.Visibility _mensajeErrorVisible = System.Windows.Visibility.Collapsed;

    [ObservableProperty]
    private decimal _total;

    [ObservableProperty]
    private bool _efectivoSeleccionado = true;

    [ObservableProperty]
    private bool _transferenciaSeleccionado;

    public ObservableCollection<Curso> Cursos { get; } = new();
    public ObservableCollection<ConceptoCobroItem> ConceptosCobro { get; } = new();

    public NuevaMatriculaViewModel(IRegistrarAlumnoUseCase useCase, ICursoRepository cursoRepo)
    {
        _useCase = useCase;
        _cursoRepo = cursoRepo;
        CargarCursos();
    }

    private void CargarCursos()
    {
        Cursos.Clear();
        foreach (var curso in _cursoRepo.ObtenerTodos())
            Cursos.Add(curso);
    }

    partial void OnFechaNacimientoChanged(DateTime value)
    {
        var edad = DateTime.UtcNow.Year - value.Year;
        if (value.Date > DateTime.UtcNow.AddYears(-edad)) edad--;
        VisibleTutor = edad < 18 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
    }

    partial void OnCursoSeleccionadoChanged(Curso? value)
    {
        if (value is null)
        {
            CobroVisible = System.Windows.Visibility.Collapsed;
            TicketVisible = System.Windows.Visibility.Collapsed;
            return;
        }

        CobroVisible = System.Windows.Visibility.Visible;
        TicketVisible = System.Windows.Visibility.Visible;
        CargarConceptosCobro(value);
    }

    private void CargarConceptosCobro(Curso curso)
    {
        ConceptosCobro.Clear();

        ConceptosCobro.Add(new ConceptoCobroItem
        {
            Descripcion = "Inscripción",
            Monto = 0,
            Seleccionado = true
        });

        ConceptosCobro.Add(new ConceptoCobroItem
        {
            Descripcion = $"Libro {curso.Nombre}",
            Monto = curso.PrecioLibro,
            Seleccionado = true
        });

        ConceptosCobro.Add(new ConceptoCobroItem
        {
            Descripcion = "Mensualidad",
            Monto = 0,
            Seleccionado = true
        });

        foreach (var item in ConceptosCobro)
            item.PropertyChanged += (_, _) => RecalcularTotal();

        RecalcularTotal();
    }

    private void RecalcularTotal()
    {
        Total = 0;
        foreach (var item in ConceptosCobro)
            if (item.Seleccionado)
                Total += item.Monto;
    }

    [RelayCommand]
    private void Registrar()
    {
        if (CursoSeleccionado is null)
        {
            MensajeError = "Seleccione un curso.";
            MensajeErrorVisible = System.Windows.Visibility.Visible;
            MensajeExitoVisible = System.Windows.Visibility.Collapsed;
            return;
        }

        try
        {
            var metodoPago = EfectivoSeleccionado ? MetodoPago.Efectivo : MetodoPago.Transferencia;

            var request = new RegistrarAlumnoRequest(
                NombreCompleto, Curp, FechaNacimiento, Telefono,
                NombreTutor, ParentescoTutor, TelefonoTutor,
                CursoSeleccionado.Id, Total, metodoPago);

            var response = _useCase.Ejecutar(request);
            AlumnoId = response.AlumnoId;
            Mensaje = response.Mensaje;
            MensajeExitoVisible = System.Windows.Visibility.Visible;
            MensajeErrorVisible = System.Windows.Visibility.Collapsed;
        }
        catch (FluentValidation.ValidationException vex)
        {
            MensajeError = string.Join(Environment.NewLine, vex.Errors.Select(e => e.ErrorMessage));
            MensajeErrorVisible = System.Windows.Visibility.Visible;
            MensajeExitoVisible = System.Windows.Visibility.Collapsed;
        }
        catch (Exception ex)
        {
            MensajeError = ex.Message;
            MensajeErrorVisible = System.Windows.Visibility.Visible;
            MensajeExitoVisible = System.Windows.Visibility.Collapsed;
        }
    }

    [RelayCommand]
    private void GenerarTicket()
    {
    }
}
