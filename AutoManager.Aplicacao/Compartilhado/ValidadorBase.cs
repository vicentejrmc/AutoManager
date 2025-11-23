namespace AutoManager.Aplicacao.Compartilhado;

public abstract class ValidadorBase<T>
{
    public abstract Result<T> Validar(T entidade);
}
