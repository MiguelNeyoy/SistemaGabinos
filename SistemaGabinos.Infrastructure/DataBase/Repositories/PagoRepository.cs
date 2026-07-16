using Microsoft.EntityFrameworkCore;
using SistemaGabinos.Domain.Entities;
using SistemaGabinos.Domain.Interfaces;
using SistemaGabinos.Infrastructure.DataBase.Context;

namespace SistemaGabinos.Infrastructure.DataBase.Repositories;

public class PagoRepository : Repository<Pago>, IPagoRepository
{
    public PagoRepository(SistemaGabinosDBContext context) : base(context) { }

    public List<Pago> ObtenerPorAlumno(int alumnoId)
        => DbSet.Where(p => p.AlumnoId == alumnoId).ToList();

    public List<Pago> ObtenerPorDeuda(int deudaId)
        => DbSet.Where(p => p.DeudaId == deudaId).ToList();
}
