using AutoManager.Dominio.Compartilhado;
using AutoManager.Infraestrutura.Orm.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace AutoManager.Infraestrutura.Orm.Compartilhado;

public class RepositorioBaseEmOrm<T> : IRepositorio<T> where T : EntidadeBase<T>
{
    protected readonly AutoManagerDbContext dbContext;
    protected readonly DbSet<T> dbSet;

    public RepositorioBaseEmOrm(AutoManagerDbContext dbContext)
    {
        this.dbContext = dbContext;
        this.dbSet = dbContext.Set<T>();
    }

    public void Inserir(T novoRegistro)
    {
        dbSet.Add(novoRegistro);
    }

    public void Editar(Guid idRegistro, T registroEditado)
    {
        var existente = SelecionarPorId(idRegistro);
        if (existente != null)
            existente.AtualizarRegistro(registroEditado);

        dbSet.Update(existente!);
    }

    public void Excluir(Guid idRegistro)
    {
        var existente = SelecionarPorId(idRegistro);
        if (existente != null)
            dbSet.Remove(existente);
    }

    public virtual List<T> SelecionarTodos()
    {
        return dbSet.ToList();
    }

    public virtual T SelecionarPorId(Guid idRegistro)
    {
        return dbSet.FirstOrDefault(x => x.Id == idRegistro)!;
    }
}