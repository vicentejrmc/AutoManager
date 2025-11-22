using AutoManager.Dominio.ModuloTaxaServico;
using AutoManager.Infraestrutura.Orm.Compartilhado;
using AutoManager.Infraestrutura.Orm.Compatilhado;
using Microsoft.EntityFrameworkCore;

namespace AutoManager.Infraestrutura.Orm.ModuloTaxaServico;

public class RepositorioTaxaServicoEmOrm : RepositorioBaseEmOrm<TaxaServico>, IRepositorioTaxaServico
{
    public RepositorioTaxaServicoEmOrm(AutoManagerDbContext dbContext) : base(dbContext) { }

    public override TaxaServico? SelecionarPorId(Guid idRegistro)
    {
        return dbSet
            .Include(t => t.Empresa)
            .FirstOrDefault(t => t.Id == idRegistro);
    }

    public override List<TaxaServico> SelecionarTodos()
    {
        return dbSet
            .Include(t => t.Empresa)
            .ToList();
    }
}