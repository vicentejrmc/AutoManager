using AutoManager.Dominio.Compartilhado;

namespace AutoManager.Dominio.ModuloTaxaServico;

public class TaxaServico : EntidadeBase<TaxaServico>
{
    public string Nome { get; set; }
    public decimal Preco { get; set; }
    public bool PrecoPorDia { get; set; }

    public TaxaServico() { }

    public TaxaServico(string nome, decimal preco, bool precoPorDia)
    {
        Nome = nome;
        Preco = preco;
        PrecoPorDia = precoPorDia;
    }

    public override void AtualizarRegistro(TaxaServico registroAtualizado)
    {
        Nome = registroAtualizado.Nome;
        Preco = registroAtualizado.Preco;
        PrecoPorDia = registroAtualizado.PrecoPorDia;
    }
}
