// Pago.cs
// Representa un pago realizado por un alumno.
// Puede ser un pago completo o parcial vinculado a una Deuda (DeudaId).
// Concepto indica si es Mensualidad o Libro.
// MetodoPago indica cómo se pagó (Efectivo, Transferencia, Tarjeta).
using SistemaGabinos.Domain.Enums;

namespace SistemaGabinos.Domain.Entities;

public class Pago
{
    public int Id { get; private set; }
    public int AlumnoId { get; private set; }
    public int? DeudaId { get; private set; }
    public decimal Monto { get; private set; }
    public DateTime Fecha { get; private set; }
    public ConceptoPago Concepto { get; private set; }
    public MetodoPago MetodoPago { get; private set; }
    public bool EstaCancelado { get; private set; }

    private Pago() { }

    public Pago(int alumnoId, int? deudaId, decimal monto, ConceptoPago concepto, MetodoPago metodoPago)
    {
        AlumnoId = alumnoId;
        DeudaId = deudaId;
        Monto = monto;
        Fecha = DateTime.UtcNow;
        Concepto = concepto;
        MetodoPago = metodoPago;
        EstaCancelado = false;
    }

    public Recibo GenerarRecibo(string folio, string detalle)
    {
        return new Recibo(Id, Monto, folio, detalle);
    }
}
