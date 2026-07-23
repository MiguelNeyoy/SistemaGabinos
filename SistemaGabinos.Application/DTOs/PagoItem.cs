namespace SistemaGabinos.Application.DTOs;

public class PagoItem
{
    public string Folio { get; set; } = string.Empty;
    public string Concepto { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
}
