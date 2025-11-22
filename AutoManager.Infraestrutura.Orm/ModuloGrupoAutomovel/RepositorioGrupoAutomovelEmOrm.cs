using AutoManager.Dominio.ModuloGrupoAutomovel;
using AutoManager.Infraestrutura.Orm.Compartilhado;
using AutoManager.Infraestrutura.Orm.Compatilhado;
using Microsoft.EntityFrameworkCore;

namespace AutoManager.Infraestrutura.Orm.ModuloGrupoAutomovel;

public class RepositorioGrupoAutomovelEmOrm : RepositorioBaseEmOrm<GrupoAutomovel>, IRepositorioGrupoAutomovel
{
    public RepositorioGrupoAutomovelEmOrm(AutoManagerDbContext dbContext) : base(dbContext) { }

    public override GrupoAutomovel? SelecionarPorId(Guid idRegistro)
    {
        return dbSet
            .Include(g => g.Empresa)
            .Include(g => g.Planos)
            .Include(g => g.Automoveis)
            .FirstOrDefault(g => g.Id == idRegistro);
    }

    public override List<GrupoAutomovel> SelecionarTodos()
    {
        return dbSet
            .Include(g => g.Empresa)
            .Include(g => g.Planos)
            .Include(g => g.Automoveis)
            .ToList();
    }
}

