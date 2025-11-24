using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.ModuloAutenticacao;
using AutoManager.Dominio.ModuloEmpresa;
using AutoManager.Dominio.ModuloFuncionario;
using AutoManager.Infraestrutura.Orm.Compartilhado;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace AutoManager.Aplicacao.ModuloAutenticacao;

public class AutenticacaoAppService
{
    private readonly AutoManagerDbContext dbContext;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ITenantProvider tenantProvider;

    public AutenticacaoAppService(
        AutoManagerDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        ITenantProvider tenantProvider
    )
    {
        this.dbContext = dbContext;
        this.httpContextAccessor = httpContextAccessor;
        this.tenantProvider = tenantProvider;
    }

    public Result RegistrarEmpresa(string usuario, string email, string senhaHash)
    {
        if (dbContext.Empresas.Any(e => e.Email == email))
            return Result.Fail("Já existe uma empresa cadastrada com este e-mail.");

        var aspNetUserId = Guid.NewGuid().ToString();

        var empresa = new Empresa
        {
            Id = Guid.NewGuid(),
            Usuario = usuario,
            Email = email,
            SenhadHash = senhaHash,
            AspNetUserId = aspNetUserId
        };

        dbContext.Empresas.Add(empresa);
        dbContext.SaveChanges();

        return Result.Ok("Empresa registrada com sucesso.");
    }

    public Result RegistrarFuncionario(
        string nome,
        string senhaHash,
        DateTime dataAdmissao,
        decimal salario,
        Guid empresaId
    )
    {
        var empresa = dbContext.Empresas.FirstOrDefault(e => e.Id == empresaId);
        if (empresa == null)
            return Result.Fail(ErrorResults.RegistroNaoEncontrado(empresaId));

        if (dbContext.Funcionarios.Any(f => f.Nome == nome && f.EmpresaId == empresaId))
            return Result.Fail(ErrorResults.RegistroDuplicado($"Ja existe um funcionario chamado {nome} cadsatrado nesta empresa."));

        var aspNetUserId = Guid.NewGuid().ToString();

        var funcionario = new Funcionario(
            nome,
            senhaHash,
            dataAdmissao,
            salario,
            empresaId,
            empresa,
            estaAtivo: true,
            aspNetUserId
        );

        dbContext.Funcionarios.Add(funcionario);
        dbContext.SaveChanges();

        return Result.Ok("Funcionario registrado com sucesso.");
    }

    public async Task<Result> LoginAsync(string usuarioOuEmail, string senhaHash)
    {
        var empresa = dbContext.Empresas.FirstOrDefault(e => e.Email == usuarioOuEmail || e.Usuario == usuarioOuEmail);
        var funcionario = dbContext.Funcionarios.FirstOrDefault(f => f.Nome == usuarioOuEmail);

        if (empresa == null && funcionario == null)
            return Result.Fail($"Usuário como o registro {usuarioOuEmail} não foi encontrado");

        if (empresa != null)
        {
            if (empresa.SenhadHash != senhaHash)
                return Result.Fail(ErrorResults.RequisicaoInvalida("Senha incorreta."));
        }
        else if (funcionario != null)
        {
            if (funcionario.SenhaHash != senhaHash)
                return Result.Fail(ErrorResults.RequisicaoInvalida("Senha incorreta."));
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, empresa != null ? empresa.Usuario : funcionario!.Nome),
            new Claim(ClaimTypes.NameIdentifier, empresa != null ? empresa.AspNetUserId : funcionario!.AspNetUserId),
            new Claim(ClaimTypes.Role, empresa != null ? "Empresa" : "Funcionario"),
            new Claim("EmpresaId", empresa?.Id.ToString() ?? funcionario!.EmpresaId.ToString()),
        };

        var identity = new ClaimsIdentity(claims, "Login");
        var principal = new ClaimsPrincipal(identity);

        await httpContextAccessor.HttpContext!.SignInAsync(principal);

        return Result.Ok("Login realizado com sucesso.");
    }

    public async Task<Result> LogoutAsync()
    {
        await httpContextAccessor.HttpContext!.SignOutAsync();
        return Result.Ok("Logout realizado com sucesso.");
    }

    public Guid ObterEmpresaAtual() 
    {
        return tenantProvider.EmpresaId.GetValueOrDefault();
    }

    public bool UsuarioAutenticado(string role)
    {
        return tenantProvider.IsInRole(role);
    }
}
