// IExcelExportService.cs
// Interfaz para el servicio de exportación de reportes financieros a CSV.
using SistemaGabinos.Application.DTOs;

namespace SistemaGabinos.Infrastructure.Hardware;

public interface IExcelExportService
{
    byte[] GenerarCsvCorteCaja(CorteCajaDto corteCaja);
    byte[] GenerarCsvDeudores(List<AlumnoDeudorDto> deudores, decimal totalGlobal);
}
