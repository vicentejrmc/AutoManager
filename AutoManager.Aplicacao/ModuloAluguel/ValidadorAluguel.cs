using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.ModuloAluguel;
using AutoManager.Dominio.ModuloAutenticacao;

namespace AutoManager.Aplicacao.ModuloAluguel;

public class ValidadorAluguel : ValidadorBase<Aluguel>
{

    private readonly ITenantProvider tenantProvider;

    public ValidadorAluguel(ITenantProvider tenantProvider)
    {
        this.tenantProvider = tenantProvider;
    }

    public override Result<Aluguel> Validar(Aluguel aluguel)
    {
        if (aluguel.CondutorId == Guid.Empty)
            return Result<Aluguel>.Fail(ErrorResults.RequisicaoInvalida("Condutor é Obrigatorio."));

        if (aluguel.AutomovelId == Guid.Empty)
            return Result<Aluguel>.Fail(ErrorResults.RequisicaoInvalida("Automóvel é obrigatório."));

        if (aluguel.PlanoDeCobrancaId == Guid.Empty)
            return Result<Aluguel>.Fail(ErrorResults.RequisicaoInvalida("Plano de cobrança é obrigatório"));

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

    public Result ValidarExclusao(Aluguel aluguel)
    {
        if(!tenantProvider.IsInRole("Empresa"))
            return Result.Fail(ErrorResults.RequisicaoInvalida("Apenas o usuario Empresa pode excluir um aluguel."));

        if(aluguel.Status == StatusAluguelEnum.EmAndamento)
            return Result.Fail(ErrorResults.RequisicaoInvalida("Aluguel em andamento não pode ser excluído."));
        
        return Result.Ok("Exclusão realizada com sucesso.");
    }
}
