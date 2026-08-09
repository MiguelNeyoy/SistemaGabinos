namespace SistemaGabinos.Application.DTOs;

public record CorteCajaDto(
    DateTime FechaInicio,
    DateTime FechaFin,
    decimal TotalRecaudado,
    decimal TotalEfectivo,
    decimal TotalTarjeta,
    decimal TotalTransferencia,
    decimal TotalInscripciones,
    decimal TotalLibros,
    decimal TotalMensualidades,
    int TotalTransacciones,
    List<PagoDetalleReporteDto> Pagos);
