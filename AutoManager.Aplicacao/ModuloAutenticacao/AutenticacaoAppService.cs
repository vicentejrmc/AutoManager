using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.ModuloAutenticacao;
using AutoManager.Dominio.ModuloEmpresa;
using AutoManager.Dominio.ModuloFuncionario;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using AutoManeger.Dominio.ModuloFuncionario;
using AutoManager.Dominio.Compartilhado;

namespace AutoManager.Aplicacao.ModuloAutenticacao;

public class AutenticacaoAppService
{
    private readonly IRepositorioEmpresa repositorioEmpresa;
    private readonly IRepositorioFuncionario repositorioFuncionario;
    private readonly IUnitOfWork unitOfWork;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ITenantProvider tenantProvider;
    private readonly IPasswordHasher passwordHasher;

    public AutenticacaoAppService(
        IRepositorioEmpresa repositorioEmpresa,
        IRepositorioFuncionario repositorioFuncionario,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        ITenantProvider tenantProvider,
        IPasswordHasher passwordHasher  
    )
    {
        this.repositorioEmpresa = repositorioEmpresa;
        this.repositorioFuncionario = repositorioFuncionario;
        this.unitOfWork = unitOfWork;
        this.httpContextAccessor = httpContextAccessor;
        this.tenantProvider = tenantProvider;
        this.passwordHasher = passwordHasher;
    }

    public Result RegistrarEmpresa(string usuario, string email, string senhaPlana)
    {
        try
        {
            if (repositorioEmpresa.SelecionarTodos().Any(e => e.Email == email))
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

            repositorioEmpresa.Inserir(empresa);
            unitOfWork.Commit();

            return Result.Ok("Empresa registrada com sucesso.");
        }
        catch (Exception ex) 
        { 
            unitOfWork.Rollback();
            return Result.Fail(ErrorResults.ErroInterno($"Erro ao registrar empresa: {ex.Message}"));
        }       
    }

    public Result RegistrarFuncionario(
        string email,
        string senhaPlana,
        DateTime dataAdmissao,
        decimal salario,
        Guid empresaId
    )
    {
        try
        {
            var empresa = repositorioEmpresa.SelecionarPorId(empresaId);
            if (empresa == null)
                return Result.Fail(ErrorResults.RegistroNaoEncontrado(empresaId));

            if (repositorioFuncionario.SelecionarTodos().Any(f => f.Email == email && f.EmpresaId == empresaId))
                return Result.Fail(ErrorResults.RegistroDuplicado($"Ja existe um funcionario com este e-mail: {email} cadsatrado nesta empresa."));

            var aspNetUserId = Guid.NewGuid().ToString();

            var funcionario = new Funcionario(
                email,
                passwordHasher.SenhaHash(senhaPlana),
                dataAdmissao,
                salario,
                empresaId,
                empresa,
                estaAtivo: true,
                aspNetUserId: aspNetUserId
            );

            repositorioFuncionario.Inserir(funcionario);
            unitOfWork.Commit();

            return Result.Ok("Funcionario registrado com sucesso.");
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();
            return Result.Fail(ErrorResults.ErroInterno($"Erro ao registrar Funcionario : {ex.Message}"));
        }
    }

    public async Task<Result> LoginAsync(string usuarioOuEmail, string senhaPlana)
    {
        var empresa = repositorioEmpresa.SelecionarTodos()
            .FirstOrDefault(e => e.Email == usuarioOuEmail || e.Usuario == usuarioOuEmail);

        var funcionario = repositorioFuncionario.SelecionarTodos()
            .FirstOrDefault(f => f.Email == usuarioOuEmail);

        if (empresa == null && funcionario == null)
            return Result.Fail($"Usuário {usuarioOuEmail} não foi encontrado.");

        if (empresa != null)
        {
            if (!passwordHasher.VerificarSenhaHash(empresa.SenhaHash, senhaPlana))
                return Result.Fail(ErrorResults.RequisicaoInvalida("Senha incorreta."));

            if(empresa.Status ==  StatusEmpresaEnum.Inativa)
                return Result.Fail(ErrorResults.RequisicaoInvalida("Empresa inativa. Contate o suporte"));

            if(empresa.Status == StatusEmpresaEnum.PendenteExclusao)
                return Result.Fail(ErrorResults.RequisicaoInvalida("Empresa com solicitação de exclusão pendente. Contate o Suporte"));
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
