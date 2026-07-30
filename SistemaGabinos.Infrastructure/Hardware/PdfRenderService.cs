// PdfRenderService.cs
// Implementación de la Capa de Generación de Documentos (On-Demand en RAM).
// Diseña la plantilla reutilizable de 3 bloques (Membrete, Cuerpo Transaccional y Pie de Página).
using System.Text;

namespace SistemaGabinos.Infrastructure.Hardware;

public class PdfRenderService : IPdfRenderService
{
    public byte[] RenderizarReciboPdf(TicketData data)
    {
        var sb = new StringBuilder();

        // ----------------------------------------------------
        // BLOQUE 1: ENCABEZADO REUTILIZABLE (Membrete Academia)
        // ----------------------------------------------------
        sb.AppendLine("****************************************************************");
        sb.AppendLine("                       GABINOS ACADEMY                          ");
        sb.AppendLine("                  COMPROBANTE OFICIAL DE PAGO                   ");
        sb.AppendLine("****************************************************************");
        sb.AppendLine("Dirección: Av. Principal #123, Col. Centro");
        sb.AppendLine("Teléfono:  (664) 123-4567 | Email: contacto@gabinosacademy.com");
        sb.AppendLine("----------------------------------------------------------------");

        // ----------------------------------------------------
        // BLOQUE 2: CUERPO DEL RECIBO (Datos Transaccionales)
        // ----------------------------------------------------
        string folio = !string.IsNullOrWhiteSpace(data.Folio) ? data.Folio : "REC-OFFLINE";
        string alumno = !string.IsNullOrWhiteSpace(data.NombreAlumno) ? data.NombreAlumno : "Público General";
        string curp = !string.IsNullOrWhiteSpace(data.Curp) ? data.Curp : "N/A";

        sb.AppendLine($"Folio:    {folio,-20} Fecha: {data.Fecha:dd/MM/yyyy HH:mm}");
        sb.AppendLine($"Cliente:  {alumno}");
        sb.AppendLine($"CURP/ID:  {curp}");
        sb.AppendLine("----------------------------------------------------------------");
        sb.AppendLine(string.Format("{0,-30} | {1,10} | {2,14}", "CONCEPTO", "METODO", "SUBTOTAL"));
        sb.AppendLine("----------------------------------------------------------------");

        foreach (var item in data.Items)
        {
            sb.AppendLine(string.Format("{0,-30} | {1,10} | ${2,13:N2}", 
                TruncarOFormatear(item.Concepto, 30), 
                data.MetodoPago, 
                item.Monto));
        }

        sb.AppendLine("----------------------------------------------------------------");
        sb.AppendLine($"TOTAL A PAGAR:                              ${data.TotalACobrar,14:N2}");
        sb.AppendLine($"MONTO RECIBIDO ({data.MetodoPago}):                      ${data.MontoRecibido,14:N2}");
        sb.AppendLine($"CAMBIO ENTREGADO:                           ${data.Cambio,14:N2}");
        sb.AppendLine("----------------------------------------------------------------");

        // ----------------------------------------------------
        // BLOQUE 3: PIE DE PÁGINA (Legales y Firmas)
        // ----------------------------------------------------
        sb.AppendLine("Este documento es un comprobante de pago oficial emitido por la ");
        sb.AppendLine("administración de Gabinos Academy.");
        sb.AppendLine();
        sb.AppendLine();
        sb.AppendLine("__________________________          __________________________");
        sb.AppendLine("    Firma de Conformidad                     Firma de Caja    ");
        sb.AppendLine("****************************************************************");

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static string TruncarOFormatear(string texto, int maxLongitud)
    {
        if (string.IsNullOrEmpty(texto)) return string.Empty;
        return texto.Length <= maxLongitud ? texto : texto.Substring(0, maxLongitud - 3) + "...";
    }
}
