using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloAluguel;
using AutoManager.Dominio.ModuloGrupoAutomovel;

namespace AutoManager.Dominio.ModuloPlanoDeCobrança;

public class PlanoCobranca : EntidadeBase<PlanoCobranca>
{
    public string Nome { get; set; }
    public Guid GrupoAutomovelId { get; set; }
    public GrupoAutomovel GrupoAutomovel { get; set; }
    public decimal ValorDiaria { get; set; }
    public decimal ValorKm { get; set; }
    public ICollection<Aluguel> Alugueis { get; set; } = new List<Aluguel>();

    public PlanoCobranca(){}

    public PlanoCobranca(
        string nome,
        Guid grupoAutomovelId,
        GrupoAutomovel grupoAutomovel,
        decimal valorDiaria,
        decimal valorKm,
        ICollection<Aluguel> alugueis
    )
    {
        Nome = nome;
        GrupoAutomovelId = grupoAutomovelId;
        GrupoAutomovel = grupoAutomovel;
        ValorDiaria = valorDiaria;
        ValorKm = valorKm;
        Alugueis = alugueis;
    }

    public override void AtualizarRegistro(PlanoCobranca registroAtualizado)
    {
        Nome = registroAtualizado.Nome;
        GrupoAutomovelId = registroAtualizado.GrupoAutomovelId;
        GrupoAutomovel = registroAtualizado.GrupoAutomovel;
        ValorDiaria = registroAtualizado.ValorDiaria;
        ValorKm = registroAtualizado.ValorKm;
        Alugueis = registroAtualizado.Alugueis;
    }
}
