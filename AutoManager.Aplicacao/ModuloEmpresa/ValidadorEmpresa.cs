using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.ModuloEmpresa;
using System.Text.RegularExpressions;

namespace AutoManager.Aplicacao.ModuloEmpresa
{
    public class ValidadorEmpresa : ValidadorBase<Empresa>
    {
        public override Result<Empresa> Validar(Empresa empresa)
        {
            if (string.IsNullOrWhiteSpace(empresa.Usuario))
                return Result<Empresa>.Fail(ErrorResults.RequisicaoInvalida("Usuário é obrigatório."));

            if (string.IsNullOrWhiteSpace(empresa.Email))
                return Result<Empresa>.Fail(ErrorResults.RequisicaoInvalida("E-mail é obrigatório."));

            if (!Regex.IsMatch(empresa.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return Result<Empresa>.Fail(ErrorResults.RequisicaoInvalida("E-mail inválido."));

            if (string.IsNullOrWhiteSpace(empresa.SenhaHash))
                return Result<Empresa>.Fail(ErrorResults.RequisicaoInvalida("Senha é obrigatória."));

            return Result<Empresa>.Ok(empresa);
        }
    }
}
