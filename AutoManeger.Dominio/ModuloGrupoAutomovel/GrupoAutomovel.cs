using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloAutomoveis;
using AutoManager.Dominio.ModuloEmpresa;
using AutoManager.Dominio.ModuloPlanoDeCobrança;

namespace AutoManager.Dominio.ModuloGrupoAutomovel;

public class GrupoAutomovel : EntidadeBase<GrupoAutomovel>
{
    public string Nome { get; set; }
    public Guid EmpresaId { get; set; }
    public Empresa Empresa { get; set; }
    public ICollection<PlanoCobranca> Planos { get; set; } = new List<PlanoCobranca>();
    public ICollection<Automovel> Automoveis { get; set; } = new List<Automovel>();

    public GrupoAutomovel(string nome, Guid empresaId)
    {
        Nome = nome;
    }

    public override void AtualizarRegistro(GrupoAutomovel registroAtualizado)
    {
        Nome = registroAtualizado.Nome;
        EmpresaId = registroAtualizado.EmpresaId;
    }
}
