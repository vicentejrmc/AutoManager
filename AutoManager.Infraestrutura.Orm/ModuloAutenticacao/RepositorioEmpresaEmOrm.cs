using AutoManager.Infraestrutura.Orm.Compartilhado;
using AutoManager.Infraestrutura.Orm.Compatilhado;
using AutoManager.Dominio.ModuloAutenticacao;
using Microsoft.EntityFrameworkCore;

namespace AutoManager.Infraestrutura.Orm.ModuloAutenticacao;

public class RepositorioEmpresaEmOrm : RepositorioBaseEmOrm<Empresa>, IRepositorioAutenticacao
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
