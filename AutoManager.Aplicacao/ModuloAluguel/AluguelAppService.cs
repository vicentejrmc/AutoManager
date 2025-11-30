using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloAluguel;
using AutoManager.Dominio.ModuloAutenticacao;

namespace AutoManager.Aplicacao.ModuloAluguel
{
    public class AluguelAppService : IAppService<Aluguel>
    {
        private readonly IRepositorioAluguel repositorioAluguel;
        private readonly IUnitOfWork unitOfWork;
        private readonly ValidadorAluguel validador;
        private readonly ITenantProvider tenantProvider;

        public AluguelAppService(
            IRepositorioAluguel repositorioAluguel,
            IUnitOfWork unitOfWork,
            ValidadorAluguel validador,
            ITenantProvider tenantProvider
        )
        {
            this.repositorioAluguel = repositorioAluguel;
            this.unitOfWork = unitOfWork;
            this.validador = validador;
            this.tenantProvider = tenantProvider;
        }

        public Result<Aluguel> Inserir(Aluguel entidade)
        {

            if (!tenantProvider.IsInRole("Empresa") && !tenantProvider.IsInRole("Funcionario"))
                return Result<Aluguel>.Fail(ErrorResults.PermissaoNegada("Somente Empresa ou Funcionário podem cadastrar aluguéis."));

            var resultadoValidacao = validador.Validar(entidade);
            if (resultadoValidacao.Falha)
                return Result<Aluguel>.Fail(ErrorResults.RequisicaoInvalida(resultadoValidacao.Mensagem));

            var empresaIdLogada = tenantProvider.EmpresaId;
            if (empresaIdLogada == null || empresaIdLogada == Guid.Empty)
                return Result<Aluguel>.Fail(ErrorResults.RequisicaoInvalida("Empresa não identificada no contexto de login."));

            entidade.Id = Guid.NewGuid();
            entidade.EmpresaId = empresaIdLogada.Value;
            entidade.Status = StatusAluguelEnum.EmAndamento;

            try
            {
                repositorioAluguel.Inserir(entidade);
                unitOfWork.Commit();

                return Result<Aluguel>.Ok(entidade, "Aluguel registrado com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result<Aluguel>.Fail(ErrorResults.ErroInterno($"Erro ao registrar aluguel: {ex.Message}"));
            }
        }

        public Result<Aluguel> Editar(Aluguel entidade)
        {
            if (!tenantProvider.IsInRole("Empresa") && !tenantProvider.IsInRole("Funcionario"))
                return Result<Aluguel>.Fail(ErrorResults.PermissaoNegada("Somente Empresa ou Funcionário podem cadastrar aluguéis."));

            var aluguel = repositorioAluguel.SelecionarPorId(entidade.Id);
            if (aluguel == null)
                return Result<Aluguel>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.Id));

            var resultadoValidacao = validador.Validar(entidade);
            if (resultadoValidacao.Falha)
                return Result<Aluguel>.Fail(resultadoValidacao.Mensagem);

            var empresaIdLogada = tenantProvider.EmpresaId;
            if (empresaIdLogada == null || empresaIdLogada == Guid.Empty)
                return Result<Aluguel>.Fail(ErrorResults.RequisicaoInvalida("Empresa não identificada no contexto de login."));

            aluguel.AtualizarRegistro(entidade);
            aluguel.EmpresaId = empresaIdLogada.Value;

            try
            {
                repositorioAluguel.Editar(entidade.Id, aluguel);
                unitOfWork.Commit();

                return Result<Aluguel>.Ok(aluguel, "Aluguel atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result<Aluguel>.Fail(ErrorResults.ErroInterno("Erro durante a edição do aluguel"));
            }
        }

