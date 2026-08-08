using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Application.Interfaces;

namespace SistemaGabinos.ViewModels;

public partial class EditarAlumnoViewModel : ObservableObject
{
    private readonly IActualizarAlumnoUseCase _actualizarUseCase;

    [ObservableProperty]
    private int _id;

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
    private string _tituloSeccionTutor = "Contacto de Emergencia / Tutor (Opcional)";

    [ObservableProperty]
    private bool _esMenorDeEdad;

    [ObservableProperty]
    private string? _mensajeError;

    public event Action? GuardadoExitoso;

    public EditarAlumnoViewModel(IActualizarAlumnoUseCase actualizarUseCase)
    {
        _actualizarUseCase = actualizarUseCase;
    }

    public void CargarAlumno(ExpedienteAlumnoDto alumno)
    {
        Id = alumno.Id;
        NombreCompleto = alumno.NombreCompleto;
        Curp = alumno.Curp;
        FechaNacimiento = alumno.FechaNacimiento;
        Telefono = alumno.Telefono;
        NombreTutor = alumno.NombreTutor;
        ParentescoTutor = alumno.ParentescoTutor;
        TelefonoTutor = alumno.TelefonoTutor;

        EvaluarEdad(alumno.FechaNacimiento);
    }

    partial void OnFechaNacimientoChanged(DateTime value)
    {
        EvaluarEdad(value);
    }

    private void EvaluarEdad(DateTime fecha)
    {
        var edad = DateTime.UtcNow.Year - fecha.Year;
        if (fecha.Date > DateTime.UtcNow.AddYears(-edad)) edad--;

        EsMenorDeEdad = edad < 18;

        TituloSeccionTutor = EsMenorDeEdad
            ? "Datos del Tutor (Obligatorio por ser menor de edad)"
            : "Contacto de Emergencia / Tutor (Opcional)";
    }

    [RelayCommand]
    private void Guardar()
    {
        MensajeError = null;

        try
        {
            var request = new ActualizarAlumnoRequest(
                Id,
                NombreCompleto,
                FechaNacimiento,
                Telefono,
                NombreTutor,
                ParentescoTutor,
                TelefonoTutor);

            _actualizarUseCase.Ejecutar(request);
            GuardadoExitoso?.Invoke();
        }
        catch (FluentValidation.ValidationException vex)
        {
            MensajeError = string.Join(Environment.NewLine, vex.Errors.Select(e => e.ErrorMessage));
        }
        catch (Exception ex)
        {
            MensajeError = ex.Message;
        }
    }
}
