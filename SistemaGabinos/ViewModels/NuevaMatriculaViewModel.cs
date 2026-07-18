using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Application.Interfaces;
using SistemaGabinos.Domain.Entities;
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
    private Curso? _cursoSeleccionado;

    [ObservableProperty]
    private decimal _montoInicial;

    [ObservableProperty]
    private string _mensaje = string.Empty;

    [ObservableProperty]
    private System.Windows.Visibility _visibleTutor = System.Windows.Visibility.Collapsed;

    public ObservableCollection<Curso> Cursos { get; } = new();

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

    [RelayCommand]
    private void Registrar()
    {
        if (CursoSeleccionado is null)
        {
            Mensaje = "Seleccione un curso.";
            return;
        }

        try
        {
            var request = new RegistrarAlumnoRequest(
                NombreCompleto, Curp, FechaNacimiento, Telefono,
                NombreTutor, ParentescoTutor, TelefonoTutor,
                CursoSeleccionado.Id, MontoInicial);

            var response = _useCase.Ejecutar(request);
            Mensaje = response.Mensaje;
        }
        catch (Exception ex)
        {
            Mensaje = ex.Message;
        }
    }
}
