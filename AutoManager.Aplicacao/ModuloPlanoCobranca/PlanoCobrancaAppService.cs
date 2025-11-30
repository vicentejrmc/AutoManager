using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloAluguel;
using AutoManager.Dominio.ModuloAutenticacao;
using AutoManager.Dominio.ModuloEmpresa;
using AutoManager.Dominio.ModuloGrupoAutomovel;
using AutoManager.Dominio.ModuloPlanoCobranca;

namespace AutoManager.Aplicacao.ModuloPlanoCobranca
{
    public class PlanoCobrancaAppService : IAppService<PlanoCobranca>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IRepositorioPlanoCobranca repositorioPlanoCobranca;
        private readonly IRepositorioGrupoAutomovel repositorioGrupoAutomovel;
        private readonly IRepositorioAluguel repositorioAluguel;
        private readonly IRepositorioEmpresa repositorioEmpresa;
        private readonly ITenantProvider tenantProvider;
        private readonly ValidadorPlanoCobranca validador;

        public PlanoCobrancaAppService(
            IUnitOfWork unitOfWork,
            IRepositorioPlanoCobranca repositorioPlanoCobranca,
            IRepositorioGrupoAutomovel repositorioGrupoAutomovel,
            IRepositorioAluguel repositorioAluguel,
            IRepositorioEmpresa repositorioEmpresa,
            ITenantProvider tenantProvider,
            ValidadorPlanoCobranca validador)
        {
            this.unitOfWork = unitOfWork;
            this.repositorioPlanoCobranca = repositorioPlanoCobranca;
            this.repositorioGrupoAutomovel = repositorioGrupoAutomovel;
            this.repositorioAluguel = repositorioAluguel;
            this.repositorioEmpresa = repositorioEmpresa;
            this.tenantProvider = tenantProvider;
            this.validador = validador;
        }

        public Result<PlanoCobranca> Inserir(PlanoCobranca entidade)
        {
            var resultadoValidacao = validador.Validar(entidade);
            if (resultadoValidacao.Falha)
                return Result<PlanoCobranca>.Fail(ErrorResults.RequisicaoInvalida(resultadoValidacao.Mensagem));

            var empresa = repositorioEmpresa.SelecionarPorId(entidade.EmpresaId);
            if (empresa == null)
                return Result<PlanoCobranca>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.EmpresaId));

            var grupo = repositorioGrupoAutomovel.SelecionarPorId(entidade.GrupoAutomovelId);
            if (grupo == null)
                return Result<PlanoCobranca>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.GrupoAutomovelId));

            entidade.Id = Guid.NewGuid();
            entidade.Empresa = empresa;
            entidade.GrupoAutomovel = grupo;

            try
            {
                repositorioPlanoCobranca.Inserir(entidade);
                unitOfWork.Commit();

                return Result<PlanoCobranca>.Ok(entidade, "Plano de cobrança registrado com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result<PlanoCobranca>.Fail(ErrorResults.ErroInterno($"Erro ao inserir plano de cobrança: {ex.Message}"));
            }
        }

        public Result<PlanoCobranca> Editar(PlanoCobranca entidade)
        {
            var plano = repositorioPlanoCobranca.SelecionarPorId(entidade.Id);
            if (plano == null)
                return Result<PlanoCobranca>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.Id));

            var resultadoValidacao = validador.Validar(entidade);
            if (resultadoValidacao.Falha)
                return Result<PlanoCobranca>.Fail(ErrorResults.RequisicaoInvalida(resultadoValidacao.Mensagem));

            var empresa = repositorioEmpresa.SelecionarPorId(entidade.EmpresaId);
            if (empresa == null)
                return Result<PlanoCobranca>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.EmpresaId));

            var grupo = repositorioGrupoAutomovel.SelecionarPorId(entidade.GrupoAutomovelId);
            if (grupo == null)
                return Result<PlanoCobranca>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.GrupoAutomovelId));

            plano.AtualizarRegistro(entidade);
            plano.Empresa = empresa;
            plano.GrupoAutomovel = grupo;

            try
            {
                repositorioPlanoCobranca.Editar(entidade.Id, plano);
                unitOfWork.Commit();

                return Result<PlanoCobranca>.Ok(plano, "Plano de cobrança atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result<PlanoCobranca>.Fail(ErrorResults.ErroInterno($"Erro ao editar plano de cobrança: {ex.Message}"));
            }
        }

        public Result Excluir(Guid id)
        {
            var plano = repositorioPlanoCobranca.SelecionarPorId(id);
            if (plano == null)
                return Result.Fail(ErrorResults.RegistroNaoEncontrado(id));

            if (!tenantProvider.IsInRole("Empresa"))
                return Result.Fail(ErrorResults.PermissaoNegada("Apenas usuários do tipo Empresa podem excluir planos de cobrança."));

            var empresaIdLogada = tenantProvider.EmpresaId;
            if (empresaIdLogada == null || plano.EmpresaId != empresaIdLogada.Value)
                return Result.Fail(ErrorResults.PermissaoNegada("Não é possível excluir planos de outra empresa."));

            var alugueisAssociados = repositorioAluguel.SelecionarTodos()
                .Where(a => a.PlanoDeCobrancaId == id && a.Status == StatusAluguelEnum.EmAndamento)
                .ToList();

            if (alugueisAssociados.Any())
                return Result.Fail(ErrorResults.ExclusaoBloqueada("Não é possível excluir planos de cobrança vinculados a aluguéis em andamento."));


            try
            {
                repositorioPlanoCobranca.Excluir(plano.Id);
                unitOfWork.Commit();

                return Result.Ok("Plano de cobrança excluído com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result.Fail(ErrorResults.ErroInterno($"Erro ao excluir plano de cobrança: {ex.Message}"));
            }
        }

        public Result<PlanoCobranca> SelecionarPorId(Guid id)
        {
            var plano = repositorioPlanoCobranca.SelecionarPorId(id);
            if (plano == null)
                return Result<PlanoCobranca>.Fail(ErrorResults.RegistroNaoEncontrado(id));

            return Result<PlanoCobranca>.Ok(plano);
        }

        public Result<List<PlanoCobranca>> SelecionarTodos()
        {
            var lista = repositorioPlanoCobranca.SelecionarTodos();
            return Result<List<PlanoCobranca>>.Ok(lista);
        }
    }
}
