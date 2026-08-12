// Pago.cs
// Representa un pago realizado por un alumno.
// Puede ser un pago completo o parcial vinculado a una Deuda (DeudaId).
// Concepto indica si es Mensualidad o Libro.
// MetodoPago indica cómo se pagó (Efectivo, Transferencia, Tarjeta).
using SistemaGabinos.Domain.Enums;
using SistemaGabinos.Domain.Exceptions;

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
        if (alumnoId <= 0)
            throw new ArgumentException("El ID del alumno debe ser mayor que cero.", nameof(alumnoId));

        if (monto <= 0)
            throw new ArgumentException("El monto del pago debe ser mayor que cero.", nameof(monto));

        if (!Enum.IsDefined(typeof(ConceptoPago), concepto))
            throw new ArgumentException("El concepto del pago no es válido.", nameof(concepto));

        if (!Enum.IsDefined(typeof(MetodoPago), metodoPago))
            throw new ArgumentException("El método de pago no es válido.", nameof(metodoPago));

        AlumnoId = alumnoId;
        DeudaId = deudaId;
        Monto = monto;
        Fecha = DateTime.UtcNow;
        Concepto = concepto;
        MetodoPago = metodoPago;
        EstaCancelado = false;
    }

    public void Cancelar()
    {
        if (EstaCancelado)
            throw new DomainException("El pago ya está cancelado.");
        EstaCancelado = true;
    }

    public Recibo GenerarRecibo(int pagoId, string folio, string detalle)
    {
        return new Recibo(pagoId, Monto, folio, detalle);
    }
}
