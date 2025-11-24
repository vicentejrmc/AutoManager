using AutoManager.Aplicacao.Compartilhado;
using Microsoft.AspNetCore.Identity;

namespace AutoManager.Aplicacao.ModuloAutenticacao;

public class SenhaHasherService : IPasswordHasher
{
    private readonly PasswordHasher<object> _hasher = new();

    public string SenhaHash(string senhaPlana)
    {
        return _hasher.HashPassword(null, senhaPlana);
    }

    public bool VerificarSenhaHash(string senhaHash, string senhaPlana)
    {
        var result = _hasher.VerifyHashedPassword(null, senhaHash, senhaPlana);
        return result != PasswordVerificationResult.Failed;
    }
}
