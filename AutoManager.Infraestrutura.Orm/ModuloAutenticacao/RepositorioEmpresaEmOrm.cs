using AutoManager.Dominio.ModuloEmpresa;
using AutoManager.Infraestrutura.Orm.Compartilhado;
using AutoManager.Infraestrutura.Orm.Compatilhado;
using AutoManeger.Dominio.ModuloAutenticacao;
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
            .FirstOrDefault(e => e.Id == idRegistro);
    }
    
    public override List<Empresa> SelecionarTodos()
    {
        return dbSet
            .Include(e => e.Funcionarios)
            .Include(e => e.Alugueis)
            .ToList();
    }
}
