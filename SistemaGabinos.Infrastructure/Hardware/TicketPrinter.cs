// TicketPrinter.cs
// Orquestador del servicio de impresión desacoplado (Clean Architecture / SRP).
// Renderiza el documento binario con IPdfRenderService y lo envía al hardware con IPrinterService.
namespace SistemaGabinos.Infrastructure.Hardware;

public class TicketPrinter : ITicketPrinter
{
    private readonly IPdfRenderService _pdfRenderService;
    private readonly IPrinterService _printerService;

    public TicketPrinter(IPdfRenderService pdfRenderService, IPrinterService printerService)
    {
        _pdfRenderService = pdfRenderService;
        _printerService = printerService;
    }

    public void ImprimirRecibo(TicketData ticket)
    {
        // 1. Capa de Renderizado de Documentos (DTO -> byte[])
        byte[] pdfBytes = _pdfRenderService.RenderizarReciboPdf(ticket);

        // 2. Capa de Hardware (byte[] -> Spooler Windows con captura de PrinterException)
        var result = _printerService.ImprimirBytes(pdfBytes);

        if (!result.Exito)
        {
            System.Diagnostics.Debug.WriteLine($"Aviso de Impresión: {result.Mensaje}");
        }
    }
}
