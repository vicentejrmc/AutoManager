using AutoManager.Dominio.ModuloPrecoCombustivel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoManager.Infraestrutura.Orm.ModuloPrecoCombustivel;

public class MapeadorPrecoCombustivelEmOrm : IEntityTypeConfiguration<PrecoCombustivel>
{
    public void Configure(EntityTypeBuilder<PrecoCombustivel> builder)
    {
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Estado)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.TipoCombustivel)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.PrecoMedio)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.DataAtualizacao)
            .IsRequired();

        builder.HasOne(x => x.Empresa)
            .WithMany(e => e.PrecoCombustiveis) // precisa adicionar essa coleção na Empresa
            .HasForeignKey(x => x.EmpresaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.Estado, x.TipoCombustivel })
            .IsUnique();
    }
}
