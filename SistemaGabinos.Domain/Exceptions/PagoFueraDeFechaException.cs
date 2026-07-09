// PagoFueraDeFechaException.cs
// Se lanza cuando la fecha de un pago es inválida (ej. fecha futura no permitida).
namespace SistemaGabinos.Domain.Exceptions;

public class PagoFueraDeFechaException()
    : DomainException("La fecha del pago no es válida.")
{
}