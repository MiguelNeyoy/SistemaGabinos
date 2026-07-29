using Microsoft.EntityFrameworkCore;
using SistemaGabinos.Domain.Entities;
using SistemaGabinos.Domain.Interfaces;
using SistemaGabinos.Infrastructure.DataBase.Context;

namespace SistemaGabinos.Infrastructure.DataBase.Repositories;

public class ReciboRepository : Repository<Recibo>, IReciboRepository
{
    public ReciboRepository(SistemaGabinosDBContext context) : base(context) { }

    public Recibo? ObtenerPorPagoId(int pagoId)
        => DbSet.FirstOrDefault(r => r.PagoId == pagoId);

    public Recibo? ObtenerPorFolio(string folio)
        => DbSet.FirstOrDefault(r => r.Folio == folio);
}
