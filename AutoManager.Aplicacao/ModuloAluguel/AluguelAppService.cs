using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloAluguel;
using AutoManager.Dominio.ModuloAutenticacao;
using AutoManager.Infraestrutura.Orm.Compartilhado;
using Microsoft.EntityFrameworkCore;

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
            ITenantProvider tenantProvider)
        {
            this.repositorioAluguel = repositorioAluguel;
            this.unitOfWork = unitOfWork;
            this.validador = validador;
            this.tenantProvider = tenantProvider;
        }

        public Result<Aluguel> Inserir(Aluguel entidade)
        {
            var resultadoValidacao = validador.Validar(entidade);
            if (resultadoValidacao.Falha)
                return Result<Aluguel>.Fail(resultadoValidacao.Mensagem);

            var empresaIdLogada = tenantProvider.EmpresaId;
            if (empresaIdLogada == null || empresaIdLogada == Guid.Empty)
                return Result<Aluguel>.Fail(ErrorResults.RequisicaoInvalida("Empresa não identificada no contexto de login."));

            entidade.Id = Guid.NewGuid();
            entidade.EmpresaId = empresaIdLogada.Value; //vínculo automático
            entidade.Ativo = true;

           repositorioAluguel.Inserir(entidade);
            unitOfWork.Commit(); 

            return Result<Aluguel>.Ok(entidade, "Aluguel registrado com sucesso.");
        }

        public Result<Aluguel> Editar(Aluguel entidade)
        {
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
            aluguel.EmpresaId = empresaIdLogada.Value; //reforço do vínculo

            repositorioAluguel.Editar(entidade.Id, aluguel);
            unitOfWork.Commit();

            return Result<Aluguel>.Ok(aluguel, "Aluguel atualizado com sucesso.");
        }

        public Result Excluir(Guid id)
        {
            var aluguel = repositorioAluguel.SelecionarPorId(id);
            if (aluguel == null)
                return Result.Fail(ErrorResults.RegistroNaoEncontrado(id));

            if (!aluguel.Ativo)
                return Result.Fail(ErrorResults.ExclusaoBloqueada("Aluguel já está inativo."));

            aluguel.Ativo = false; //desativar em vez de excluir fisicamente
            repositorioAluguel.Excluir(aluguel.Id);
            unitOfWork.Commit();

            return Result.Ok("Aluguel desativado com sucesso.");
        }

        public Result<Aluguel> SelecionarPorId(Guid id)
        {
            var aluguel = repositorioAluguel.SelecionarPorId(id);

            if (aluguel == null)
                return Result<Aluguel>.Fail(ErrorResults.RegistroNaoEncontrado(id));

            return Result<Aluguel>.Ok(aluguel);
        }

        public List<Aluguel> SelecionarTodos()
        {
            return repositorioAluguel.SelecionarTodos();
        }
    }
}

