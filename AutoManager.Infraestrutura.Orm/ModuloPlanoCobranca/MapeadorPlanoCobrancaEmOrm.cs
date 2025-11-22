using AutoManager.Dominio.ModuloPlanoCobranca;
using AutoManager.Dominio.ModuloPlanoDeCobranca;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoManager.Infraestrutura.Orm.ModuloPlanoDeCobranca;

public class MapeadorPlanoCobrancaEmOrm : IEntityTypeConfiguration<PlanoCobranca>
{
    public void Configure(EntityTypeBuilder<PlanoCobranca> builder)
    {
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Nome)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ValorDiaria)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.ValorKm)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasOne(x => x.Empresa)
            .WithMany(e => e.PlanosCobranca) // coleção correta na Empresa
            .HasForeignKey(x => x.EmpresaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.GrupoAutomovel)
            .WithMany(g => g.Planos)
            .HasForeignKey(x => x.GrupoAutomovelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Alugueis)
            .WithOne(a => a.PlanoDeCobranca) // navegação correta em Aluguel
            .HasForeignKey(a => a.PlanoDeCobrancaId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.Nome).IsUnique();
    }
}
