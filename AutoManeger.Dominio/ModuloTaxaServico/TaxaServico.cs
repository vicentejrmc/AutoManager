using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloEmpresa;

namespace AutoManager.Dominio.ModuloTaxaServico;

public class TaxaServico : EntidadeBase<TaxaServico>
{
    public string Nome { get; set; }
    public decimal Preco { get; set; }
    public bool PrecoPorDia { get; set; }
    public Guid EmpresaId { get; set; }
    public Empresa Empresa { get; set; }

    public TaxaServico() { }

    public TaxaServico(string nome, decimal preco, bool precoPorDia, Guid empresaId)
    {
        Nome = nome;
        Preco = preco;
        PrecoPorDia = precoPorDia;
        EmpresaId = empresaId;
    }

    public override void AtualizarRegistro(TaxaServico registroAtualizado)
    {
        Nome = registroAtualizado.Nome;
        Preco = registroAtualizado.Preco;
        PrecoPorDia = registroAtualizado.PrecoPorDia;
        EmpresaId = registroAtualizado.EmpresaId;
    }
}
