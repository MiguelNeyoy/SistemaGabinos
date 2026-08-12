using Microsoft.EntityFrameworkCore;
using SistemaGabinos.Domain.Interfaces;
using SistemaGabinos.Infrastructure.DataBase.Context;

namespace SistemaGabinos.Infrastructure.DataBase.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly SistemaGabinosDBContext Context;
    protected readonly DbSet<T> DbSet;

    public Repository(SistemaGabinosDBContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public T? ObtenerPorId(int id) => DbSet.Find(id);

    public void Guardar(T entity)
    {
        if (Context.Entry(entity).State == EntityState.Detached)
            DbSet.Add(entity);

        Context.SaveChanges();
    }
}
