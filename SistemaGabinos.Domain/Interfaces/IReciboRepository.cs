// IReciboRepository.cs
// Contrato para la persistencia y consulta de recibos generados.
using SistemaGabinos.Domain.Entities;

namespace SistemaGabinos.Domain.Interfaces;

public interface IReciboRepository : IRepository<Recibo>
{
    Recibo? ObtenerPorPagoId(int pagoId);
    Recibo? ObtenerPorFolio(string folio);
}
