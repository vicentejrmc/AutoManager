using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloAutenticacao;
using AutoManager.Dominio.ModuloEmpresa;
using AutoManager.Dominio.ModuloFuncionario;

namespace AutoManager.Aplicacao.ModuloFuncionario
{
    public class FuncionarioAppService : IAppService<Funcionario>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IRepositorioFuncionario repositorioFuncionario;
        private readonly IRepositorioEmpresa repositorioEmpresa;
        private readonly IPasswordHasher passwordHasher;
        private readonly ITenantProvider tenantProvider;
        private readonly FuncionarioValidador validador;

        public FuncionarioAppService(
            IUnitOfWork unitOfWork,
            IRepositorioFuncionario repositorioFuncionario,
            IRepositorioEmpresa repositorioEmpresa,
            IPasswordHasher passwordHasher,
            ITenantProvider tenantProvider,
            FuncionarioValidador validador)
        {
            this.unitOfWork = unitOfWork;
            this.repositorioFuncionario = repositorioFuncionario;
            this.repositorioEmpresa = repositorioEmpresa;
            this.passwordHasher = passwordHasher;
            this.tenantProvider = tenantProvider;
            this.validador = validador;
        }

        public Result<Funcionario> Inserir(Funcionario entidade)
        {
            var resultadoValidacao = validador.Validar(entidade);
            if (resultadoValidacao.Falha)
                return Result<Funcionario>.Fail(ErrorResults.RequisicaoInvalida(resultadoValidacao.Mensagem));

            var empresa = repositorioEmpresa.SelecionarPorId(entidade.EmpresaId);
            if (empresa == null)
                return Result<Funcionario>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.EmpresaId));

            var funcionarios = repositorioFuncionario.SelecionarTodos();
            if (funcionarios.Any(f => f.Email == entidade.Email && f.EmpresaId == entidade.EmpresaId))
                return Result<Funcionario>.Fail(ErrorResults.RegistroDuplicado($"Já existe um funcionário com e-mail {entidade.Email} nesta empresa."));

            entidade.Id = Guid.NewGuid();
            entidade.SenhaHash = passwordHasher.SenhaHash(entidade.SenhaHash);
            entidade.AspNetUserId = Guid.NewGuid().ToString();
            entidade.EstaAtivo = true;
            entidade.Empresa = empresa;

            try
            {
                repositorioFuncionario.Inserir(entidade);
                unitOfWork.Commit();

                return Result<Funcionario>.Ok(entidade, "Funcionário registrado com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result<Funcionario>.Fail(ErrorResults.ErroInterno($"Erro ao inserir o funcionário: {ex.Message}"));
            }


        }

        public Result<Funcionario> Editar(Funcionario entidade)
        {
            var funcionario = repositorioFuncionario.SelecionarPorId(entidade.Id);
            if (funcionario == null)
                return Result<Funcionario>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.Id));

            var resultadoValidacao = validador.Validar(entidade);
            if (resultadoValidacao.Falha)
                return Result<Funcionario>.Fail(ErrorResults.RequisicaoInvalida(resultadoValidacao.Mensagem));

            var empresa = repositorioEmpresa.SelecionarPorId(entidade.EmpresaId);
            if (empresa == null)
                return Result<Funcionario>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.EmpresaId));

            funcionario.AtualizarRegistro(entidade);
            funcionario.Empresa = empresa;

            if (!string.IsNullOrWhiteSpace(entidade.SenhaHash))
                funcionario.SenhaHash = passwordHasher.SenhaHash(entidade.SenhaHash);


            try
            {
                repositorioFuncionario.Editar(entidade.Id, funcionario);
                unitOfWork.Commit();

                return Result<Funcionario>.Ok(funcionario, "Funcionário atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result<Funcionario>.Fail(ErrorResults.ErroInterno($"Erro ao editar o funcionário: {ex.Message}"));
            }
        }

        public Result Excluir(Guid id)
        {
            var funcionario = repositorioFuncionario.SelecionarPorId(id);
            if (funcionario == null)
                return Result.Fail(ErrorResults.RegistroNaoEncontrado(id));

            if (!tenantProvider.IsInRole("Empresa"))
                return Result.Fail(ErrorResults.PermissaoNegada("Apenas usuários do tipo Empresa podem excluir funcionários."));

            var empresaIdLogada = tenantProvider.EmpresaId;
            if (empresaIdLogada == null)
                return Result.Fail(ErrorResults.PermissaoNegada("Empresa logada inválida."));

            // Verifica se o funcionário pertence à empresa do usuário logado em caso de vazamento de dados.(Se isso acontecer, lascou-se!)
            if (funcionario.EmpresaId != empresaIdLogada.Value)
                return Result.Fail(ErrorResults.PermissaoNegada("Não é possível excluir funcionários de outra empresa."));

            try
            {
                repositorioFuncionario.Excluir(funcionario.Id);
                unitOfWork.Commit();

                return Result.Ok("Funcionário foi excluído do banco de dados com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result.Fail(ErrorResults.ErroInterno($"Erro ao excluir o funcionário: {ex.Message}"));
            }
        }

        public Result<Funcionario> SelecionarPorId(Guid id)
        {
            var funcionario = repositorioFuncionario.SelecionarPorId(id);

            if (funcionario == null)
                return Result<Funcionario>.Fail(ErrorResults.RegistroNaoEncontrado(id));

            return Result<Funcionario>.Ok(funcionario);
        }

        public Result<List<Funcionario>> SelecionarTodos()
        {
            var lista = repositorioFuncionario.SelecionarTodos();
            return Result<List<Funcionario>>.Ok(lista);
        }


        // Auxiliares. apenas desativação/bloqueio do funcionário

        public Result AtivarFuncionario(Guid id)
        {
            return AlterarStatusFuncionario(id, true);
        }

        public Result DesativarFuncionario(Guid id)
        {
            return AlterarStatusFuncionario(id, false);
        }

        public Result AlterarStatusFuncionario(Guid id, bool status)
        {
            if (!tenantProvider.IsInRole("Empresa"))
                return Result.Fail(ErrorResults.PermissaoNegada("Somente usuários do tipo Empresa podem alterar o status de funcionários."));

            var empresaIdLogada = tenantProvider.EmpresaId;
            if (empresaIdLogada == null)
                return Result.Fail(ErrorResults.PermissaoNegada("Empresa logada inválida."));

            var funcionario = repositorioFuncionario.SelecionarPorId(id);
            if (funcionario == null)
                return Result.Fail(ErrorResults.RegistroNaoEncontrado(id));

            if (funcionario.EmpresaId != empresaIdLogada.Value)
                return Result.Fail(ErrorResults.PermissaoNegada("Não é possível alterar status de funcionários de outra empresa."));

            if(status && funcionario.EstaAtivo)
                return Result.Fail(ErrorResults.RequisicaoInvalida("Funcionário já está ativo."));
            
            if(!status && !funcionario.EstaAtivo)
                return Result.Fail(ErrorResults.RequisicaoInvalida("Funcionário já está inativo."));

            try
            {
                funcionario.EstaAtivo = status;
                repositorioFuncionario.Editar(id, funcionario); // apenas atualização
                unitOfWork.Commit();

                var mensagem = status ? "Funcionário ativado com sucesso." : "Funcionário desativado com sucesso.";

                return Result.Ok(mensagem);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result.Fail(ErrorResults.ErroInterno($"Erro ao alterar o status do funcionário: {ex.Message}"));
            }
        }

    }
}
