using AutoManager.Dominio.ModuloAutomoveis;
using AutoManager.Infraestrutura.Orm.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace AutoManager.Infraestrutura.Orm.ModuloAutomoveis;

public class RepositorioAutomovelEmOrm: RepositorioBaseEmOrm<Automovel>, IRepositorioAutomovel
{
    public RepositorioAutomovelEmOrm(AutoManagerDbContext dbContext) : base(dbContext) { }

    public override Automovel? SelecionarPorId(Guid idRegistro)
    {
        return dbSet
            .Include(a => a.Empresa)
            .Include(a => a.GrupoAutomovel)
            .Include(a => a.Alugueis)
            .FirstOrDefault(a => a.Id == idRegistro);
    }

    public override List<Automovel> SelecionarTodos()
    {
        return dbSet
            .Include(a => a.Empresa)
            .Include(a => a.GrupoAutomovel)
            .Include(a => a.Alugueis)
            .ToList();
    }
}

