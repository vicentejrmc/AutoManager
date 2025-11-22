using AutoManager.Dominio.ModuloCondutor;
using AutoManager.Infraestrutura.Orm.Compartilhado;
using AutoManager.Infraestrutura.Orm.Compatilhado;
using Microsoft.EntityFrameworkCore;

namespace AutoManager.Infraestrutura.Orm.ModuloCondutores;

public class RepositorioCondutorEmOrm : RepositorioBaseEmOrm<Condutor>, IRepositorioCondutor
{
    public RepositorioCondutorEmOrm(AutoManagerDbContext dbContext) : base(dbContext) { }

    public override Condutor? SelecionarPorId(Guid idRegistro)
    {
        return dbSet
            .Include(c => c.Cliente)
            .Include(c => c.Empresa)
            .Include(c => c.Alugueis)
            .FirstOrDefault(c => c.Id == idRegistro);
    }

    public override List<Condutor> SelecionarTodos()
    {
        return dbSet
            .Include(c => c.Cliente)
            .Include(c => c.Empresa)
            .Include(c => c.Alugueis)
            .ToList();
    }
}
