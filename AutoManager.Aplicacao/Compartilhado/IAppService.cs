namespace AutoManager.Aplicacao.Compartilhado;

public interface IAppService<T>
{
    Result<T> Inserir(T entidade);
    Result<T> Editar(T entidade);
    Result Excluir(Guid id);
    Result<T> SelecionarPorId(Guid id);
    List<T> SelecionarTodos();
}
