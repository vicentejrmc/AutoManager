using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloAutenticacao;
using AutoManager.Dominio.ModuloCliente;
using AutoManager.Dominio.ModuloEmpresa;

namespace AutoManager.Aplicacao.ModuloCliente
{
    public class ClienteAppService : IAppService<Cliente>
    {
        private readonly ITenantProvider tenantProvider;
        private readonly IUnitOfWork unitOfWork;
        private readonly IRepositorioCliente repositorioCliente;
        private readonly IRepositorioEmpresa repositorioEmpresa;
        private readonly ValidadorCliente validadorCliente;

        public ClienteAppService(
            ITenantProvider tenantProvider,
            IUnitOfWork unitOfWork,
            IRepositorioCliente repositorioCliente,
            IRepositorioEmpresa repositorioEmpresa,
            ValidadorCliente validadorCliente
        )
        {
            this.tenantProvider = tenantProvider;
            this.unitOfWork = unitOfWork;
            this.repositorioCliente = repositorioCliente;
            this.repositorioEmpresa = repositorioEmpresa;
            this.validadorCliente = validadorCliente;
        }

        public Result<Cliente> Inserir(Cliente entidade)
        {
            //validação em Profundidade
            if (!tenantProvider.IsInRole("Empresa") && !tenantProvider.IsInRole("Funcionario"))
                return Result<Cliente>.Fail(ErrorResults.PermissaoNegada("Somente Empresa ou Funcionário podem cadastrar clientes."));

            var empresaIdLogada = tenantProvider.EmpresaId;
            if (empresaIdLogada == null)
                return Result<Cliente>.Fail(ErrorResults.PermissaoNegada("Empresa não identificada."));

            var empresa = repositorioEmpresa.SelecionarPorId(empresaIdLogada.Value);
            if (empresa == null)
                return Result<Cliente>.Fail(ErrorResults.RegistroNaoEncontrado(empresaIdLogada.Value));

            entidade.EmpresaId = empresa.Id;
            entidade.Empresa = empresa;

            var resultadoValidacao = validadorCliente.Validar(entidade);
            if (resultadoValidacao.Falha)
                return Result<Cliente>.Fail(ErrorResults.RequisicaoInvalida(resultadoValidacao.Mensagem));

            try
            {
                repositorioCliente.Inserir(entidade);
                unitOfWork.Commit();

                return Result<Cliente>.Ok(entidade, "Cliente registrado com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result<Cliente>.Fail(ErrorResults.ErroInterno($"Erro ao inserir cliente: {ex.Message}"));
            }
        }

        public Result<Cliente> Editar(Cliente entidade)
        {
            if (!tenantProvider.IsInRole("Empresa") && !tenantProvider.IsInRole("Funcionario"))
                return Result<Cliente>.Fail(ErrorResults.PermissaoNegada("Somente Empresa ou Funcionário podem editar clientes."));

            var cliente = repositorioCliente.SelecionarPorId(entidade.Id);
            if (cliente == null)
                return Result<Cliente>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.Id));

            var empresaIdLogada = tenantProvider.EmpresaId;
            if (empresaIdLogada == null)
                return Result<Cliente>.Fail(ErrorResults.PermissaoNegada("Empresa não identificada."));

            if (cliente.EmpresaId != empresaIdLogada.Value)
                return Result<Cliente>.Fail(ErrorResults.PermissaoNegada("Não é possível editar clientes de outra empresa."));

            var empresa = repositorioEmpresa.SelecionarPorId(empresaIdLogada.Value);
            if (empresa == null)
                return Result<Cliente>.Fail(ErrorResults.RegistroNaoEncontrado(empresaIdLogada.Value));

            entidade.EmpresaId = empresa.Id;
            entidade.Empresa = empresa;

            var resultadoValidacao = validadorCliente.Validar(entidade);
            if (resultadoValidacao.Falha)
                return Result<Cliente>.Fail(resultadoValidacao.Mensagem);

            try
            {
                cliente.AtualizarRegistro(entidade);
                cliente.Empresa = empresa;

                repositorioCliente.Editar(entidade.Id, cliente);
                unitOfWork.Commit();

                return Result<Cliente>.Ok(cliente, "Cliente atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result<Cliente>.Fail(ErrorResults.ErroInterno($"Erro ao editar cliente: {ex.Message}"));
            }
        }

        public Result Excluir(Guid id)
        {
            var cliente = repositorioCliente.SelecionarPorId(id);
            if (cliente == null)
                return Result.Fail(ErrorResults.RegistroNaoEncontrado(id));

            if (!tenantProvider.IsInRole("Empresa"))
                return Result.Fail(ErrorResults.PermissaoNegada("Somente usuários Empresa podem excluir clientes."));

            var empresaIdLogada = tenantProvider.EmpresaId;
            if (empresaIdLogada == null || cliente.EmpresaId != empresaIdLogada.Value)
                return Result.Fail(ErrorResults.PermissaoNegada("Não é possível excluir clientes de outra empresa."));

            try
            {
                repositorioCliente.Excluir(id);
                unitOfWork.Commit();

                return Result.Ok("Cliente excluído com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result.Fail(ErrorResults.ErroInterno($"Erro ao excluir cliente: {ex.Message}"));
            }
        }

        public Result<Cliente> SelecionarPorId(Guid id)
        {
            if (!tenantProvider.IsInRole("Empresa") && !tenantProvider.IsInRole("Funcionario"))
                return Result<Cliente>.Fail(ErrorResults.PermissaoNegada("Somente Empresa ou Funcionário podem consultar clientes."));

            var cliente = repositorioCliente.SelecionarPorId(id);
            if (cliente == null)
                return Result<Cliente>.Fail(ErrorResults.RegistroNaoEncontrado(id));

            return Result<Cliente>.Ok(cliente);
        }

        public Result<List<Cliente>> SelecionarTodos()
        {
            if (!tenantProvider.IsInRole("Empresa") && !tenantProvider.IsInRole("Funcionario"))
                return Result<List<Cliente>>.Fail(ErrorResults.PermissaoNegada("Somente Empresa ou Funcionário podem consultar clientes."));

            var clientes = repositorioCliente.SelecionarTodos();
            return Result<List<Cliente>>.Ok(clientes);
        }
    }
}
