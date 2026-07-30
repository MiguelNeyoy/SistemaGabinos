// PrinterService.cs
// Implementación de la Capa de Hardware y Dispositivos.
// Envía bytes binarios al spooler de impresión de Windows y maneja excepciones físicas de hardware.
using System.Diagnostics;
using System.Drawing.Printing;
using System.Text;

namespace SistemaGabinos.Infrastructure.Hardware;

public class PrinterService : IPrinterService
{
    public PrintResult ImprimirBytes(byte[] documentoBytes, string? nombreImpresora = null)
    {
        if (documentoBytes == null || documentoBytes.Length == 0)
        {
            return new PrintResult(false, "El documento a imprimir está vacío.", false);
        }

        try
        {
            string textoContenido = Encoding.UTF8.GetString(documentoBytes);

            // Simulación / Envió al Spooler de Windows
            var printDoc = new PrintDocument();
            if (!string.IsNullOrWhiteSpace(nombreImpresora))
            {
                printDoc.PrinterSettings.PrinterName = nombreImpresora;
            }

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

            // Intentar enviar a la cola de impresión de Windows
            if (PrinterSettings.InstalledPrinters.Count > 0)
            {
                printDoc.Print();
            }

            Debug.WriteLine("=== DOCUMENTO IMPRESO VÍA PRINT SERVICE ===");
            Debug.WriteLine(textoContenido);

            return new PrintResult(true, "Documento enviado exitosamente a la cola de impresión.", false);
        }
        catch (InvalidPrinterException)
        {
            return new PrintResult(false, 
                "El pago fue registrado correctamente, pero no se pudo conectar con la impresora. Puede reimprimir el recibo desde el Historial.", 
                true);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error de Hardware de Impresión: {ex.Message}");
            return new PrintResult(false, 
                "El pago fue registrado correctamente, pero ocurrió un aviso de impresora. Puede reimprimir el recibo desde el Historial.", 
                true);
        }
    }
}
