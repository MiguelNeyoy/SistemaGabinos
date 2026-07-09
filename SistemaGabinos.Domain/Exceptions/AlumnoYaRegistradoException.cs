// AlumnoYaRegistradoException.cs
// Se lanza cuando se intenta registrar un alumno con una CURP que ya existe en el sistema.
namespace SistemaGabinos.Domain.Exceptions;

public class AlumnoYaRegistradoException(string curp)
    : DomainException($"Ya existe un alumno registrado con la CURP: {curp}")
{
}