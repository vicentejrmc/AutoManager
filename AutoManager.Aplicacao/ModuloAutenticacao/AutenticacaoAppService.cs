using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.ModuloAutenticacao;
using AutoManager.Dominio.ModuloEmpresa;
using AutoManager.Dominio.ModuloFuncionario;
using AutoManager.Infraestrutura.Orm.Compartilhado;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace AutoManager.Aplicacao.ModuloAutenticacao;

public class AutenticacaoAppService
{
    private readonly AutoManagerDbContext dbContext;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ITenantProvider tenantProvider;
    private readonly IPasswordHasher passwordHasher;

    public AutenticacaoAppService(
        AutoManagerDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        ITenantProvider tenantProvider,
        IPasswordHasher passwordHasher  
    )
    {
        this.dbContext = dbContext;
        this.httpContextAccessor = httpContextAccessor;
        this.tenantProvider = tenantProvider;
        this.passwordHasher = passwordHasher;
    }

    public Result RegistrarEmpresa(string usuario, string email, string senhaPlana)
    {
        if (dbContext.Empresas.IgnoreQueryFilters().Any(e => e.Email == email))
            return Result.Fail(ErrorResults.RegistroDuplicado(
                $"Já existe uma empresa com este e-mail: {email}"));

        var aspNetUserId = Guid.NewGuid().ToString();

        var empresa = new Empresa
        {
            Id = Guid.NewGuid(),
            Usuario = usuario,
            Email = email,
            SenhaHash = passwordHasher.SenhaHash(senhaPlana),
            AspNetUserId = aspNetUserId
        };

        dbContext.Empresas.Add(empresa);
        dbContext.SaveChanges();

        return Result.Ok("Empresa registrada com sucesso.");
    }

    public Result RegistrarFuncionario(
        string email,
        string senhaPlana,
        DateTime dataAdmissao,
        decimal salario,
        Guid empresaId
    )
    {
        var empresa = dbContext.Empresas.FirstOrDefault(e => e.Id == empresaId);
        if (empresa == null)
            return Result.Fail(ErrorResults.RegistroNaoEncontrado(empresaId));

        if (dbContext.Funcionarios.Any(f => f.Email == email && f.EmpresaId == empresaId))
            return Result.Fail(ErrorResults.RegistroDuplicado($"Ja existe um funcionario com este email: {email} cadsatrado nesta empresa."));

        var aspNetUserId = Guid.NewGuid().ToString();

        var funcionario = new Funcionario(
            email,
            passwordHasher.SenhaHash(senhaPlana),
            dataAdmissao,
            salario,
            empresaId,
            empresa,
            estaAtivo: true,
            aspNetUserId : Guid.NewGuid().ToString()
        );

        dbContext.Funcionarios.Add(funcionario);
        dbContext.SaveChanges();

        return Result.Ok("Funcionario registrado com sucesso.");
    }

    public async Task<Result> LoginAsync(string usuarioOuEmail, string senhaPlana)
    {
        var empresa = dbContext.Empresas
            .IgnoreQueryFilters()
            .FirstOrDefault(e => e.Email == usuarioOuEmail || e.Usuario == usuarioOuEmail);

        var funcionario = dbContext.Funcionarios
            .IgnoreQueryFilters()
            .FirstOrDefault(f => f.Email == usuarioOuEmail);

        if (empresa == null && funcionario == null)
            return Result.Fail($"Usuário {usuarioOuEmail} não foi encontrado.");

        if (empresa != null)
        {
            if (!passwordHasher.VerificarSenhaHash(empresa.SenhaHash, senhaPlana))
                return Result.Fail(ErrorResults.RequisicaoInvalida("Senha incorreta."));
        }
        else if (funcionario != null)
        {
            if (!passwordHasher.VerificarSenhaHash(funcionario.SenhaHash, senhaPlana))
                return Result.Fail(ErrorResults.RequisicaoInvalida("Senha incorreta."));
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, empresa != null ? empresa.Usuario : funcionario!.Email),
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
