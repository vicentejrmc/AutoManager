using AutoManager.Dominio.ModuloCondutor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoManager.Infraestrutura.Orm.ModuloCondutor;

public class MapeadorCondutorEmOrm : IEntityTypeConfiguration<Condutor>
{
    public void Configure(EntityTypeBuilder<Condutor> builder)
    {
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Nome)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.CPF)
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(x => x.CNH)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.ValidadeCNH)
            .IsRequired();

        builder.Property(x => x.Telefone)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne(x => x.Cliente)
            .WithMany(c => c.Condutores)
            .HasForeignKey(x => x.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Empresa)
            .WithMany(e => e.Condutores)
            .HasForeignKey(x => x.EmpresaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Alugueis)
            .WithOne(a => a.Condutor)
            .HasForeignKey(a => a.CondutorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.CPF).IsUnique();
        builder.HasIndex(x => x.CNH).IsUnique();
    }
}

