using Microsoft.EntityFrameworkCore;
using SistemaGabinos.Domain.Entities;
using SistemaGabinos.Domain.Interfaces;
using SistemaGabinos.Infrastructure.DataBase.Context;

namespace SistemaGabinos.Infrastructure.DataBase.Repositories;

public class DeudaRepository : Repository<Deuda>, IDeudaRepository
{
    public DeudaRepository(SistemaGabinosDBContext context) : base(context) { }

    public List<Deuda> ObtenerPorAlumno(int alumnoId)
        => DbSet.Where(d => d.AlumnoId == alumnoId).ToList();

    public List<Deuda> ObtenerDeudasPendientesGlobales()
        => DbSet.Where(d => d.MontoPagado < d.MontoTotal).ToList();
}
