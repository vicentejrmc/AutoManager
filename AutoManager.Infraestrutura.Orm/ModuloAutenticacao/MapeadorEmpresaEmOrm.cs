using AutoManager.Dominio.ModuloEmpresa;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoManager.Infraestrutura.Orm.ModuloAutenticacao;

public class MapeadorEmpresaEmOrm : IEntityTypeConfiguration<Empresa>
{
    public void Configure(EntityTypeBuilder<Empresa> builder)
    {
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Usuario)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.SenhadHash)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.AspNetUserId)
            .HasMaxLength(100)
            .IsRequired();

        // Coleções

        builder.HasMany(e => e.Funcionarios)
            .WithOne(f => f.Empresa)
            .HasForeignKey(f => f.EmpresaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Alugueis)
            .WithOne(a => a.Empresa)
            .HasForeignKey(a => a.EmpresaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.PlanosCobranca)
            .WithOne(p => p.Empresa)
            .HasForeignKey(p => p.EmpresaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.GruposAutomoveis)
            .WithOne(g => g.Empresa)
            .HasForeignKey(g => g.EmpresaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Automoveis)
            .WithOne(a => a.Empresa)
            .HasForeignKey(a => a.EmpresaId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Clientes)
        .WithOne(c => c.Empresa)
        .HasForeignKey(c => c.EmpresaId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Condutores)
            .WithOne(c => c.Empresa)
            .HasForeignKey(c => c.EmpresaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.PrecoCombustiveis)
            .WithOne(c => c.Empresa)
            .HasForeignKey(c => c.EmpresaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.TaxasServico)
            .WithOne(c => c.Empresa)
            .HasForeignKey(c => c.EmpresaId)
            .OnDelete(DeleteBehavior.Cascade);



        builder.HasIndex(x => x.Email).IsUnique();
    }
}
