using AutoManager.Dominio.ModuloGrupoAutomovel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoManager.Infraestrutura.Orm.ModuloGrupoAutomovel;

public class MapeadorGrupoAutomovelEmOrm : IEntityTypeConfiguration<GrupoAutomovel>
{
    public void Configure(EntityTypeBuilder<GrupoAutomovel> builder)
    {
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Nome)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne(x => x.Empresa)
            .WithMany(e => e.GruposAutomoveis)
            .HasForeignKey(x => x.EmpresaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Planos)
            .WithOne(p => p.GrupoAutomovel)
            .HasForeignKey(p => p.GrupoAutomovelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Automoveis)
            .WithOne(a => a.GrupoAutomovel)
            .HasForeignKey(a => a.GrupoAutomovelId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.Nome).IsUnique();
    }
}
