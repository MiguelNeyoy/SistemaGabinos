// ITicketPrinter.cs
// Interfaz para el servicio de impresión de tickets en impresora térmica.
using SistemaGabinos.Domain.Enums;

namespace SistemaGabinos.Infrastructure.Hardware;

public record TicketItemData(
    string Concepto,
    decimal Monto
);

public record TicketData(
    string NombreAlumno,
    string Curp,
    List<TicketItemData> Items,
    decimal TotalACobrar,
    decimal MontoRecibido,
    decimal Cambio,
    MetodoPago MetodoPago,
    DateTime Fecha,
    string? Folio = null
);

public interface ITicketPrinter
{
    SistemaGabinos.Infrastructure.Hardware.PrintResult ImprimirRecibo(TicketData ticket);
}
