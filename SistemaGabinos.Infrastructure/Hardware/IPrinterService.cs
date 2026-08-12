// IPrinterService.cs
// Capa de Hardware y Dispositivos (Clean Architecture).
// Envia bytes binarios de documentos al spooler de impresión de Windows.
namespace SistemaGabinos.Infrastructure.Hardware;

public interface IPrinterService
{
    PrintResult ImprimirBytes(byte[] documentoBytes, string? nombreImpresora = null);
}
