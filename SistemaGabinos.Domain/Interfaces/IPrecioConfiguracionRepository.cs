using SistemaGabinos.Domain.Entities;

namespace SistemaGabinos.Domain.Interfaces;

public interface IPrecioConfiguracionRepository : IRepository<PrecioConfiguracion>
{
    PrecioConfiguracion Obtener();
}
