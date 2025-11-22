using AutoManager.Dominio.ModuloFuncionario;
using AutoManager.Infraestrutura.Orm.Compartilhado;
using AutoManager.Infraestrutura.Orm.Compatilhado;
using AutoManeger.Dominio.ModuloFuncionario;
using Microsoft.EntityFrameworkCore;


namespace AutoManager.Infraestrutura.Orm.ModuloFuncionario;

public class RepositorioFuncionarioEmOrm : RepositorioBaseEmOrm<Funcionario>, IRepositorioFuncionario
{
    public RepositorioFuncionarioEmOrm(AutoManagerDbContext dbContext) : base(dbContext) {}

    public override Funcionario? SelecionarPorId(Guid idRegistro)
    {
        return dbSet
            .Include(f => f.Empresa)
            .FirstOrDefault(f => f.Id == idRegistro);
    }

    public override List<Funcionario> SelecionarTodos()
    {
        return dbSet
            .Include(f => f.Empresa)
            .ToList();
    }
}
