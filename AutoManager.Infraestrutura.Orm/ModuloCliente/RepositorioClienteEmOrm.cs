using AutoManager.Dominio.ModuloCliente;
using AutoManager.Infraestrutura.Orm.Compartilhado;
using AutoManager.Infraestrutura.Orm.Compatilhado;
using Microsoft.EntityFrameworkCore;

namespace AutoManager.Infraestrutura.Orm.ModuloClientes;

public class RepositorioClienteEmOrm : RepositorioBaseEmOrm<Cliente>, IRepositorioCliente
{
    public RepositorioClienteEmOrm(AutoManagerDbContext dbContext) : base(dbContext) { }

    public override Cliente? SelecionarPorId(Guid idRegistro)
    {
        return dbSet
            .Include(c => c.Empresa)
            .Include(c => c.Condutores)
            .FirstOrDefault(c => c.Id == idRegistro);
    }

    public override List<Cliente> SelecionarTodos()
    {
        return dbSet
            .Include(c => c.Empresa)
            .Include(c => c.Condutores)
            .ToList();
    }
}
