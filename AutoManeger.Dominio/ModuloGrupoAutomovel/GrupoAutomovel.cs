using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloAutomoveis;
using AutoManager.Dominio.ModuloPlanoDeCobrança;

namespace AutoManager.Dominio.ModuloGrupoAutomovel;

public class GrupoAutomovel : EntidadeBase<GrupoAutomovel>
{
    public string Nome { get; set; }
    public ICollection<PlanoCobranca> Planos { get; set; } = new List<PlanoCobranca>();
    public ICollection<Automovel> Automoveis { get; set; } = new List<Automovel>();

    public GrupoAutomovel(string nome)
    {
        Nome = nome;
    }

    public override void AtualizarRegistro(GrupoAutomovel registroAtualizado)
    {
        Nome = registroAtualizado.Nome;
    }
}
