using Microsoft.EntityFrameworkCore;
using SistemaGabinos.Domain.Entities;
using SistemaGabinos.Domain.Interfaces;
using SistemaGabinos.Infrastructure.DataBase.Context;

namespace SistemaGabinos.Infrastructure.DataBase.Repositories;

public class InscripcionRepository : Repository<Inscripcion>, IInscripcionRepository
{
    public InscripcionRepository(SistemaGabinosDBContext context) : base(context) { }

    public List<Inscripcion> ObtenerPorAlumno(int alumnoId)
        => DbSet.Where(i => i.AlumnoId == alumnoId).ToList();
}
