using AutoManager.Dominio.ModuloCliente;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoManager.Infraestrutura.Orm.ModuloCliente;

public class MapeadorPessoaJuridicaEmOrm : IEntityTypeConfiguration<PessoaJuridica>
{
    public void Configure(EntityTypeBuilder<PessoaJuridica> builder)
    {
        builder.Property(pj => pj.CNPJ)
            .HasMaxLength(14)
            .IsRequired();

        builder.HasIndex(pj => pj.CNPJ).IsUnique();
    }
}
