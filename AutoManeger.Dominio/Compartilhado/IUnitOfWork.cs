using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoManeger.Dominio.Compartilhado;

public interface IUnitOfWork
{
    public void Commit();
    public void Rollback();
}
