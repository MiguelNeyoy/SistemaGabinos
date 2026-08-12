namespace SistemaGabinos.Application.DTOs;

public record ReporteFinancieroGeneralDto(
    CorteCajaDto CorteCaja,
    List<AlumnoDeudorDto> Deudores,
    decimal TotalGlobalPorCobrar,
    int TotalAlumnosDeudores);
