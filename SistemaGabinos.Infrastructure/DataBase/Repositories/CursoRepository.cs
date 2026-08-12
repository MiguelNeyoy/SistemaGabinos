using Microsoft.EntityFrameworkCore;
using SistemaGabinos.Domain.Entities;
using SistemaGabinos.Domain.Interfaces;
using SistemaGabinos.Infrastructure.DataBase.Context;

namespace SistemaGabinos.Infrastructure.DataBase.Repositories;

public class CursoRepository : Repository<Curso>, ICursoRepository
{
    public CursoRepository(SistemaGabinosDBContext context) : base(context) { }

    public List<Curso> ObtenerTodos()
        => DbSet.ToList();
}
