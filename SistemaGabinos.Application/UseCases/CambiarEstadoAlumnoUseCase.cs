using SistemaGabinos.Application.Interfaces;
using SistemaGabinos.Domain.Enums;
using SistemaGabinos.Domain.Exceptions;
using SistemaGabinos.Domain.Interfaces;

namespace SistemaGabinos.Application.UseCases;

public class CambiarEstadoAlumnoUseCase : ICambiarEstadoAlumnoUseCase
{
    private readonly IAlumnoRepository _alumnoRepo;
    private readonly IInscripcionRepository _inscripcionRepo;

    public CambiarEstadoAlumnoUseCase(
        IAlumnoRepository alumnoRepo,
        IInscripcionRepository inscripcionRepo)
    {
        _alumnoRepo = alumnoRepo;
        _inscripcionRepo = inscripcionRepo;
    }

    public string DarDeBaja(int alumnoId)
    {
        var alumno = _alumnoRepo.ObtenerPorId(alumnoId)
            ?? throw new KeyNotFoundException("Alumno no encontrado.");

        if (alumno.Estado == EstadoAlumno.Inactivo)
            throw new DomainException("El alumno ya está dado de baja.");

        alumno.DarDeBaja();
        _alumnoRepo.Guardar(alumno);

        var inscripciones = _inscripcionRepo.ObtenerPorAlumno(alumnoId);
        var vigente = inscripciones.FirstOrDefault(i => i.Estado == EstadoInscripcion.Vigente);
        if (vigente is not null)
        {
            vigente.Cancelar();
            _inscripcionRepo.Guardar(vigente);
        }

        return $"El alumno '{alumno.NombreCompleto}' ha sido dado de baja.";
    }

    public string Reactivar(int alumnoId)
    {
        var alumno = _alumnoRepo.ObtenerPorId(alumnoId)
            ?? throw new KeyNotFoundException("Alumno no encontrado.");

        if (alumno.Estado == EstadoAlumno.Activo)
            throw new DomainException("El alumno ya está activo.");

        alumno.Reactivar();
        _alumnoRepo.Guardar(alumno);

        return $"El alumno '{alumno.NombreCompleto}' ha sido reactivado.";
    }
}
