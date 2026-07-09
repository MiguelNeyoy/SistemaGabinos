// CURPInvalidoException.cs
// Se lanza cuando la CURP proporcionada no tiene el formato válido de 18 caracteres alfanuméricos.
namespace SistemaGabinos.Domain.Exceptions;

public class CURPInvalidoException(string curp)
    : DomainException($"La CURP '{curp}' no tiene un formato válido.")
{
}
