using AutoManager.Dominio.Compartilhado;

namespace AutoManager.Dominio.ModuloPrecoCombustivel;

public class PrecoCombustivel : EntidadeBase<PrecoCombustivel>
{
    public string Estado { get; set; }
    public string TipoCombustivel { get; set; }
    public decimal PrecoMedio { get; set; }
    public DateTime DataAtualizacao { get; set; }

    public PrecoCombustivel() { }

    public PrecoCombustivel(string estado, string tipoCombustivel, decimal precoMedio, DateTime dataAtualizacao)
    {
        Estado = estado;
        TipoCombustivel = tipoCombustivel;
        PrecoMedio = precoMedio;
        DataAtualizacao = dataAtualizacao;
    }

    public override void AtualizarRegistro(PrecoCombustivel registroAtualizado)
    {
        Estado = registroAtualizado.Estado;
        TipoCombustivel = registroAtualizado.TipoCombustivel;
        PrecoMedio = registroAtualizado.PrecoMedio;
        DataAtualizacao = registroAtualizado.DataAtualizacao;
    }
}

