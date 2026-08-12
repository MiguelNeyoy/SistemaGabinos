// MetodoPago.cs
// Define los métodos de pago aceptados.
// - Efectivo: pago en efectivo.
// - Transferencia: pago por transferencia bancaria.
// - Tarjeta: pago con tarjeta de crédito/débito.
namespace SistemaGabinos.Domain.Enums;

public enum MetodoPago
{
    Efectivo,
    Transferencia,
    Tarjeta
}
