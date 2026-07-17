using FluentValidation;
using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Application.Interfaces;
using SistemaGabinos.Application.Validators;
using SistemaGabinos.Domain.Entities;
using SistemaGabinos.Domain.Enums;
using SistemaGabinos.Domain.Exceptions;
using SistemaGabinos.Domain.Interfaces;

namespace SistemaGabinos.Application.UseCases;

public class RegistrarAlumnoUseCase : IRegistrarAlumnoUseCase
{
    private readonly IAlumnoRepository _alumnoRepo;
    private readonly IInscripcionRepository _inscripcionRepo;
    private readonly IDeudaRepository _deudaRepo;
    private readonly RegistrarAlumnoValidator _validator;

    public RegistrarAlumnoUseCase(
        IAlumnoRepository alumnoRepo,
        IInscripcionRepository inscripcionRepo,
        IDeudaRepository deudaRepo,
        RegistrarAlumnoValidator validator)
    {
        _alumnoRepo = alumnoRepo;
        _inscripcionRepo = inscripcionRepo;
        _deudaRepo = deudaRepo;
        _validator = validator;
    }

    public RegistrarAlumnoResponse Ejecutar(RegistrarAlumnoRequest request)
    {
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        if (_alumnoRepo.ObtenerPorCURP(request.Curp) is not null)
            throw new AlumnoYaRegistradoException(request.Curp);

        var alumno = new Alumno(
            request.NombreCompleto,
            request.Curp,
            request.FechaNacimiento,
            request.Telefono,
            request.NombreTutor,
            request.ParentescoTutor,
            request.TelefonoTutor);

        alumno.ValidarReglasDeNegocio();

        _alumnoRepo.Guardar(alumno);

        var inscripcion = new Inscripcion(alumno.Id, request.CursoId);
        _inscripcionRepo.Guardar(inscripcion);

        var deuda = new Deuda(alumno.Id, ConceptoDeuda.Inscripcion, request.MontoInicial);
        _deudaRepo.Guardar(deuda);

        return new RegistrarAlumnoResponse(alumno.Id, "Alumno registrado exitosamente.");
    }
}
