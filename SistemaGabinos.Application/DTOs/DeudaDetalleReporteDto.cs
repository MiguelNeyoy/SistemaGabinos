using SistemaGabinos.Domain.Enums;

namespace SistemaGabinos.Application.DTOs;

public record DeudaDetalleReporteDto(
    ConceptoDeuda Concepto,
    decimal MontoTotal,
    decimal MontoPagado,
    decimal SaldoPendiente);
