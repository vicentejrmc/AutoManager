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
            .HasMaxLength(250);

        builder.Property(x => x.AspNetUserId)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasMany(e => e.Funcionarios)
            .WithOne(f => f.Empresa)
            .HasForeignKey(f => f.EmpresaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Alugueis)
            .WithOne(a => a.Empresa)
            .HasForeignKey(a => a.EmpresaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Automoveis)
            .WithOne(a => a.Empresa)
            .HasForeignKey(a => a.EmpresaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Email).IsUnique();
    }
}
