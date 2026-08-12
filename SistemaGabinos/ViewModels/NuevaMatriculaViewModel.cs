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
    private readonly IObtenerPreciosConfiguracionUseCase _obtenerPreciosUseCase;

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
    private Horario _horarioSeleccionado = Horario.Mañana;

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

    public ObservableCollection<Curso> Cursos { get; } = new();
    public ObservableCollection<ConceptoCobroItem> ConceptosCobro { get; } = new();
    public Horario[] Horarios => Enum.GetValues<Horario>();

    public NuevaMatriculaViewModel(
        IRegistrarAlumnoUseCase useCase,
        ICursoRepository cursoRepo,
        IObtenerPreciosConfiguracionUseCase obtenerPreciosUseCase)
    {
        _useCase = useCase;
        _cursoRepo = cursoRepo;
        _obtenerPreciosUseCase = obtenerPreciosUseCase;
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
            return;
        }

        CobroVisible = System.Windows.Visibility.Visible;
        CargarConceptosCobro(value);
    }

    private void CargarConceptosCobro(Curso curso)
    {
        ConceptosCobro.Clear();
        var config = _obtenerPreciosUseCase.Ejecutar();

        ConceptosCobro.Add(new ConceptoCobroItem
        {
            Descripcion = "Inscripción Estándar",
            Monto = config?.CostoInscripcion ?? 500m,
            Seleccionado = true
        });

        ConceptosCobro.Add(new ConceptoCobroItem
        {
            Descripcion = $"Libro {curso.Nombre}",
            Monto = curso.PrecioLibro > 0 ? curso.PrecioLibro : (config?.CostoLibro ?? 350m),
            Seleccionado = true
        });

        ConceptosCobro.Add(new ConceptoCobroItem
        {
            Descripcion = "Primera Mensualidad Base",
            Monto = config?.CostoMensualidad ?? 1400m,
            Seleccionado = true
        });

        RecalcularTotal();
    }

    private void RecalcularTotal()
    {
        Total = 0;
        foreach (var item in ConceptosCobro)
            Total += item.Monto;
    }

    public event Action<int>? NavegarACobroSolicitado;

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
            var request = new RegistrarAlumnoRequest(
                NombreCompleto, Curp, FechaNacimiento, Telefono,
                NombreTutor, ParentescoTutor, TelefonoTutor,
                CursoSeleccionado.Id, HorarioSeleccionado);

            var response = _useCase.Ejecutar(request);
            AlumnoId = response.AlumnoId;
            Mensaje = response.Mensaje;
            MensajeExitoVisible = System.Windows.Visibility.Visible;
            MensajeErrorVisible = System.Windows.Visibility.Collapsed;

            NavegarACobroSolicitado?.Invoke(response.AlumnoId);
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
}
