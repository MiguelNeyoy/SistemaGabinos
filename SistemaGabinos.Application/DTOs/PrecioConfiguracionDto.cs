namespace SistemaGabinos.Application.DTOs;

public record PrecioConfiguracionDto(
    decimal CostoInscripcion,
    decimal CostoMensualidad,
    decimal CostoLibro,
    decimal CostoExamenUbicacion,
    decimal MontoDescuentoBeca);
