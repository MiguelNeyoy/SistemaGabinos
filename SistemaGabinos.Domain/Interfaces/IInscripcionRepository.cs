// IInscripcionRepository.cs
// Contrato para la persistencia de inscripciones.
// ObtenerPorAlumno permite consultar el historial de inscripciones de un alumno.
using SistemaGabinos.Domain.Entities;

namespace SistemaGabinos.Domain.Interfaces;

public interface IInscripcionRepository
{
    Inscripcion? ObtenerPorId(int id);
    List<Inscripcion> ObtenerPorAlumno(int alumnoId);
    void Guardar(Inscripcion inscripcion);
}
