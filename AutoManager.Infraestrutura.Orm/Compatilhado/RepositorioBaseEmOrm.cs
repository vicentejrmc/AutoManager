using AutoManager.Dominio.Compartilhado;
using AutoManager.Infraestrutura.Orm.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace AutoManager.Infraestrutura.Orm.Compatilhado;

public class RepositorioBaseEmOrm<T> where T : EntidadeBase<T>
{
    protected readonly AutoManagerDbContext dbContext;
    protected readonly DbSet<T> dbSet;

    public RepositorioBaseEmOrm(AutoManagerDbContext dbContext)
    {
        this.dbContext = dbContext;
        this.dbSet = dbContext.Set<T>();
    }

    public virtual void Inserir(T entidade)
    {
        dbSet.Add(entidade);
    }

    public virtual void Editar(T entidade)
    {
        dbSet.Update(entidade);
    }

    public virtual void Excluir(T entidade)
    {
        dbSet.Remove(entidade);
    }

    public virtual T? SelecionarPorId(Guid id)
    {
        return dbSet.FirstOrDefault(e => e.Id == id);
    }

    public virtual List<T> SelecionarTodos()
    {
        return dbSet.ToList();
    }
}
