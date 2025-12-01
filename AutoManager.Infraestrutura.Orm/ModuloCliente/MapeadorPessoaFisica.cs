using AutoManager.Dominio.ModuloCliente;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoManager.Infraestrutura.Orm.ModuloCliente;

public class MapeadorPessoaFisicaEmOrm : IEntityTypeConfiguration<PessoaFisica>
{
    public void Configure(EntityTypeBuilder<PessoaFisica> builder)
    {
        builder.Property(pf => pf.CPF)
            .HasMaxLength(11)
            .IsRequired();

        builder.HasIndex(pf => pf.CPF).IsUnique();

        builder.Property(pf => pf.RG)
            .HasMaxLength(20);

        builder.Property(pf => pf.CNH)
            .HasMaxLength(20);
    }
}
