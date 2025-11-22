using AutoManager.Dominio.ModuloCliente;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoManager.Infraestrutura.Orm.ModuloCliente;

public class MapeadorClienteEmOrm : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Nome)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Telefone)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne(x => x.Empresa)
            .WithMany(e => e.Clientes)
            .HasForeignKey(c => c.EmpresaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Condutores)
            .WithOne(c => c.Cliente)
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);

        //configuração de herança THP
        builder.HasDiscriminator<string>("TipoCliente")
            .HasValue<PessoaFisica>("PessoaFisica")
            .HasValue<PessoaJuridica>("PessoaJuridica");

        //configurações específicas para PessoaFisica
        builder.HasIndex("CPF").IsUnique();
        builder.Property<string>("CPF").HasMaxLength(11);
        builder.Property<string>("RG").HasMaxLength(20);
        builder.Property<string>("CNH").HasMaxLength(20);

        //configurações específicas para PessoaJuridica
        builder.HasIndex("CNPJ").IsUnique();
        builder.Property<string>("CNPJ").HasMaxLength(14);

    }
}
