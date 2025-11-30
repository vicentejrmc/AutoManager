using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.ModuloPlanoCobranca;

namespace AutoManager.Aplicacao.ModuloPlanoCobranca
{
    public class ValidadorPlanoCobranca : ValidadorBase<PlanoCobranca>
    {
        public override Result<PlanoCobranca> Validar(PlanoCobranca entidade)
        {
            if (string.IsNullOrWhiteSpace(entidade.Nome))
                return Result<PlanoCobranca>.Fail(ErrorResults.RequisicaoInvalida("O nome do plano de cobrança é obrigatório."));

            if (entidade.EmpresaId == Guid.Empty)
                return Result<PlanoCobranca>.Fail(ErrorResults.RequisicaoInvalida("O plano de cobrança deve estar vinculado a uma empresa."));

            if (entidade.GrupoAutomovelId == Guid.Empty)
                return Result<PlanoCobranca>.Fail(ErrorResults.RequisicaoInvalida("O plano de cobrança deve estar vinculado a um grupo de automóvel."));

            if (entidade.ValorDiaria <= 0)
                return Result<PlanoCobranca>.Fail(ErrorResults.RequisicaoInvalida("O valor da diária deve ser maior que zero."));

            if (entidade.ValorKm < 0)
                return Result<PlanoCobranca>.Fail(ErrorResults.RequisicaoInvalida("O valor por km não pode ser negativo."));

            return Result<PlanoCobranca>.Ok(entidade);
        }
    }
}
