namespace AutoManager.Aplicacao.Compartilhado;

public class Result
{
    public bool Sucesso { get; }
    public bool Falha => !Sucesso;
    public string Mensagem { get; }

    protected Result(bool sucesso, string mensagem)
    {
        Sucesso = sucesso;
        Mensagem = mensagem;
    }

    public static Result Ok(string mensagem = "") => new Result(true, mensagem);
    public static Result Fail(string mensagem) => new Result(false, mensagem);
}


public class Result<T> : Result
{
    public T Valor { get; }

    protected Result(bool sucesso, T valor, string mensagem) : base(sucesso, mensagem)
    {
        Valor = valor;
    }

    public static Result<T> Ok(T valor, string mensagem = "") => new Result<T>(true, valor, mensagem);
    public static new Result<T> Fail(string mensagem) => new Result<T>(false, default, mensagem);
}
