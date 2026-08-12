// TipoFolioExtensions.cs
// Formateador desacoplado y fuertemente tipado para prefijos y folios.
namespace SistemaGabinos.Domain.Enums;

public static class TipoFolioExtensions
{
    public static string ObtenerPrefijo(this TipoFolio tipo) => tipo switch
    {
        TipoFolio.Pago => "PAGO-",
        TipoFolio.Deuda => "DEUDA-",
        TipoFolio.Recibo => "RECIBO-",
        _ => "DOC-"
    };

    public static string Formatear(this TipoFolio tipo, int id, DateTime? fecha = null)
    {
        var prefijo = tipo.ObtenerPrefijo();
        if (tipo == TipoFolio.Recibo && fecha.HasValue)
        {
            return $"{prefijo}{fecha.Value:yyyyMMdd}-{id:D4}";
        }
        return $"{prefijo}{id:D4}";
    }
}
