using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Application.Interfaces;
using SistemaGabinos.Domain.Interfaces;

namespace SistemaGabinos.Application.UseCases;

public class ActualizarPreciosUseCase : IActualizarPreciosUseCase
{
    private readonly IPrecioConfiguracionRepository _precioConfigRepo;

    public ActualizarPreciosUseCase(IPrecioConfiguracionRepository precioConfigRepo)
    {
        _precioConfigRepo = precioConfigRepo;
    }

    public void Ejecutar(PrecioConfiguracionDto request)
    {
        var config = _precioConfigRepo.Obtener();
        config.CambiarPrecios(
            request.CostoInscripcion,
            request.CostoMensualidad,
            request.CostoLibro,
            request.CostoExamenUbicacion,
            request.MontoDescuentoBeca);

        _precioConfigRepo.Guardar(config);
    }
}
