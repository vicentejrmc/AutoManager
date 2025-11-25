using AutoManager.Dominio.ModuloAluguel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoManager.Infraestrutura.Orm.ModuloAluguel;

public class MapeadorAluguelEmOrm : IEntityTypeConfiguration<Aluguel>
{
    public void Configure(EntityTypeBuilder<Aluguel> builder)
    {
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.DataSaida)
            .IsRequired();

        builder.Property(x => x.DataPrevistaRetorno)
            .IsRequired();

        builder.Property(x => x.Ativo)
            .IsRequired();

        builder.Property(x => x.DataDevolucao);

        builder.Property(x => x.ValorTotal)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasOne(a => a.Empresa)
            .WithMany(e => e.Alugueis)
            .HasForeignKey(a => a.EmpresaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Condutor)
            .WithMany(c => c.Alugueis)
            .HasForeignKey(a => a.CondutorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(a => a.Automovel)
            .WithMany(c => c.Alugueis)
            .HasForeignKey(a => a.AutomovelId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(a => a.PlanoDeCobranca)
            .WithMany(p => p.Alugueis)
            .HasForeignKey(a => a.PlanoDeCobrancaId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(a => a.Taxas)
            .WithMany()
            .UsingEntity(j => j.ToTable("AluguelTaxas"));

        builder.HasIndex(x => x.Id).IsUnique();
    }
}
