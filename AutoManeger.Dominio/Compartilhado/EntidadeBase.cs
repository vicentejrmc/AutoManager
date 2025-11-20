namespace AutoManager.Dominio.Compartilhado;

public abstract class EntidadeBase<T>
{
    public Guid Id { get; set; }
    public abstract void AtualizarRegistro(T registroAtualizado);
}
