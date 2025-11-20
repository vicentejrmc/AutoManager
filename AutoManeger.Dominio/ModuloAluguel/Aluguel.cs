using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloAutomoveis;
using AutoManager.Dominio.ModuloCondutor;
using AutoManager.Dominio.ModuloPlanoDeCobrança;
using AutoManager.Dominio.ModuloTaxaServico;

namespace AutoManager.Dominio.ModuloAluguel;

public class Aluguel : EntidadeBase<Aluguel>
{
    public Guid CondutorId { get; set; }
    public Condutor Condutor { get; set; }
    public Guid AutomovelId { get; set; }
    public Automovel Automovel { get; set; }
    public Guid PlanoDeCobrancaId { get; set; }
    public PlanoCobranca PlanoDeCobranca { get; set; }
    public DateTime DataSaida { get; set; }
    public DateTime DataPrevistaRetorno { get; set; }
    public DateTime? DataDevolucao { get; set; }
    public decimal ValorTotal { get; set; }
    public ICollection<TaxaServico> Taxas { get; set; } = new List<TaxaServico>();

    public Aluguel() { }

    public Aluguel(
        Guid condutorId,
        Condutor condutor,
        Guid automovelId,
        Automovel automovel,
        Guid planoDeCobrancaId,
        PlanoCobranca planoDeCobranca,
        DateTime dataSaida,
        DateTime dataPrevistaRetorno,
        DateTime? dataDevolucao,
        decimal valorTotal,
        ICollection<TaxaServico> taxas
    )
    {
        CondutorId = condutorId;
        Condutor = condutor;
        AutomovelId = automovelId;
        Automovel = automovel;
        PlanoDeCobrancaId = planoDeCobrancaId;
        PlanoDeCobranca = planoDeCobranca;
        DataSaida = dataSaida;
        DataPrevistaRetorno = dataPrevistaRetorno;
        DataDevolucao = dataDevolucao;
        ValorTotal = valorTotal;
        Taxas = taxas;
    }

    public override void AtualizarRegistro(Aluguel registroAtualizado)
    {
        CondutorId = registroAtualizado.CondutorId;
        AutomovelId = registroAtualizado.AutomovelId;
        PlanoDeCobrancaId = registroAtualizado.PlanoDeCobrancaId;
        DataSaida = registroAtualizado.DataSaida;
        DataPrevistaRetorno = registroAtualizado.DataPrevistaRetorno;
        DataDevolucao = registroAtualizado.DataDevolucao;
        ValorTotal = registroAtualizado.ValorTotal;
    }
}

