using AutoManager.Dominio.ModuloEmpresa;
using AutoManager.Infraestrutura.Orm.Compartilhado;
using AutoManager.Infraestrutura.Orm.Compatilhado;
using Microsoft.EntityFrameworkCore;

namespace AutoManager.Infraestrutura.Orm.ModuloAutenticacao;

public class RepositorioEmpresaEmOrm : RepositorioBaseEmOrm<Empresa>
{
    public RepositorioEmpresaEmOrm(AutoManagerDbContext dbContext)
        : base(dbContext) { }

    public override Empresa? SelecionarPorId(Guid idRegistro)
    {
       return dbSet
            .Include(e => e.Funcionarios)
            .FirstOrDefault(e => e.Id == idRegistro);
    }
    
    public override List<Empresa> SelecionarTodos()
    {
        return dbSet
            .Include(e => e.Funcionarios)
            .ToList();
    }
}
