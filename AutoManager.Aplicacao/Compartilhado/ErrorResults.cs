namespace AutoManager.Aplicacao.Compartilhado;

using FluentResults;

public static class ErrorResults
{
    public static Error RegistroDuplicado(string mensagem) =>
       new Error("Registro duplicado").CausedBy(mensagem).WithMetadata("TipoErro", "RegistroDuplicado");

    public static Error RegistroNaoEncontrado(Guid id) =>
        new Error("Registro não encontrad.")
        .CausedBy($"Não foi possível obter o regitro com o ID: {id}")
        .WithMetadata("TipoErro", "RegistroNaoEncontrado");

    public static Error ExclusaoBloqueada(string mensagem) =>
        new Error("Exclusão bloqueada").CausedBy(mensagem).WithMetadata("TipoErro", "ExclusaoBloqueada");

    public static Error RequisicaoInvalida(string mensagem) =>
        new Error("Requisição inválida").CausedBy(mensagem).WithMetadata("TipoErro", "RequisicaoInvalida");

    public static Error ErroInterno(string mensagem) =>
        new Error("Erro interno no servidor").CausedBy(mensagem).WithMetadata("TipoErro", "ErroInterno");
}
