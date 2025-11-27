using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloAluguel;
using AutoManager.Dominio.ModuloAutenticacao;
using AutoManager.Dominio.ModuloAutomoveis;
using AutoManager.Dominio.ModuloCondutor;
using AutoManager.Dominio.ModuloEmpresa;
using AutoManager.Dominio.ModuloGrupoAutomovel;

namespace AutoManager.Aplicacao.ModuloAutomoveis;

public class AutomovelAppService : IAppService<Automovel>
{
    private readonly ITenantProvider tenantProvider;
    private readonly IUnitOfWork unitOfWork;
    private readonly IRepositorioAutomovel repositorioAutomovel;
    private readonly IRepositorioEmpresa repositorioEmpresa;
    private readonly IRepositorioGrupoAutomovel repositorioGrupoAutomovel;
    private readonly ValidadorAutomovel validadorAutomovel;

    public AutomovelAppService(
        ITenantProvider tenantProvider,
        IUnitOfWork unitOfWork,
        IRepositorioAutomovel repositorioAutomovel,
        IRepositorioEmpresa repositorioEmpresa,
        IRepositorioGrupoAutomovel repositorioGrupoAutomovel,
        ValidadorAutomovel validadorAutomovel
    )
    {
        this.tenantProvider = tenantProvider;
        this.unitOfWork = unitOfWork;
        this.repositorioAutomovel = repositorioAutomovel;
        this.repositorioEmpresa = repositorioEmpresa;
        this.repositorioGrupoAutomovel = repositorioGrupoAutomovel;
        this.validadorAutomovel = validadorAutomovel;
    }

    public Result<Automovel> Inserir(Automovel entidade)
    {
        if(!tenantProvider.IsInRole("Empresa"))
            return Result<Automovel>.Fail(ErrorResults.PermissaoNegada("Apenas usuários com o papel 'Empresa' podem inserir automóveis."));

        var resultValidacao = validadorAutomovel.Validar(entidade);
        if(resultValidacao.Falha)
            return Result<Automovel>.Fail(ErrorResults.RequisicaoInvalida(resultValidacao.Mensagem));

        var empresa = repositorioEmpresa.SelecionarPorId(entidade.EmpresaId);
        if(empresa == null)
            return Result<Automovel>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.EmpresaId));

        var grupoAutomovel = repositorioGrupoAutomovel.SelecionarPorId(entidade.GrupoAutomovelId);
        if(grupoAutomovel == null || grupoAutomovel.EmpresaId != entidade.EmpresaId)
            return Result<Automovel>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.GrupoAutomovelId));

        try
        {
            entidade.Empresa = empresa;
            entidade.GrupoAutomovel = grupoAutomovel;

            repositorioAutomovel.Inserir(entidade);
            unitOfWork.Commit();

            return Result<Automovel>.Ok(entidade, "Automóvel inserido com sucesso.");

        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();
            return Result<Automovel>.Fail(ErrorResults.ErroInterno($"Ocorreu um erro ao inserir o automóvel: {ex.Message}"));

        }
    }

    public Result<Automovel> Editar(Automovel entidade)
    {
        if (!tenantProvider.IsInRole("Empresa") && !tenantProvider.IsInRole("Funcionario"))
            return Result<Automovel>.Fail(ErrorResults.PermissaoNegada("Somente Empresa ou Funcionário podem editar automóveis."));

        var automovel = repositorioAutomovel.SelecionarPorId(entidade.Id);
        if (automovel == null)
            return Result<Automovel>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.Id));

        var empresaIdLogada = tenantProvider.EmpresaId;
        if (empresaIdLogada == null)
            return Result<Automovel>.Fail(ErrorResults.PermissaoNegada("Empresa não identificada."));

        if (automovel.EmpresaId != empresaIdLogada.Value)
            return Result<Automovel>.Fail(ErrorResults.PermissaoNegada("Não é possível editar automóveis de outra empresa."));

        var empresa = repositorioEmpresa.SelecionarPorId(empresaIdLogada.Value);
        if (empresa == null)
            return Result<Automovel>.Fail(ErrorResults.RegistroNaoEncontrado(empresaIdLogada.Value));

        var grupo = repositorioGrupoAutomovel.SelecionarPorId(entidade.GrupoAutomovelId);
        if (grupo == null)
            return Result<Automovel>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.GrupoAutomovelId));

        var resultadoValidacao = validadorAutomovel.Validar(entidade);
        if (resultadoValidacao.Falha)
            return Result<Automovel>.Fail(resultadoValidacao.Mensagem);

        try
        {
            automovel.AtualizarRegistro(entidade);
            automovel.Empresa = empresa;
            automovel.GrupoAutomovel = grupo;

            repositorioAutomovel.Editar(entidade.Id, automovel);
            unitOfWork.Commit();

            return Result<Automovel>.Ok(automovel, "Automóvel atualizado com sucesso.");
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();
            return Result<Automovel>.Fail(ErrorResults.ErroInterno($"Erro ao editar automóvel: {ex.Message}"));
        }
    }


    public Result Excluir(Guid id)
    {
       var automovel = repositorioAutomovel.SelecionarPorId(id);
        if(automovel == null)
            return Result.Fail(ErrorResults.RegistroNaoEncontrado(id));

        if(!tenantProvider.IsInRole("Empresa"))
            return Result.Fail(ErrorResults.PermissaoNegada("Apenas usuário 'Empresa' pode excluir automóveis."));

        //Em caso de Vazamento de Dados, garantir que apenas a empresa proprietária do automóvel possa excluí-lo
        var empresaIdLogada = tenantProvider.EmpresaId;
        if(empresaIdLogada == null || automovel.EmpresaId != empresaIdLogada)
            return Result.Fail(ErrorResults.PermissaoNegada("Apenas a empresa proprietária do automóvel pode excluí-lo."));

        if (automovel.Alugueis.Any(a => a.Status == StatusAluguelEnum.EmAndamento))
            return Result.Fail(ErrorResults.ExclusaoBloqueada("Não é possível excluir um automóvel que está com aluguel em andamento."));

        try
        {
            repositorioAutomovel.Excluir(id);
            unitOfWork.Commit();

            return Result.Ok("Automóvel excluído com sucesso.");
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();
            return Result.Fail(ErrorResults.ErroInterno($"Ocorreu um erro ao excluir o automóvel: {ex.Message}"));
        }
    }

    public Result<Automovel> SelecionarPorId(Guid id)
    {
        if (!tenantProvider.IsInRole("Empresa") && !tenantProvider.IsInRole("Funcionario"))
            return Result<Automovel>.Fail(ErrorResults.PermissaoNegada("Somente Empresa ou Funcionário podem consultar automóveis."));

        var automovel = repositorioAutomovel.SelecionarPorId(id);
        if(automovel == null)
            return Result<Automovel>.Fail(ErrorResults.RegistroNaoEncontrado(id));
        
        return Result<Automovel>.Ok(automovel);
    }

    public List<Automovel> SelecionarTodos()
    {
        if (!tenantProvider.IsInRole("Empresa") && !tenantProvider.IsInRole("Funcionario"))
            return new List<Automovel>();

        return repositorioAutomovel.SelecionarTodos();
    }
}
