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
    public DateTime FechaCreacion { get; private set; }
    public bool EstaPagada { get; private set; }

    private Deuda() { }

    public Deuda(int alumnoId, ConceptoDeuda concepto, decimal montoTotal)
    {
        AlumnoId = alumnoId;
        Concepto = concepto;
        MontoTotal = montoTotal;
        FechaCreacion = DateTime.UtcNow;
        EstaPagada = false;
    }

    public void MarcarComoPagada()
    {
        EstaPagada = true;
    }
}
