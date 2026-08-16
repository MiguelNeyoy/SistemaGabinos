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
    private readonly IPrecioConfiguracionRepository _precioConfigRepo;
    private readonly ICursoRepository _cursoRepo;
    private readonly RegistrarAlumnoValidator _validator;

    public RegistrarAlumnoUseCase(
        IAlumnoRepository alumnoRepo,
        IInscripcionRepository inscripcionRepo,
        IDeudaRepository deudaRepo,
        IPrecioConfiguracionRepository precioConfigRepo,
        ICursoRepository cursoRepo,
        RegistrarAlumnoValidator validator)
    {
        _alumnoRepo = alumnoRepo;
        _inscripcionRepo = inscripcionRepo;
        _deudaRepo = deudaRepo;
        _precioConfigRepo = precioConfigRepo;
        _cursoRepo = cursoRepo;
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
            request.TelefonoTutor,
            request.CostoMensualidadPactada,
            request.DescuentoBecaPactada);

        alumno.ValidarReglasDeNegocio();
        _alumnoRepo.Guardar(alumno);

        var inscripcion = new Inscripcion(alumno.Id, request.CursoId, request.Horario);
        _inscripcionRepo.Guardar(inscripcion);

        var configuracion = _precioConfigRepo.Obtener();
        var curso = _cursoRepo.ObtenerPorId(request.CursoId);

        var deudaInscripcion = new Deuda(alumno.Id, ConceptoDeuda.Inscripcion, configuracion.CostoInscripcion);
        var deudaLibro = new Deuda(alumno.Id, ConceptoDeuda.Libro, curso?.PrecioLibro ?? configuracion.CostoLibro);
        var deudaMensualidad = new Deuda(alumno.Id, ConceptoDeuda.Mensualidad, alumno.MensualidadNeta);

        _deudaRepo.Guardar(deudaInscripcion);
        _deudaRepo.Guardar(deudaLibro);
        _deudaRepo.Guardar(deudaMensualidad);

        return new RegistrarAlumnoResponse(alumno.Id, "Alumno inscrito correctamente y deudas iniciales generadas.");
    }
}
