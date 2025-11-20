using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoManeger.Dominio.Compartilhado;

public interface IRepositorio<T> where T : EntidadeBase<T>
{
    public void Inserir(T novoRegistro);
    public void Editar(Guid idRegistro, T registroEditado);
    public void Excluir(Guid idRegistro);
    public List<T> SelecionarTodos();
    public T SelecionarPorId(Guid idRegistro);
}
