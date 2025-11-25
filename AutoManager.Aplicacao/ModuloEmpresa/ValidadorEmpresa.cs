using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.ModuloEmpresa;
using System.Text.RegularExpressions;

namespace AutoManager.Aplicacao.ModuloEmpresa
{
    public class ValidadorEmpresa : ValidadorBase<Empresa>
    {
        public override Result<Empresa> Validar(Empresa entidade)
        {
            if (string.IsNullOrWhiteSpace(entidade.Usuario))
                return Result<Empresa>.Fail(ErrorResults.RequisicaoInvalida("Usuário é obrigatório."));

            if (string.IsNullOrWhiteSpace(entidade.Email))
                return Result<Empresa>.Fail(ErrorResults.RequisicaoInvalida("E-mail é obrigatório."));

            if (!Regex.IsMatch(entidade.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return Result<Empresa>.Fail(ErrorResults.RequisicaoInvalida("E-mail inválido."));

            if (string.IsNullOrWhiteSpace(entidade.SenhaHash))
                return Result<Empresa>.Fail(ErrorResults.RequisicaoInvalida("Senha é obrigatória."));

            return Result<Empresa>.Ok(entidade);
        }
    }
}
