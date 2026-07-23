using Microsoft.EntityFrameworkCore;
using SistemaGabinos.Domain.Entities;
using SistemaGabinos.Domain.Interfaces;
using SistemaGabinos.Infrastructure.DataBase.Context;

namespace SistemaGabinos.Infrastructure.DataBase.Repositories;

public class AlumnoRepository : Repository<Alumno>, IAlumnoRepository
{
    public AlumnoRepository(SistemaGabinosDBContext context) : base(context) { }

    public Alumno? ObtenerPorCURP(string curp)
        => DbSet.FirstOrDefault(a => a.CURP == curp);

    public List<Alumno> BuscarPorNombreOCurp(string criterio, int maxResultados = 10)
    {
        if (string.IsNullOrWhiteSpace(criterio))
            return new List<Alumno>();

        var c = criterio.Trim();
        return DbSet
            .Where(a => EF.Functions.Like(a.NombreCompleto, $"%{c}%") || EF.Functions.Like(a.CURP, $"%{c}%"))
            .Take(maxResultados)
            .ToList();
    }

    public List<Alumno> ObtenerTodos()
        => DbSet.ToList();

    public void Eliminar(int id)
    {
        var alumno = DbSet.Find(id);
        if (alumno is not null)
            DbSet.Remove(alumno);

        Context.SaveChanges();
    }
}
