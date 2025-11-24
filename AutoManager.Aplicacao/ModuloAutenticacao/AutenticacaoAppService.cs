using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.ModuloAutenticacao;
using AutoManager.Dominio.ModuloEmpresa;
using AutoManager.Dominio.ModuloFuncionario;
using AutoManager.Infraestrutura.Orm.Compartilhado;
using Microsoft.AspNetCore.Http;

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
        DateTime dataAdmissao,
        decimal salario,
        Guid empresaId
    )
    {
        var empresa = dbContext.Empresas.FirstOrDefault(e => e.Id == empresaId);
        if (empresa == null)
            return Result.Fail(ErrorResults.RegistroNaoEncontrado(empresaId));

        if (dbContext.Funcionarios.Any(f => f.Nome == nome && f.EmpresaId == empresaId))
            return Result.Fail(ErrorResults.RegistroDuplicado($"Ja existe um funcionario {nome} cadsatrado nesta empresa."));

        var aspNetUserId = Guid.NewGuid().ToString();

        var funcionario = new Funcionario(
            nome,
            dataAdmissao,
            salario,
            empresaId,
            empresa,
            estaAtivo: true,
            aspNetUserId
        );

        return Result.Ok("Funcionario registrado com sucesso.");
    }

    public async Task<Result> LoginAsync
}
