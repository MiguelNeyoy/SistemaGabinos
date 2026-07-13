// Deuda.cs
// Representa una cuenta por cobrar (deuda) asociada a un alumno.
// Una deuda puede pagarse en múltiples parcialidades (varios Pago registros).
// Concepto indica si es por Inscripcion, Mensualidad o Libro.
using SistemaGabinos.Domain.Enums;

namespace SistemaGabinos.Domain.Entities;

public class Deuda
{
    public int Id { get; private set; }
    public int AlumnoId { get; private set; }
    public ConceptoDeuda Concepto { get; private set; }
    public decimal MontoTotal { get; private set; }
    public decimal MontoPagado { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public bool EstaPagada => MontoPagado >= MontoTotal;

    private Deuda() { }

    public Deuda(int alumnoId, ConceptoDeuda concepto, decimal montoTotal)
    {
        if (alumnoId <= 0)
            throw new ArgumentException("El ID del alumno debe ser mayor que cero.", nameof(alumnoId));

        if (montoTotal <= 0)
            throw new ArgumentException("El monto total de la deuda debe ser mayor que cero.", nameof(montoTotal));

        if (!Enum.IsDefined(typeof(ConceptoDeuda), concepto))
            throw new ArgumentException("El concepto de la deuda no es válido.", nameof(concepto));

        AlumnoId = alumnoId;
        Concepto = concepto;
        MontoTotal = montoTotal;
        FechaCreacion = DateTime.UtcNow;
    }

    public void RegistrarAbono(decimal monto)
    {
        if (monto <= 0)
            throw new ArgumentException("El abono debe ser mayor que cero.");

        if (EstaPagada)
            throw new ArgumentException("La deuda ya está completamente pagada.");

        if (MontoPagado + monto > MontoTotal)
            throw new ArgumentException("El abono excede el saldo pendiente.");

        MontoPagado += monto;
    }
}
