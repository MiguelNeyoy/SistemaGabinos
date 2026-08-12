// RegistrarPagoResponse.cs
// DTO de respuesta con el desglose financiero del cobro registrado.
namespace SistemaGabinos.Application.DTOs;

public record RegistrarPagoResponse(
    List<int> PagosGeneradosIds,
    decimal TotalAbonado,
    decimal CambioEntregado,
    decimal SaldoPendienteRestante,
    string FolioRecibo,
    bool NivelActualizado,
    string Mensaje);
