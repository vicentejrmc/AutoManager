using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloAluguel;
using AutoManager.Dominio.ModuloAutenticacao;
using AutoManager.Dominio.ModuloAutomoveis;
using AutoManager.Dominio.ModuloCliente;
using AutoManager.Dominio.ModuloCondutor;
using AutoManager.Dominio.ModuloEmpresa;
using AutoManager.Dominio.ModuloFuncionario;
using AutoManager.Dominio.ModuloGrupoAutomovel;
using AutoManager.Dominio.ModuloPlanoDeCobrança;
using AutoManager.Dominio.ModuloPrecoCombustivel;
using AutoManager.Dominio.ModuloTaxaServico;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoManager.Infraestrutura.Orm.Compartilhado;

public class AutoManagerDbContext : IdentityDbContext, IUnitOfWork
{
    public DbSet<Empresa> Empresas { get; set; }
    public DbSet<Funcionario> Funcionarios { get; set; }
    public DbSet<GrupoAutomovel> GruposAutomoveis { get; set; }
    public DbSet<PlanoCobranca> PlanosCobranca { get; set; }
    public DbSet<Automovel> Automoveis { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<PessoaFisica> PessoasFisicas { get; set; }
    public DbSet<PessoaJuridica> PessoasJuridicas { get; set; }
    public DbSet<Condutor> Condutores { get; set; }
    public DbSet<TaxaServico> TaxasServicos { get; set; }
    public DbSet<Aluguel> Alugueis { get; set; }
    public DbSet<PrecoCombustivel> PrecosCombustivel { get; set; }

    private readonly ITenantProvider? tenantProvider;

    public AutoManagerDbContext(DbContextOptions options, ITenantProvider? tenantProvider = null)
        : base(options)
    {
        this.tenantProvider = tenantProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (tenantProvider is not null)
        {
            modelBuilder.Entity<Empresa>()
                .HasQueryFilter(x => tenantProvider.UsuarioId != null && x.Id.Equals(tenantProvider.UsuarioId));
        }

        var assembly = typeof(AutoManagerDbContext).Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);

        base.OnModelCreating(modelBuilder);
    }

    public void Commit()
    {
        SaveChanges();
    }

    public void Rollback()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                case EntityState.Modified:
                case EntityState.Deleted:
                    entry.State = EntityState.Unchanged;
                    break;
            }
        }
    }
}