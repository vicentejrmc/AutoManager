using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.ModuloPrecoCombustivel;

namespace AutoManager.Aplicacao.ModuloPrecoCombustivel
{
    public class ValidadorPrecoCombustivel : ValidadorBase<PrecoCombustivel>
    {
        public override Result<PrecoCombustivel> Validar(PrecoCombustivel entidade)
        {
            if (string.IsNullOrWhiteSpace(entidade.Estado))
                return Result<PrecoCombustivel>.Fail(ErrorResults.RequisicaoInvalida("O estado é obrigatório."));

            if (string.IsNullOrWhiteSpace(entidade.TipoCombustivel))
                return Result<PrecoCombustivel>.Fail(ErrorResults.RequisicaoInvalida("O tipo de combustível é obrigatório."));

            if (entidade.PrecoMedio <= 0)
                return Result<PrecoCombustivel>.Fail(ErrorResults.RequisicaoInvalida("O preço médio deve ser maior que zero."));

            if (entidade.EmpresaId == Guid.Empty)
                return Result<PrecoCombustivel>.Fail(ErrorResults.RequisicaoInvalida("O preço de combustível deve estar vinculado a uma empresa."));

            return Result<PrecoCombustivel>.Ok(entidade);
        }
    }
}
