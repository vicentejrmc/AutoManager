using AutoManager.Dominio.ModuloAutenticacao;
using AutoManager.Infraestrutura.Orm.Compartilhado;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AutoManager.Infraestrutura.Orm.ModuloAutenticacao;

internal class TenantProvider : ITenantProvider
{

    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly AutoManagerDbContext dbContext;

    public TenantProvider(IHttpContextAccessor httpContextAccessor, AutoManagerDbContext dbContext)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.dbContext = dbContext;
    }

    public Guid? EmpresaId
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;

            if(user == null || !user.Identity?.IsAuthenticated == true)
                return null;

            var aspNetUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            var empresa = dbContext.Empresas.FirstOrDefault(e => e.AspNetUserId == aspNetUserId);
            if (empresa != null)
                return empresa.Id;

            var funcionario = dbContext.Funcionarios.FirstOrDefault(f => f.AspNetUserId == aspNetUserId);
            if (funcionario != null)
                return funcionario.EmpresaId;

            return null;
        }
    }

    public bool IsInRole(string role)
    {
        var user = httpContextAccessor.HttpContext?.User;
        return user?.IsInRole(role) ?? false;
    }
}
