using AutoManager.Dominio.ModuloAutenticacao;
using Microsoft.AspNetCore.Http;

namespace AutoManager.Infraestrutura.Orm.ModuloAutenticacao;

internal class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public Guid? EmpresaId
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;

            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
                return null;

            var empresaClaim = user.FindFirst("EmpresaId")?.Value;
            if (Guid.TryParse(empresaClaim, out var id))
                return id;

            return null;
        }
    }

    public bool IsInRole(string role)
    {
        var user = httpContextAccessor.HttpContext?.User;
        return user?.IsInRole(role) ?? false;
    }
}
