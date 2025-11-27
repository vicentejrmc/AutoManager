using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloAutenticacao;
using AutoManager.Dominio.ModuloCliente;
using AutoManager.Dominio.ModuloCondutor;
using AutoManager.Dominio.ModuloEmpresa;

namespace AutoManager.Aplicacao.ModuloCondutor;

public class CondutorAppService : IAppService<Condutor>
{
    private readonly ITenantProvider tenantProvider;
    private readonly IUnitOfWork unitOfWork;
    private readonly IRepositorioCondutor repositorioCondutor;
    private readonly IRepositorioCliente repositorioCliente;
    private readonly IRepositorioEmpresa repositorioEmpresa;
    private readonly ValidadorCondutor validador;


    public Result<Condutor> Inserir(Condutor entidade)
    {     
        if(!tenantProvider.IsInRole("Empresa") && !tenantProvider.IsInRole("Funcionario"))
            return Result<Condutor>.Fail(ErrorResults.PermissaoNegada("Somente usuario da empresa ou funcionario podem inserir condutores"));

        var empresaIdLogada = tenantProvider.EmpresaId;
        if (empresaIdLogada == null)
            return Result<Condutor>.Fail(ErrorResults.PermissaoNegada("Empresa não identificada."));

        var empresa = repositorioEmpresa.SelecionarPorId(empresaIdLogada.Value);
        if (empresa == null)
            return Result<Condutor>.Fail(ErrorResults.RegistroNaoEncontrado(empresaIdLogada.Value));

        var cliente = repositorioCliente.SelecionarPorId(entidade.ClienteId);
        if (cliente == null)
            return Result<Condutor>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.ClienteId));

        entidade.EmpresaId = empresa.Id;
        entidade.Empresa = empresa;
        entidade.Cliente = cliente;

        var resultadoValidacao = validador.Validar(entidade);
        if (resultadoValidacao.Falha)
            return Result<Condutor>.Fail(resultadoValidacao.Mensagem);

        try
        {
            repositorioCondutor.Inserir(entidade);
            unitOfWork.Commit();

            return Result<Condutor>.Ok(entidade, "Condutor registrado com sucesso.");
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();
            return Result<Condutor>.Fail(ErrorResults.ErroInterno($"Erro ao inserir condutor: {ex.Message}"));
        }
    }


    public Result<Condutor> Editar(Condutor entidade)
    {
        if (!tenantProvider.IsInRole("Empresa") && !tenantProvider.IsInRole("Funcionario"))
            return Result<Condutor>.Fail(ErrorResults.PermissaoNegada("Somente Empresa ou Funcionário podem editar condutores."));

        var condutor = repositorioCondutor.SelecionarPorId(entidade.Id);
        if (condutor == null)
            return Result<Condutor>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.Id));

        var empresaIdLogada = tenantProvider.EmpresaId;
        if (empresaIdLogada == null)
            return Result<Condutor>.Fail(ErrorResults.PermissaoNegada("Empresa não identificada."));

        if (condutor.EmpresaId != empresaIdLogada.Value)
            return Result<Condutor>.Fail(ErrorResults.PermissaoNegada("Não é possível editar condutores de outra empresa."));

        var empresa = repositorioEmpresa.SelecionarPorId(empresaIdLogada.Value);
        if (empresa == null)
            return Result<Condutor>.Fail(ErrorResults.RegistroNaoEncontrado(empresaIdLogada.Value));

        var cliente = repositorioCliente.SelecionarPorId(entidade.ClienteId);
        if (cliente == null)
            return Result<Condutor>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.ClienteId));

        entidade.EmpresaId = empresa.Id;
        entidade.Empresa = empresa;
        entidade.Cliente = cliente;

        var resultadoValidacao = validador.Validar(entidade);
        if (resultadoValidacao.Falha)
            return Result<Condutor>.Fail(resultadoValidacao.Mensagem);

        try
        {
            condutor.AtualizarRegistro(entidade);
            condutor.Empresa = empresa;
            condutor.Cliente = cliente;

            repositorioCondutor.Editar(entidade.Id, condutor);
            unitOfWork.Commit();

            return Result<Condutor>.Ok(condutor, "Condutor atualizado com sucesso.");
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();
            return Result<Condutor>.Fail(ErrorResults.ErroInterno($"Erro ao editar condutor: {ex.Message}"));
        }
    }

    public Result Excluir(Guid id)
    {
        var condutor = repositorioCondutor.SelecionarPorId(id);
        if (condutor == null)
            return Result.Fail(ErrorResults.RegistroNaoEncontrado(id));

        if (!tenantProvider.IsInRole("Empresa"))
            return Result.Fail(ErrorResults.PermissaoNegada("Somente usuários Empresa podem excluir condutores."));

        var empresaIdLogada = tenantProvider.EmpresaId;
        if (empresaIdLogada == null || condutor.EmpresaId != empresaIdLogada.Value)
            return Result.Fail(ErrorResults.PermissaoNegada("Não é possível excluir condutores de outra empresa."));

        try
        {
            repositorioCondutor.Excluir(id);
            unitOfWork.Commit();
            return Result.Ok("Condutor excluído com sucesso.");
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();
            return Result.Fail(ErrorResults.ErroInterno($"Erro ao excluir condutor: {ex.Message}"));
        }
    }

    public Result<Condutor> SelecionarPorId(Guid id)
    {
        if (!tenantProvider.IsInRole("Empresa") && !tenantProvider.IsInRole("Funcionario"))
            return Result<Condutor>.Fail(ErrorResults.PermissaoNegada("Somente Empresa ou Funcionário podem consultar condutores."));

        var condutor = repositorioCondutor.SelecionarPorId(id);
        if (condutor == null)
            return Result<Condutor>.Fail(ErrorResults.RegistroNaoEncontrado(id));

        return Result<Condutor>.Ok(condutor);
    }

    public List<Condutor> SelecionarTodos()
    {
        if (!tenantProvider.IsInRole("Empresa") && !tenantProvider.IsInRole("Funcionario"))
            return new List<Condutor>();

        return repositorioCondutor.SelecionarTodos();
    }
}
