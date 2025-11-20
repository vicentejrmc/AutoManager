using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoManeger.Dominio.Compartilhado;

public abstract class EntidadeBase<T>
{
    public Guid Id { get; set; }
    public abstract void AtualizarRegistro(T registroAtualizado);
}
