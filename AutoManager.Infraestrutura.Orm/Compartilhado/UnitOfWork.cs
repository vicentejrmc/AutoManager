using AutoManager.Dominio.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace AutoManager.Infraestrutura.Orm.Compartilhado;

public class UnitOfWork : IUnitOfWork
{

    private readonly AutoManagerDbContext dbContext;

    public UnitOfWork(AutoManagerDbContext dbContext)
    {
        this.dbContext = dbContext;
    }


    public void Commit()
    {
        dbContext.SaveChanges();
    }

    public void Rollback()
    {
       foreach(var entry in dbContext.ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.State = EntityState.Unchanged;
                    break;

                case EntityState.Added:
                    entry.State = EntityState.Detached;
                    break;

                case EntityState.Deleted:
                    entry.Reload();
                    break;
            }
        }
    }
}
