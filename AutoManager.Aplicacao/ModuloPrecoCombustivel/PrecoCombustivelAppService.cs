using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloAutenticacao;
using AutoManager.Dominio.ModuloEmpresa;
using AutoManager.Dominio.ModuloPrecoCombustivel;

namespace AutoManager.Aplicacao.ModuloPrecoCombustivel
{
    public class PrecoCombustivelAppService : IAppService<PrecoCombustivel>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IRepositorioPrecoCombustivel repositorioPrecoCombustivel;
        private readonly IRepositorioEmpresa repositorioEmpresa;
        private readonly ITenantProvider tenantProvider;
        private readonly ValidadorPrecoCombustivel validador;

        public PrecoCombustivelAppService(
            IUnitOfWork unitOfWork,
            IRepositorioPrecoCombustivel repositorioPrecoCombustivel,
            IRepositorioEmpresa repositorioEmpresa,
            ITenantProvider tenantProvider,
            ValidadorPrecoCombustivel validador)
        {
            this.unitOfWork = unitOfWork;
            this.repositorioPrecoCombustivel = repositorioPrecoCombustivel;
            this.repositorioEmpresa = repositorioEmpresa;
            this.tenantProvider = tenantProvider;
            this.validador = validador;
        }

        public Result<PrecoCombustivel> Inserir(PrecoCombustivel entidade)
        {
            var resultadoValidacao = validador.Validar(entidade);
            if (resultadoValidacao.Falha)
                return Result<PrecoCombustivel>.Fail(ErrorResults.RequisicaoInvalida(resultadoValidacao.Mensagem));

            var empresa = repositorioEmpresa.SelecionarPorId(entidade.EmpresaId);
            if (empresa == null)
                return Result<PrecoCombustivel>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.EmpresaId));

            entidade.Id = Guid.NewGuid();
            entidade.Empresa = empresa;
            entidade.DataAtualizacao = DateTime.Now;

            try
            {
                repositorioPrecoCombustivel.Inserir(entidade);
                unitOfWork.Commit();

                return Result<PrecoCombustivel>.Ok(entidade, "Preço de combustível registrado com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result<PrecoCombustivel>.Fail(ErrorResults.ErroInterno($"Erro ao inserir preço de combustível: {ex.Message}"));
            }
        }

        public Result<PrecoCombustivel> Editar(PrecoCombustivel entidade)
        {
            var preco = repositorioPrecoCombustivel.SelecionarPorId(entidade.Id);
            if (preco == null)
                return Result<PrecoCombustivel>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.Id));

            var resultadoValidacao = validador.Validar(entidade);
            if (resultadoValidacao.Falha)
                return Result<PrecoCombustivel>.Fail(ErrorResults.RequisicaoInvalida(resultadoValidacao.Mensagem));

            var empresa = repositorioEmpresa.SelecionarPorId(entidade.EmpresaId);
            if (empresa == null)
                return Result<PrecoCombustivel>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.EmpresaId));

            preco.AtualizarRegistro(entidade);
            preco.Empresa = empresa;
            preco.DataAtualizacao = DateTime.Now;

            try
            {
                repositorioPrecoCombustivel.Editar(entidade.Id, preco);
                unitOfWork.Commit();

                return Result<PrecoCombustivel>.Ok(preco, "Preço de combustível atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result<PrecoCombustivel>.Fail(ErrorResults.ErroInterno($"Erro ao editar preço de combustível: {ex.Message}"));
            }
        }

        public Result Excluir(Guid id)
        {
            var preco = repositorioPrecoCombustivel.SelecionarPorId(id);
            if (preco == null)
                return Result.Fail(ErrorResults.RegistroNaoEncontrado(id));

            if (!tenantProvider.IsInRole("Empresa"))
                return Result.Fail(ErrorResults.PermissaoNegada("Apenas usuários do tipo Empresa podem excluir preços de combustível."));

            var empresaIdLogada = tenantProvider.EmpresaId;
            if (empresaIdLogada == null || preco.EmpresaId != empresaIdLogada.Value)
                return Result.Fail(ErrorResults.PermissaoNegada("Não é possível excluir preços de outra empresa."));

            try
            {
                repositorioPrecoCombustivel.Excluir(preco.Id);
                unitOfWork.Commit();

                return Result.Ok("Preço de combustível excluído com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result.Fail(ErrorResults.ErroInterno($"Erro ao excluir preço de combustível: {ex.Message}"));
            }
        }

        public Result<PrecoCombustivel> SelecionarPorId(Guid id)
        {
            var preco = repositorioPrecoCombustivel.SelecionarPorId(id);
            if (preco == null)
                return Result<PrecoCombustivel>.Fail(ErrorResults.RegistroNaoEncontrado(id));

            return Result<PrecoCombustivel>.Ok(preco);
        }

        public Result<List<PrecoCombustivel>> SelecionarTodos()
        {
            var lista = repositorioPrecoCombustivel.SelecionarTodos();
            return Result<List<PrecoCombustivel>>.Ok(lista);
        }
    }
}
