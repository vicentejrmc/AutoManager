using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloAutomoveis;

namespace AutoManager.Dominio.ModuloGrupoAutomovel;

public class GrupoAutomovel : EntidadeBase<GrupoAutomovel>
{
    public string Nome { get; set; }
    public ICollection<PlanoCobranca> Planos { get; set; } = new List<PlanoCobranca>();
    public ICollection<Automovel> Automoveis { get; set; } = new List<Automovel>();

    public GrupoAutomovel(string nome, ICollection<PlanoCobranca> planos, ICollection<Automovel> automoveis)
    {
        Nome = nome;
        Planos = planos;
        Automoveis = automoveis;
    }

    public GrupoAutomovel()
    {
    }

    public override void AtualizarRegistro(GrupoAutomovel registroAtualizado)
    {
        Nome = registroAtualizado.Nome;
        Planos = registroAtualizado.Planos;
        Automoveis = registroAtualizado.Automoveis;
    }

    public void EditarNome(string novoNome)
    {
        if (string.IsNullOrWhiteSpace(novoNome))
            throw new ArgumentException("O nome do grupo de automóveis não pode ser vazio.", nameof(novoNome));

        Nome = novoNome;
    }

    public void ExcluirGrupo()
    {
        if (Automoveis.Any())
            throw new InvalidOperationException("Não é possível excluir um grupo de automóveis que possui automóveis associados.");

        if (Planos.Any())
            throw new InvalidOperationException("Não é possível excluir um grupo associado a planos de Cobrança");
    }

    public string VisualizarResumoGrupo()
    {
        var resumo = $"Grupo: {Nome}\n";
        resumo += "Automóveis:\n";
        foreach (var automovel in Automoveis)
        {
            resumo += $"- {automovel.Modelo} ({automovel.Ano})\n";
        }

        resumo += "Planos de Cobrança:\n";
        foreach (var plano in Planos)
        {
            resumo += $"- {plano.Nome} ({plano.Valor})\n";
        }
        return resumo;
    }
}
