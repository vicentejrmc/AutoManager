using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AutoManager.Infraestrutura.Orm.Compartilhado;

public class AutoManagerDbContextFactory : IDesignTimeDbContextFactory<AutoManagerDbContext>
{
    public AutoManagerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AutoManagerDbContext>();

        optionsBuilder.UseNpgsql
            ("Host=localhost;Port=5432;Database=AutoManagerDb;Username=postgres;Password=minhasenhafraca");
        return new AutoManagerDbContext(optionsBuilder.Options);
    }
}
