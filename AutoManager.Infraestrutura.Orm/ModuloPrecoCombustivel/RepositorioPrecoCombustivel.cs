using AutoManager.Dominio.ModuloPrecoCombustivel;
using AutoManager.Infraestrutura.Orm.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace AutoManager.Infraestrutura.Orm.ModuloPrecoCombustivel;

public class RepositorioPrecoCombustivelEmOrm : RepositorioBaseEmOrm<PrecoCombustivel>, IRepositorioPrecoCombustivel
{
    public RepositorioPrecoCombustivelEmOrm(AutoManagerDbContext dbContext) : base(dbContext) { }

    public override PrecoCombustivel? SelecionarPorId(Guid idRegistro)
    {
        return dbSet
            .Include(p => p.Empresa)
            .FirstOrDefault(p => p.Id == idRegistro);
    }

    public override List<PrecoCombustivel> SelecionarTodos()
    {
        return dbSet
            .Include(p => p.Empresa)
            .ToList();
    }
}
