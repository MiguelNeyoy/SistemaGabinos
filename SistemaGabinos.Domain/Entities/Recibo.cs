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
        PagoId = pagoId;
        Monto = monto;
        FechaEmision = DateTime.UtcNow;
        Folio = folio;
        Detalle = detalle;
    }
}
