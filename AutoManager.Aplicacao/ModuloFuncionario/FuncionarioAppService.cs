using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloAutenticacao;
using AutoManager.Dominio.ModuloEmpresa;
using AutoManager.Dominio.ModuloFuncionario;
using AutoManeger.Dominio.ModuloFuncionario;
using Microsoft.EntityFrameworkCore;

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
                return Result<Funcionario>.Fail(resultadoValidacao.Mensagem);

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
                return Result<Funcionario>.Fail(resultadoValidacao.Mensagem);

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
                return Result.Fail(ErrorResults.RequisicaoInvalida("Apenas usuários do tipo Empresa podem excluir funcionários."));

            var empresaIdLogada = tenantProvider.EmpresaId;

            if (empresaIdLogada == null)
                return Result.Fail(ErrorResults.RequisicaoInvalida("Empresa logada inválida."));

            // Verifica se o funcionário pertence à empresa do usuário logado em caso de vazamento de dados.(Se isso acontecer, lascou-se!)
            if (funcionario.EmpresaId != empresaIdLogada)
                return Result.Fail(ErrorResults.RequisicaoInvalida("Não é possível excluir funcionários de outra empresa."));

            if (!funcionario.EstaAtivo)
                return Result.Fail(ErrorResults.ExclusaoBloqueada("Funcionário já está inativo."));

            try
            {
                funcionario.EstaAtivo = false;
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

        public List<Funcionario> SelecionarTodos()
        {
            return repositorioFuncionario.SelecionarTodos();
        }


        // Auxiliares para uso interno da Empresa em caso de apenas desativação/bloqueio do funcionário

        public Result AtivarFuncionario(Guid id)
        {
            return AlterarStatusFuncionario(id, true);
        }

        public Result DesativarFuncionario(Guid id)
        {
            return AlterarStatusFuncionario(id, false);
        }

        public Result AlterarStatusFuncionario(Guid id, bool ativar)
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
                return Result.Fail(ErrorResults.PermissaoNegada("Não é possível desativar funcionários de outra empresa."));

            if(ativar && funcionario.EstaAtivo)
                return Result.Fail(ErrorResults.RequisicaoInvalida("Funcionário já está ativo."));
            if(!ativar && !funcionario.EstaAtivo)
                return Result.Fail(ErrorResults.RequisicaoInvalida("Funcionário já está inativo."));

            try
            {
                funcionario.EstaAtivo = false;
                repositorioFuncionario.Editar(id, funcionario); // apenas atualização
                unitOfWork.Commit();

                var mensagem = ativar ? "Funcionário ativado com sucesso." : "Funcionário desativado com sucesso.";

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
