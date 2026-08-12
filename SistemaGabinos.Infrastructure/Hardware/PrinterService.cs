// PrinterService.cs
// Implementación de la Capa de Hardware y Dispositivos.
// Envía bytes binarios al spooler de impresión de Windows y maneja excepciones físicas de hardware.
// Incluye filtro de impresoras virtuales de Windows (Microsoft Print to PDF, XPS, Fax, OneNote, etc.).
using System.Diagnostics;
using System.Drawing.Printing;
using System.Text;

namespace SistemaGabinos.Infrastructure.Hardware;

public class PrinterService : IPrinterService
{
    private static readonly string[] ImpresorasVirtualesOmisión = new[]
    {
        "microsoft print to pdf",
        "microsoft xps document writer",
        "fax",
        "onenote",
        "send to onenote",
        "root print queue"
    };

    public PrintResult ImprimirBytes(byte[] documentoBytes, string? nombreImpresora = null)
    {
        if (documentoBytes == null || documentoBytes.Length == 0)
        {
            return new PrintResult(false, "El documento a imprimir está vacío.", false);
        }

        try
        {
            var printDoc = new PrintDocument();
            
            // Si se especifica un nombre, usarlo; de lo contrario, usar la predeterminada de Windows.
            string printerTarget = !string.IsNullOrWhiteSpace(nombreImpresora) 
                ? nombreImpresora 
                : printDoc.PrinterSettings.PrinterName;

            // Validar que exista la impresora destino
            if (string.IsNullOrWhiteSpace(printerTarget) || !printDoc.PrinterSettings.IsValid)
            {
                return new PrintResult(false, 
                    "No se encontró una impresora física válida o predeterminada en el sistema.", 
                    true);
            }

            // Filtrar si la impresora seleccionada es virtual / genérica de Windows
            string printerLower = printerTarget.ToLowerInvariant().Trim();
            if (ImpresorasVirtualesOmisión.Any(v => printerLower.Contains(v)))
            {
                return new PrintResult(false, 
                    $"La impresora seleccionada ('{printerTarget}') es una impresora virtual de software y no un dispositivo físico de impresión.", 
                    true);
            }

            string textoContenido = Encoding.UTF8.GetString(documentoBytes);
            int lineIndex = 0;
            string[] lineas = textoContenido.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            printDoc.PrintPage += (sender, e) =>
            {
                var font = new System.Drawing.Font("Courier New", 10);
                float leftMargin = e.MarginBounds.Left;
                float topMargin = e.MarginBounds.Top;
                float yPos = topMargin;
                int linesPerPage = (int)(e.MarginBounds.Height / font.GetHeight(e.Graphics!));

                int count = 0;
                while (count < linesPerPage && lineIndex < lineas.Length)
                {
                    string line = lineas[lineIndex];
                    yPos = topMargin + (count * font.GetHeight(e.Graphics!));
                    e.Graphics!.DrawString(line, font, System.Drawing.Brushes.Black, leftMargin, yPos, new System.Drawing.StringFormat());
                    count++;
                    lineIndex++;
                }

                e.HasMorePages = (lineIndex < lineas.Length);
            };

            printDoc.PrinterSettings.PrinterName = printerTarget;
            printDoc.Print();

            Debug.WriteLine($"=== DOCUMENTO ENVIADO A IMPRESORA FÍSICA: {printerTarget} ===");

            return new PrintResult(true, $"Documento enviado exitosamente a la impresora '{printerTarget}'.", false);
        }
        catch (InvalidPrinterException)
        {
            return new PrintResult(false, 
                "No se pudo conectar con la impresora especificada. Verifique que esté encendida y conectada.", 
                true);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error de Hardware de Impresión: {ex.Message}");
            return new PrintResult(false, 
                $"Aviso de impresora: {ex.Message}", 
                true);
        }
    }
}
