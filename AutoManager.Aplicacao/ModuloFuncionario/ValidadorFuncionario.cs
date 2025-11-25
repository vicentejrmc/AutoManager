using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.ModuloFuncionario;
using System.Text.RegularExpressions;

namespace AutoManager.Aplicacao.ModuloFuncionario
{
    public class FuncionarioValidador : ValidadorBase<Funcionario>
    {
        public override Result<Funcionario> Validar(Funcionario entidade)
        {
            if (string.IsNullOrWhiteSpace(entidade.Email))
                return Result<Funcionario>.Fail(ErrorResults.RequisicaoInvalida("E-mail é obrigatório."));

            if (!Regex.IsMatch(entidade.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return Result<Funcionario>.Fail(ErrorResults.RequisicaoInvalida("E-mail inválido."));

            if (string.IsNullOrWhiteSpace(entidade.SenhaHash))
                return Result<Funcionario>.Fail(ErrorResults.RequisicaoInvalida("Senha é obrigatória."));

            if (entidade.DataAdmissao == default)
                return Result<Funcionario>.Fail(ErrorResults.RequisicaoInvalida("Data de admissão é obrigatória."));

            if (entidade.Salario <= 0)
                return Result<Funcionario>.Fail(ErrorResults.RequisicaoInvalida("Salário deve ser maior que zero."));

            if (entidade.EmpresaId == Guid.Empty)
                return Result<Funcionario>.Fail(ErrorResults.RequisicaoInvalida("Empresa vinculada é obrigatória."));

            return Result<Funcionario>.Ok(entidade);
        }
    }
}
