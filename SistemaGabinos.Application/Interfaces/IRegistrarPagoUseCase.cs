// IRegistrarPagoUseCase.cs
// Interfaz para el caso de uso transaccional de registro de pago en ventanilla.
using SistemaGabinos.Application.DTOs;

namespace SistemaGabinos.Application.Interfaces;

public interface IRegistrarPagoUseCase
{
    RegistrarPagoResponse Ejecutar(RegistrarPagoRequest request);
}
