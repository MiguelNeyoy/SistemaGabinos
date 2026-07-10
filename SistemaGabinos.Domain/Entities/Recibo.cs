// Recibo.cs
// Comprobante generado a partir de un Pago.
// Contiene los datos del recibo (folio, monto, detalle).
// La impresión del recibo (PDF, impresora) es responsabilidad de Infrastructure.
namespace SistemaGabinos.Domain.Entities;

public class Recibo
{
    public int Id { get; private set; }
    public int PagoId { get; private set; }
    public decimal Monto { get; private set; }
    public DateTime FechaEmision { get; private set; }
    public string Folio { get; private set; }
    public string Detalle { get; private set; }

    private Recibo() { }

    public Recibo(int pagoId, decimal monto, string folio, string detalle)
    {
        if (pagoId <= 0)
            throw new ArgumentException("El ID del pago debe ser mayor que cero.", nameof(pagoId));

        if (monto <= 0)
            throw new ArgumentException("El monto del recibo debe ser mayor que cero.", nameof(monto));

        if (string.IsNullOrWhiteSpace(folio))
            throw new ArgumentException("El folio no puede estar vacío.", nameof(folio));

        if (string.IsNullOrWhiteSpace(detalle))
            throw new ArgumentException("El detalle no puede estar vacío.", nameof(detalle));

        PagoId = pagoId;
        Monto = monto;
        FechaEmision = DateTime.UtcNow;
        Folio = folio;
        Detalle = detalle;
    }
}
