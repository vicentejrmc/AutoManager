using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.ModuloAluguel;

namespace AutoManager.Aplicacao.ModuloAluguel;

public class ValidadorAluguel : ValidadorBase<Aluguel>
{
    public override Result<Aluguel> Validar(Aluguel aluguel)
    {
        if (aluguel.CondutorId == Guid.Empty)
            return Result<Aluguel>.Fail(ErrorResults.RequisicaoInvalida("Condutor é Obrigatorio."));

        if (aluguel.AutomovelId == Guid.Empty)
            return Result<Aluguel>.Fail(ErrorResults.RequisicaoInvalida("Automóvel é obrigatório."));

        if (aluguel.PlanoDeCobrancaId == Guid.Empty)
            return Result<Aluguel>.Fail(ErrorResults.RequisicaoInvalida("Plando de cobrança é obrigatório"));

        if (aluguel.DataSaida == default)
            return Result<Aluguel>.Fail(ErrorResults.RequisicaoInvalida("Data de saída é obrigatoria."));

        if (aluguel.DataPrevistaRetorno == default)
            return Result<Aluguel>.Fail(ErrorResults.RequisicaoInvalida("Data prevista para retorno é obrigatória."));

        if (aluguel.DataPrevistaRetorno < aluguel.DataSaida)
            return Result<Aluguel>.Fail(ErrorResults.RequisicaoInvalida("Data prevista para retorno não pode ser menor que a data de saída."));

        if (aluguel.ValorTotal <= 0)
            return Result<Aluguel>.Fail(ErrorResults.RequisicaoInvalida("O valor total do aluguel não pode ser menor ou igual a zero"));

        return Result<Aluguel>.Ok(aluguel);
    }
}
