// Inscripcion.cs
// Representa la inscripción de un alumno a un curso (libro/nivel) específico.
// Este flujo es solo para alumnos nuevos que entran por primera vez al sistema.
// Los alumnos que ya están registrados y avanzan de nivel no usan este flujo.
using SistemaGabinos.Domain.Enums;
using SistemaGabinos.Domain.Exceptions;

namespace SistemaGabinos.Domain.Entities;

public class Inscripcion
{
    public int Id { get; private set; }
    public int AlumnoId { get; private set; }
    public int CursoId { get; private set; }
    public DateTime FechaInscripcion { get; private set; }
    public EstadoInscripcion Estado { get; private set; }

    private Inscripcion() { }

    public Inscripcion(int alumnoId, int cursoId)
    {
        if (alumnoId <= 0) throw new ArgumentException("AlumnoId debe ser mayor que cero.", nameof(alumnoId));
        if (cursoId <= 0) throw new ArgumentException("CursoId debe ser mayor que cero.", nameof(cursoId));

        AlumnoId = alumnoId;
        CursoId = cursoId;
        FechaInscripcion = DateTime.UtcNow;
        Estado = EstadoInscripcion.Vigente;
    }

    public void Activar()
    {
        if (Estado == EstadoInscripcion.Vencida)
            throw new DomainException("No se puede activar una inscripción vencida.");
        Estado = EstadoInscripcion.Vigente;
    }

    public void Vencer()
    {
        Estado = EstadoInscripcion.Vencida;
    }

    public void Cancelar()
    {
        Estado = EstadoInscripcion.Cancelada;
    }
}
