using AutoManager.Dominio.ModuloFuncionario;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoManager.Infraestrutura.Orm.ModuloFuncionario;

public class MapeadorFuncionarioEmOrm : IEntityTypeConfiguration<Funcionario>
{
    public void Configure(EntityTypeBuilder<Funcionario> builder)
    {
        builder.Property(x => x.Id)
             .ValueGeneratedNever()
             .IsRequired();

        builder.Property(x => x.Nome)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.senhaHash)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.DataAdmissao)
            .IsRequired();

        builder.Property(x => x.Salario)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.EstaAtivo)
            .IsRequired();

        builder.Property(x => x.AspNetUserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.HasOne(f => f.Empresa)
            .WithMany(e => e.Funcionarios)
            .HasForeignKey(f => f.EmpresaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Id).IsUnique();
    }
}
