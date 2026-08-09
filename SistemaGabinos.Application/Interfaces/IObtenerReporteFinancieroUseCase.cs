using SistemaGabinos.Application.DTOs;

namespace SistemaGabinos.Application.Interfaces;

public interface IObtenerReporteFinancieroUseCase
{
    ReporteFinancieroGeneralDto GenerarReporte(DateTime? inicio = null, DateTime? fin = null);
    CorteCajaDto GenerarCorteCaja(DateTime inicio, DateTime fin);
    List<AlumnoDeudorDto> ObtenerDeudores();
}
