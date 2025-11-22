using AutoManager.Dominio.ModuloAutomoveis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoManager.Infraestrutura.Orm.ModuloAutomoveis;

public class MapeadorAutomovelEmOrm : IEntityTypeConfiguration<Automovel>
{
    public void Configure(EntityTypeBuilder<Automovel> builder)
    {
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Placa)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.Marca)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Modelo)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Cor)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.TipoCombustivel)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.CapacidadeCombustivel)
            .IsRequired();

        builder.Property(x => x.Ano)
            .IsRequired();

        builder.Property(x => x.FotoUrl)
            .HasMaxLength(250);

        builder.HasOne(a => a.GrupoAutomovel)
            .WithMany(g => g.Automoveis)
            .HasForeignKey(a => a.GrupoAutomovelId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(a => a.Empresa)
            .WithMany(e => e.Automoveis)
            .HasForeignKey(a => a.EmpresaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Alugueis)
            .WithOne(al => al.Automovel)
            .HasForeignKey(al => al.AutomovelId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.Placa).IsUnique();
    }
}
