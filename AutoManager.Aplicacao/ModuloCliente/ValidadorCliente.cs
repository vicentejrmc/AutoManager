using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.ModuloCliente;

namespace AutoManager.Aplicacao.ModuloCliente;

public class ValidadorCliente : ValidadorBase<Cliente>
{
    public override Result<Cliente> Validar(Cliente entidade)
    {
        if (string.IsNullOrWhiteSpace(entidade.Nome))
            return Result<Cliente>.Fail(ErrorResults.RequisicaoInvalida("O nome do cliente é obrigatório."));

        if (string.IsNullOrWhiteSpace(entidade.Telefone))
            return Result<Cliente>.Fail(ErrorResults.RequisicaoInvalida("O telefone do cliente é obrigatório."));

        //Segurança para EmpresaId obrigatório. Ela deve ser informada automaticamente pelo sistema através do contexto da empresa logada.
        if (entidade.EmpresaId == Guid.Empty)
            return Result<Cliente>.Fail(ErrorResults.RequisicaoInvalida("A empresa do cliente é obrigatória."));

        //Validações adicionais para PF
        if(entidade is PessoaFisica pf)
        {
            if (string.IsNullOrWhiteSpace(pf.CPF))
                return Result<Cliente>.Fail(ErrorResults.RequisicaoInvalida("O CPF do cliente é obrigatório."));
            
            if (string.IsNullOrWhiteSpace(pf.RG))
                return Result<Cliente>.Fail(ErrorResults.RequisicaoInvalida("O RG do cliente é obrigatório."));
            
            if (string.IsNullOrWhiteSpace(pf.CNH))
                return Result<Cliente>.Fail(ErrorResults.RequisicaoInvalida("A CNH do cliente é obrigatória."));
        }

        //Validações adicionais para PJ
        if(entidade is PessoaJuridica pj)
        {
            if (string.IsNullOrWhiteSpace(pj.CNPJ))
                return Result<Cliente>.Fail(ErrorResults.RequisicaoInvalida("O CNPJ do cliente é obrigatório."));
        }

        return Result<Cliente>.Ok(entidade);
    }
}
