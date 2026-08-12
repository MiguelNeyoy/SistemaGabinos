// TicketPrinter.cs
// Orquestador del servicio de impresión desacoplado (Clean Architecture / SRP).
// Renderiza el documento binario con IPdfRenderService y lo envía al hardware con IPrinterService.
// Retorna PrintResult para que el caller pueda reaccionar a fallos de impresora.
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

    public SistemaGabinos.Infrastructure.Hardware.PrintResult ImprimirRecibo(TicketData ticket)
    {
        byte[] pdfBytes = _pdfRenderService.RenderizarReciboPdf(ticket);
        return _printerService.ImprimirBytes(pdfBytes);
    }
}
