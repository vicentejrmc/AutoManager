using AutoManager.Dominio.ModuloPlanoCobranca;
using AutoManager.Infraestrutura.Orm.Compartilhado;
using AutoManager.Infraestrutura.Orm.Compatilhado;
using Microsoft.EntityFrameworkCore;

namespace AutoManager.Infraestrutura.Orm.ModuloPlanoCobranca;

public class RepositorioPlanoCobrancaEmOrm : RepositorioBaseEmOrm<PlanoCobranca>, IRepositorioPlanoCobranca
{
    public RepositorioPlanoCobrancaEmOrm(AutoManagerDbContext dbContext) : base(dbContext) { }

    public override PlanoCobranca? SelecionarPorId(Guid idRegistro)
    {
        return dbSet
            .Include(p => p.Empresa)
            .Include(p => p.GrupoAutomovel)
            .Include(p => p.Alugueis)
            .FirstOrDefault(p => p.Id == idRegistro);
    }

    public override List<PlanoCobranca> SelecionarTodos()
    {
        return dbSet
            .Include(p => p.Empresa)
            .Include(p => p.GrupoAutomovel)
            .Include(p => p.Alugueis)
            .ToList();
    }
}