        //Uso Exclusivo do Usuario Administrador 'Empresa'
        public Result Excluir(Guid id)
        {
            if (!tenantProvider.IsInRole("Empresa"))
                return Result<Aluguel>.Fail(ErrorResults.PermissaoNegada("Somente usuário Empresa pode excluir alugueis."));

            var aluguel = repositorioAluguel.SelecionarPorId(id);
            if (aluguel == null)
                return Result.Fail(ErrorResults.RegistroNaoEncontrado(id));

            var resultadoValidacao = validador.ValidarExclusao(aluguel);
            if (resultadoValidacao.Falha)
                return resultadoValidacao;

            try
            {
                repositorioAluguel.Excluir(aluguel.Id);
                unitOfWork.Commit();

                return Result.Ok("Aluguel excluído com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result.Fail(ErrorResults.ErroInterno("Erro ao excluir aluguel fisicamente."));
            }
        }

        public Result<Aluguel> SelecionarPorId(Guid id)
        {
            if (!tenantProvider.IsInRole("Empresa") && !tenantProvider.IsInRole("Funcionario"))
            return Result<Aluguel>.Fail(ErrorResults.PermissaoNegada("Somente Empresa ou Funcionário podem consultar aluguéis."));


            var aluguel = repositorioAluguel.SelecionarPorId(id);

            if (aluguel == null)
                return Result<Aluguel>.Fail(ErrorResults.RegistroNaoEncontrado(id));

            return Result<Aluguel>.Ok(aluguel);
        }

        public Result<List<Aluguel>> SelecionarTodos()
        {
            if (!tenantProvider.IsInRole("Empresa") && !tenantProvider.IsInRole("Funcionario"))
                return Result<List<Aluguel>>.Fail(ErrorResults.PermissaoNegada("Somente Empresa ou Funcionário podem listar aluguéis."));

            var lista = repositorioAluguel.SelecionarTodos();
            return Result<List<Aluguel>>.Ok(lista);
        }

        public Result<Aluguel> FinalizarAluguel(Guid id, DateTime dataDevolucao, decimal valorFinal)
        {
            if (!tenantProvider.IsInRole("Empresa") && !tenantProvider.IsInRole("Funcionario"))
                return Result<Aluguel>.Fail(ErrorResults.PermissaoNegada("Somente Empresa ou Funcionário podem finalizar aluguéis."));

            var aluguel = repositorioAluguel.SelecionarPorId(id);
            if (aluguel == null)
                return Result<Aluguel>.Fail(ErrorResults.RegistroNaoEncontrado(id));

            if (aluguel.Status != StatusAluguelEnum.EmAndamento)
                return Result<Aluguel>.Fail(ErrorResults.RequisicaoInvalida("Aluguel não está em andamento."));

            try
            {
                aluguel.DataDevolucao = dataDevolucao;
                aluguel.ValorTotal = valorFinal;
                aluguel.Status = StatusAluguelEnum.Finalizado;

                repositorioAluguel.Editar(aluguel.Id, aluguel);
                unitOfWork.Commit();

                return Result<Aluguel>.Ok(aluguel, "Aluguel finalizado com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result<Aluguel>.Fail(ErrorResults.ErroInterno("Erro ao finalizar aluguel."));
            }
        }

        public Result<Aluguel> CancelarAluguel(Guid id)
        {
            if (!tenantProvider.IsInRole("Empresa") && !tenantProvider.IsInRole("Funcionario"))
                return Result<Aluguel>.Fail(ErrorResults.PermissaoNegada("Somente Empresa ou Funcionário podem cancelar aluguéis."));

            var aluguel = repositorioAluguel.SelecionarPorId(id);
            if (aluguel == null)
                return Result<Aluguel>.Fail(ErrorResults.RegistroNaoEncontrado(id));

            if (aluguel.Status != StatusAluguelEnum.EmAndamento)
                return Result<Aluguel>.Fail(ErrorResults.RequisicaoInvalida("Somente aluguéis em andamento podem ser cancelados."));

            try
            {
                aluguel.Status = StatusAluguelEnum.Cancelado;
                repositorioAluguel.Editar(aluguel.Id, aluguel);
                unitOfWork.Commit();

                return Result<Aluguel>.Ok(aluguel, "Aluguel cancelado com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result<Aluguel>.Fail(ErrorResults.ErroInterno("Erro ao cancelar aluguel."));
            }
        }
    }
}

