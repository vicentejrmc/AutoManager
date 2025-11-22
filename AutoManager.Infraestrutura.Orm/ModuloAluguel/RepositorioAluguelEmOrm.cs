using AutoManager.Dominio.ModuloAluguel;
using AutoManager.Infraestrutura.Orm.Compartilhado;
using AutoManager.Infraestrutura.Orm.Compatilhado;
using Microsoft.EntityFrameworkCore;

namespace AutoManager.Infraestrutura.Orm.ModuloAluguel;

public class RepositorioAluguelEmOrm : RepositorioBaseEmOrm<Aluguel>, IRepositorioAluguel
{
    public RepositorioAluguelEmOrm(AutoManagerDbContext dbContext) : base(dbContext) { }

    public override Aluguel? SelecionarPorId(Guid id)
    {
        return dbSet
            .Include(a => a.Empresa)
            .Include(a => a.Condutor)
            .Include(a => a.Automovel)
            .Include(a => a.PlanoDeCobranca)
            .Include(a => a.Taxas)
            .FirstOrDefault(a => a.Id == id);
    }

    public override List<Aluguel> SelecionarTodos()
    {
        return dbSet
            .Include(a => a.Empresa)
            .Include(a => a.Condutor)
            .Include(a => a.Automovel)
            .Include(a => a.PlanoDeCobranca)
            .Include(a => a.Taxas)
            .ToList();
    }
}
