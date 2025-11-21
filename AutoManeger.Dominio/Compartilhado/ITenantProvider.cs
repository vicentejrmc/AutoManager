using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoManager.Dominio.Compartilhado;

public interface ITenantProvider
{
    Guid? UsuarioId { get; }
    bool IsInRole(string roleName);
}
