namespace AutoManager.Aplicacao.Compartilhado;

public static class ErrorResults
{
    public static string RegistroDuplicado(string mensagem) =>
          $"Registro duplicado: {mensagem}";

    public static string RegistroNaoEncontrado(Guid id) =>
        $"Registro com ID {id} não encontrado.";

    public static string ExclusaoBloqueada(string mensagem) =>
        $"Exclusão bloqueada: {mensagem}";

    public static string RequisicaoInvalida(string mensagem) =>
        $"Requisição inválida: {mensagem}";

    public static string ErroInterno(string mensagem) =>
        $"Erro interno no servidor: {mensagem}";
}
