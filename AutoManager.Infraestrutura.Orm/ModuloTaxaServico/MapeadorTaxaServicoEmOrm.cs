using AutoManager.Dominio.ModuloTaxaServico;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoManager.Infraestrutura.Orm.ModuloTaxaServico;

public class MapeadorTaxaServicoEmOrm : IEntityTypeConfiguration<TaxaServico>
{
    public void Configure(EntityTypeBuilder<TaxaServico> builder)
    {
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Nome)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Preco)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.PrecoPorDia)
            .IsRequired();

        builder.HasOne(x => x.Empresa)
            .WithMany(e => e.TaxasServico) // precisa adicionar essa coleção na Empresa
            .HasForeignKey(x => x.EmpresaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.Nome, x.EmpresaId })
            .IsUnique();
    }
}
