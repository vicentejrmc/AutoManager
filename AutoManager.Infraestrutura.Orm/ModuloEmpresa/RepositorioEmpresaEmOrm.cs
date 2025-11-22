using AutoManager.Infraestrutura.Orm.Compartilhado;
using Microsoft.EntityFrameworkCore;
using AutoManager.Dominio.ModuloEmpresa;

namespace AutoManager.Infraestrutura.Orm.ModuloEmpresa;

public class RepositorioEmpresaEmOrm : RepositorioBaseEmOrm<Empresa>, IRepositorioEmpresa
{
    public RepositorioEmpresaEmOrm(AutoManagerDbContext dbContext)
        : base(dbContext) { }

    public override Empresa? SelecionarPorId(Guid idRegistro)
    {
        return dbSet
            .Include(e => e.Funcionarios)
            .Include(e => e.Alugueis)
            .Include(e => e.PlanosCobranca)
            .Include(e => e.Automoveis)
            .Include(e => e.Clientes)
            .Include(e => e.Condutores)
            .Include(e => e.GruposAutomoveis)
            .Include(e => e.PrecoCombustiveis)
            .Include(e => e.TaxasServico)
            .FirstOrDefault(e => e.Id == idRegistro);
    }

    public override List<Empresa> SelecionarTodos()
    {
        return dbSet
            .Include(e => e.Funcionarios)
            .Include(e => e.Alugueis)
            .Include(e => e.PlanosCobranca)
            .Include(e => e.Automoveis)
            .Include(e => e.Clientes)
            .Include(e => e.Condutores)
            .Include(e => e.GruposAutomoveis)
            .Include(e => e.PrecoCombustiveis)
            .Include(e => e.TaxasServico)
            .ToList();
    }
}