using AutoManager.Dominio.ModuloEmpresa;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoManager.Infraestrutura.Orm.ModuloAutenticacao;

public class MapeadorEmpresaOrm : IEntityTypeConfiguration<Empresa>
{
    public void Configure(EntityTypeBuilder<Empresa> builder)
    {
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Usuario)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.SenhadHash)
            .HasMaxLength(100);

        builder.Property(x => x.AspNetUserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.HasMany(x => x.Funcionarios)
            .WithOne(f => f.Empresa)
            .HasForeignKey(f => f.EmpresaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Id).IsUnique();
    }
}
