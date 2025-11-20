namespace AutoManager.Dominio.Compartilhado;

public interface IRepositorio<T> where T : EntidadeBase<T>
{
    public void Inserir(T novoRegistro);
    public void Editar(Guid idRegistro, T registroEditado);
    public void Excluir(Guid idRegistro);
    public List<T> SelecionarTodos();
    public T SelecionarPorId(Guid idRegistro);
}
