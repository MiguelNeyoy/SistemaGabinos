using SistemaGabinos.Domain.Entities;
using SistemaGabinos.Domain.Interfaces;
using SistemaGabinos.Infrastructure.DataBase.Context;

namespace SistemaGabinos.Infrastructure.DataBase.Repositories;

public class PrecioConfiguracionRepository : Repository<PrecioConfiguracion>, IPrecioConfiguracionRepository
{
    public PrecioConfiguracionRepository(SistemaGabinosDBContext context) : base(context) { }

    public PrecioConfiguracion Obtener()
    {
        var config = DbSet.FirstOrDefault(p => p.Id == 1);
        if (config is null)
        {
            config = new PrecioConfiguracion(
                costoInscripcion: 1500m,
                costoMensualidad: 1200m,
                costoLibro: 600m,
                costoExamenUbicacion: 300m,
                montoDescuentoBeca: 250m);

            DbSet.Add(config);
            Context.SaveChanges();
        }

        return config;
    }
}
