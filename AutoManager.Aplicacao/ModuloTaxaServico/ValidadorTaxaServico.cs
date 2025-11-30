using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.ModuloTaxaServico;

namespace AutoManager.Aplicacao.ModuloTaxaServico
{
    public class ValidadorTaxaServico : ValidadorBase<TaxaServico>
    {
        public override Result<TaxaServico> Validar(TaxaServico entidade)
        {
            if (string.IsNullOrWhiteSpace(entidade.Nome))
                return Result<TaxaServico>.Fail(ErrorResults.RequisicaoInvalida("O nome da taxa/serviço é obrigatório."));

            if (entidade.Preco <= 0)
                return Result<TaxaServico>.Fail(ErrorResults.RequisicaoInvalida("O preço da taxa/serviço deve ser maior que zero."));

            if (entidade.EmpresaId == Guid.Empty)
                return Result<TaxaServico>.Fail(ErrorResults.RequisicaoInvalida("A taxa/serviço deve estar vinculada a uma empresa."));

            return Result<TaxaServico>.Ok(entidade);
        }
    }
}
