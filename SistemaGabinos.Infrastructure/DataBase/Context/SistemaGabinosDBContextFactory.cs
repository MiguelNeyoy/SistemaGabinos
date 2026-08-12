using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SistemaGabinos.Infrastructure.DataBase.Context;

public class SistemaGabinosDBContextFactory : IDesignTimeDbContextFactory<SistemaGabinosDBContext>
{
    public SistemaGabinosDBContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SistemaGabinosDBContext>();
        optionsBuilder.UseSqlite("Data Source=SistemaGabinos.db");

        return new SistemaGabinosDBContext(optionsBuilder.Options);
    }
}
