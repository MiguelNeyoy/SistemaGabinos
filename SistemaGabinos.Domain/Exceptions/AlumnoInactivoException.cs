// AlumnoInactivoException.cs
// Se lanza cuando se intenta realizar una acción sobre un alumno en estado Inactivo.
namespace SistemaGabinos.Domain.Exceptions;

public class AlumnoInactivoException(string nombre)
    : DomainException($"El alumno {nombre} se encuentra inactivo y no puede realizar esta acción.")
{
}