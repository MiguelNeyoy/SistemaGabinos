// IPdfRenderService.cs
// Capa de Generación de Documentos (Clean Architecture).
// Transforma datos transaccionales (TicketData) en un documento binario (byte[]) en memoria RAM.
namespace SistemaGabinos.Infrastructure.Hardware;

public interface IPdfRenderService
{
    byte[] RenderizarReciboPdf(TicketData data);
}
