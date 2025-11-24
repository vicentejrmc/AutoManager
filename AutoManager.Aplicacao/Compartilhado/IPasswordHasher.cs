namespace AutoManager.Aplicacao.Compartilhado;

public interface IPasswordHasher
{
    string SenhaHash(string password);
    bool VerificarSenhaHash(string senhaHash, string senhaPlana);
}
