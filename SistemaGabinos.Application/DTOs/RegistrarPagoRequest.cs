// RegistrarPagoRequest.cs
// DTO de entrada para el cobro transaccional en ventanilla exprés.
using SistemaGabinos.Domain.Enums;

namespace SistemaGabinos.Application.DTOs;

public record RegistrarPagoRequest(
    int AlumnoId,
    List<int> DeudasSeleccionadasIds,
    decimal MontoRecibido,
    MetodoPago MetodoPago);
