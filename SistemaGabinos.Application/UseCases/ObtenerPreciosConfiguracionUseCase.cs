using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Application.Interfaces;
using SistemaGabinos.Domain.Interfaces;

namespace SistemaGabinos.Application.UseCases;

public class ObtenerPreciosConfiguracionUseCase : IObtenerPreciosConfiguracionUseCase
{
    private readonly IPrecioConfiguracionRepository _precioConfigRepo;

    public ObtenerPreciosConfiguracionUseCase(IPrecioConfiguracionRepository precioConfigRepo)
    {
        _precioConfigRepo = precioConfigRepo;
    }

    public PrecioConfiguracionDto Ejecutar()
    {
        var config = _precioConfigRepo.Obtener();
        return new PrecioConfiguracionDto(
            config.CostoInscripcion,
            config.CostoMensualidad,
            config.CostoLibro,
            config.CostoExamenUbicacion,
            config.MontoDescuentoBeca);
    }
}
