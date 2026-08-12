using SistemaGabinos.Application.DTOs;

namespace SistemaGabinos.Application.Interfaces;

public interface IActualizarPreciosUseCase
{
    void Ejecutar(PrecioConfiguracionDto request);
}
