using FluentValidation;
using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Application.Interfaces;
using SistemaGabinos.Application.Validators;
using SistemaGabinos.Domain.Enums;
using SistemaGabinos.Domain.Exceptions;
using SistemaGabinos.Domain.Interfaces;

namespace SistemaGabinos.Application.UseCases;

public class ActualizarAlumnoUseCase : IActualizarAlumnoUseCase
{
    private readonly IAlumnoRepository _alumnoRepo;
    private readonly ActualizarAlumnoValidator _validator;

    public ActualizarAlumnoUseCase(
        IAlumnoRepository alumnoRepo,
        ActualizarAlumnoValidator validator)
    {
        _alumnoRepo = alumnoRepo;
        _validator = validator;
    }

    public void Ejecutar(ActualizarAlumnoRequest request)
    {
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var alumno = _alumnoRepo.ObtenerPorId(request.Id)
            ?? throw new KeyNotFoundException("Alumno no encontrado.");

        if (alumno.Estado == EstadoAlumno.Inactivo)
            throw new AlumnoInactivoException(alumno.NombreCompleto);

        alumno.ActualizarDatos(
            request.NombreCompleto,
            request.FechaNacimiento,
            request.Telefono,
            request.NombreTutor,
            request.ParentescoTutor,
            request.TelefonoTutor);

        _alumnoRepo.Guardar(alumno);
    }
}
