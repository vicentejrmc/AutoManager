using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloAluguel;
using AutoManager.Dominio.ModuloAutenticacao;
using AutoManager.Dominio.ModuloAutomoveis;
using AutoManager.Dominio.ModuloEmpresa;
using AutoManager.Dominio.ModuloGrupoAutomovel;
using AutoManager.Dominio.ModuloPlanoCobranca;

namespace AutoManager.Aplicacao.ModuloGrupoAutomovel
{
    public class GrupoAutomovelAppService : IAppService<GrupoAutomovel>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IRepositorioGrupoAutomovel repositorioGrupoAutomovel;
        private readonly IRepositorioAutomovel repositorioAutomovel;
        private readonly IRepositorioEmpresa repositorioEmpresa;
        private readonly IRepositorioPlanoCobranca repositorioPlanoCobranca;
        private readonly IRepositorioAluguel repositorioAluguel;
        private readonly ITenantProvider tenantProvider;
        private readonly ValidadorGrupoAutomovel validador;

        public GrupoAutomovelAppService(
            IUnitOfWork unitOfWork,
            IRepositorioGrupoAutomovel repositorioGrupoAutomovel,
            IRepositorioAutomovel repositorioAutomovel,
            IRepositorioEmpresa repositorioEmpresa,
            IRepositorioPlanoCobranca repositorioPlanoCobranca,
            IRepositorioAluguel repositorioAluguel,
            ITenantProvider tenantProvider,
            ValidadorGrupoAutomovel validador)
        {
            this.unitOfWork = unitOfWork;
            this.repositorioGrupoAutomovel = repositorioGrupoAutomovel;
            this.repositorioAutomovel = repositorioAutomovel;
            this.repositorioEmpresa = repositorioEmpresa;
            this.repositorioPlanoCobranca = repositorioPlanoCobranca;
            this.repositorioAluguel = repositorioAluguel;
            this.tenantProvider = tenantProvider;
            this.validador = validador;
        }

        public Result<GrupoAutomovel> Inserir(GrupoAutomovel entidade)
        {
            var resultadoValidacao = validador.Validar(entidade);
            if (resultadoValidacao.Falha)
                return Result<GrupoAutomovel>.Fail(ErrorResults.RequisicaoInvalida(resultadoValidacao.Mensagem));

            var empresa = repositorioEmpresa.SelecionarPorId(entidade.EmpresaId);
            if (empresa == null)
                return Result<GrupoAutomovel>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.EmpresaId));

            entidade.Id = Guid.NewGuid();
            entidade.Empresa = empresa;

            try
            {
                repositorioGrupoAutomovel.Inserir(entidade);
                unitOfWork.Commit();

                return Result<GrupoAutomovel>.Ok(entidade, "Grupo de Automóvel registrado com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result<GrupoAutomovel>.Fail(ErrorResults.ErroInterno($"Erro ao inserir grupo de automóvel: {ex.Message}"));
            }
        }

        public Result<GrupoAutomovel> Editar(GrupoAutomovel entidade)
        {
            var grupo = repositorioGrupoAutomovel.SelecionarPorId(entidade.Id);
            if (grupo == null)
                return Result<GrupoAutomovel>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.Id));

            var resultadoValidacao = validador.Validar(entidade);
            if (resultadoValidacao.Falha)
                return Result<GrupoAutomovel>.Fail(ErrorResults.RequisicaoInvalida(resultadoValidacao.Mensagem));

            var empresa = repositorioEmpresa.SelecionarPorId(entidade.EmpresaId);
            if (empresa == null)
                return Result<GrupoAutomovel>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.EmpresaId));

            grupo.AtualizarRegistro(entidade);
            grupo.Empresa = empresa;

            try
            {
                repositorioGrupoAutomovel.Editar(entidade.Id, grupo);
                unitOfWork.Commit();

                return Result<GrupoAutomovel>.Ok(grupo, "Grupo de Automóvel atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result<GrupoAutomovel>.Fail(ErrorResults.ErroInterno($"Erro ao editar grupo de automóvel: {ex.Message}"));
            }
        }

        public Result Excluir(Guid id)
        {
            var grupo = repositorioGrupoAutomovel.SelecionarPorId(id);
            if (grupo == null)
                return Result.Fail(ErrorResults.RegistroNaoEncontrado(id));

            if (!tenantProvider.IsInRole("Empresa"))
                return Result.Fail(ErrorResults.PermissaoNegada("Apenas usuários do tipo Empresa podem excluir grupos de automóveis."));

            var empresaIdLogada = tenantProvider.EmpresaId;
            if (empresaIdLogada == null || grupo.EmpresaId != empresaIdLogada.Value)
                return Result.Fail(ErrorResults.PermissaoNegada("Não é possível excluir grupos de outra empresa."));
            
            var automoveisAssociados = repositorioAutomovel.SelecionarTodos()
                .Where(auto => auto.GrupoAutomovelId == id)
                .ToList();

            if (automoveisAssociados.Any())
                return Result.Fail(ErrorResults.ExclusaoBloqueada("Não é possível excluir grupos vinculados a automóveis."));

            var planosAssociados = repositorioPlanoCobranca.SelecionarTodos()
                .Where(plano => plano.GrupoAutomovelId == id)
                .ToList();

            if (planosAssociados.Any())
                return Result.Fail(ErrorResults.ExclusaoBloqueada("Não é possível excluir grupos vinculados a planos de cobrança."));

            var alugueisAssociados = repositorioAluguel.SelecionarTodos()
                .Where(a => a.Automovel.GrupoAutomovelId == id && a.Status == StatusAluguelEnum.EmAndamento)
                .ToList();

            if (alugueisAssociados.Any())
                return Result.Fail(ErrorResults.ExclusaoBloqueada("Não é possível excluir grupos vinculados a aluguéis em andamento."));


            try
            {
                repositorioGrupoAutomovel.Excluir(grupo.Id);
                unitOfWork.Commit();

                return Result.Ok("Grupo de Automóvel excluído com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result.Fail(ErrorResults.ErroInterno($"Erro ao excluir grupo de automóvel: {ex.Message}"));
            }
        }

        public Result<GrupoAutomovel> SelecionarPorId(Guid id)
        {
            var grupo = repositorioGrupoAutomovel.SelecionarPorId(id);
            if (grupo == null)
                return Result<GrupoAutomovel>.Fail(ErrorResults.RegistroNaoEncontrado(id));

            return Result<GrupoAutomovel>.Ok(grupo);
        }

        public Result<List<GrupoAutomovel>> SelecionarTodos()
        {
            var lista = repositorioGrupoAutomovel.SelecionarTodos();
            return Result<List<GrupoAutomovel>>.Ok(lista);
        }
    }
}
