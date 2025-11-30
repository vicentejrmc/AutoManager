using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloAluguel;
using AutoManager.Dominio.ModuloAutenticacao;
using AutoManager.Dominio.ModuloEmpresa;
using AutoManager.Dominio.ModuloTaxaServico;

namespace AutoManager.Aplicacao.ModuloTaxaServico
{
    public class TaxaServicoAppService : IAppService<TaxaServico>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IRepositorioTaxaServico repositorioTaxaServico;
        private readonly IRepositorioEmpresa repositorioEmpresa;
        private readonly IRepositorioAluguel repositorioAluguel;
        private readonly ITenantProvider tenantProvider;
        private readonly ValidadorTaxaServico validador;

        public TaxaServicoAppService(
            IUnitOfWork unitOfWork,
            IRepositorioTaxaServico repositorioTaxaServico,
            IRepositorioEmpresa repositorioEmpresa,
            IRepositorioAluguel repositorioAluguel,
            ITenantProvider tenantProvider,
            ValidadorTaxaServico validador)
        {
            this.unitOfWork = unitOfWork;
            this.repositorioTaxaServico = repositorioTaxaServico;
            this.repositorioEmpresa = repositorioEmpresa;
            this.repositorioAluguel = repositorioAluguel;
            this.tenantProvider = tenantProvider;
            this.validador = validador;
        }

        public Result<TaxaServico> Inserir(TaxaServico entidade)
        {
            var resultadoValidacao = validador.Validar(entidade);
            if (resultadoValidacao.Falha)
                return Result<TaxaServico>.Fail(ErrorResults.RequisicaoInvalida(resultadoValidacao.Mensagem));

            var empresa = repositorioEmpresa.SelecionarPorId(entidade.EmpresaId);
            if (empresa == null)
                return Result<TaxaServico>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.EmpresaId));

            entidade.Id = Guid.NewGuid();
            entidade.Empresa = empresa;

            try
            {
                repositorioTaxaServico.Inserir(entidade);
                unitOfWork.Commit();

                return Result<TaxaServico>.Ok(entidade, "Taxa/Serviço registrado com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result<TaxaServico>.Fail(ErrorResults.ErroInterno($"Erro ao inserir taxa/serviço: {ex.Message}"));
            }
        }

        public Result<TaxaServico> Editar(TaxaServico entidade)
        {
            var taxa = repositorioTaxaServico.SelecionarPorId(entidade.Id);
            if (taxa == null)
                return Result<TaxaServico>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.Id));

            var resultadoValidacao = validador.Validar(entidade);
            if (resultadoValidacao.Falha)
                return Result<TaxaServico>.Fail(ErrorResults.RequisicaoInvalida(resultadoValidacao.Mensagem));

            var empresa = repositorioEmpresa.SelecionarPorId(entidade.EmpresaId);
            if (empresa == null)
                return Result<TaxaServico>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.EmpresaId));

            taxa.AtualizarRegistro(entidade);
            taxa.Empresa = empresa;

            try
            {
                repositorioTaxaServico.Editar(entidade.Id, taxa);
                unitOfWork.Commit();

                return Result<TaxaServico>.Ok(taxa, "Taxa/Serviço atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result<TaxaServico>.Fail(ErrorResults.ErroInterno($"Erro ao editar taxa/serviço: {ex.Message}"));
            }
        }

        public Result Excluir(Guid id)
        {
            var taxa = repositorioTaxaServico.SelecionarPorId(id);
            if (taxa == null)
                return Result.Fail(ErrorResults.RegistroNaoEncontrado(id));

            if (!tenantProvider.IsInRole("Empresa"))
                return Result.Fail(ErrorResults.PermissaoNegada("Apenas usuários do tipo Empresa podem excluir taxas/serviços."));

            var empresaIdLogada = tenantProvider.EmpresaId;
            if (empresaIdLogada == null || taxa.EmpresaId != empresaIdLogada.Value)
                return Result.Fail(ErrorResults.PermissaoNegada("Não é possível excluir taxas/serviços de outra empresa."));

            var alugueisAssociados = repositorioAluguel.SelecionarTodos()
                .Where(a => a.Taxas.Any(ts => ts.Id == id) && a.Status == StatusAluguelEnum.EmAndamento)
                .ToList();
            if (alugueisAssociados.Any())
                return Result.Fail(ErrorResults.ExclusaoBloqueada("Não é possível excluir esta taxa/serviço pois está associada a alugueis em andamento."));

            try
            {
                repositorioTaxaServico.Excluir(taxa.Id);
                unitOfWork.Commit();

                return Result.Ok("Taxa/Serviço excluído com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result.Fail(ErrorResults.ErroInterno($"Erro ao excluir taxa/serviço: {ex.Message}"));
            }
        }

        public Result<TaxaServico> SelecionarPorId(Guid id)
        {
            var taxa = repositorioTaxaServico.SelecionarPorId(id);
            if (taxa == null)
                return Result<TaxaServico>.Fail(ErrorResults.RegistroNaoEncontrado(id));

            return Result<TaxaServico>.Ok(taxa);
        }

        public Result<List<TaxaServico>> SelecionarTodos()
        {
            var lista = repositorioTaxaServico.SelecionarTodos();
            return Result<List<TaxaServico>>.Ok(lista);
        }
    }
}
