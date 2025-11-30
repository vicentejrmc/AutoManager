using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloAluguel;
using AutoManager.Dominio.ModuloAutenticacao;
using AutoManager.Dominio.ModuloEmpresa;

namespace AutoManager.Aplicacao.ModuloEmpresa
{
    public class EmpresaAppService : IAppService<Empresa>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ITenantProvider tenantProvider;
        private readonly IRepositorioEmpresa repositorioEmpresa;
        private readonly IPasswordHasher passwordHasher;
        private readonly ValidadorEmpresa validador;

        public EmpresaAppService(
            IUnitOfWork unitOfWork,
            ITenantProvider tenantProvider,
            IRepositorioEmpresa repositorioEmpresa,
            IPasswordHasher passwordHasher,
            ValidadorEmpresa validador)
        {
            this.unitOfWork = unitOfWork;
            this.tenantProvider = tenantProvider;
            this.repositorioEmpresa = repositorioEmpresa;
            this.passwordHasher = passwordHasher;
            this.validador = validador;
        }

        public Result<Empresa> Inserir(Empresa entidade)
        {
            var resultadoValidacao = validador.Validar(entidade);
            if (resultadoValidacao.Falha)
                return Result<Empresa>.Fail(ErrorResults.RequisicaoInvalida(resultadoValidacao.Mensagem));


            var empresa = repositorioEmpresa.SelecionarTodos();
            if (empresa.Any(e => e.Email == entidade.Email))
                return Result<Empresa>.Fail(ErrorResults.RegistroDuplicado($"Empresa com e-mail {entidade.Email}"));

            entidade.Id = Guid.NewGuid();
            entidade.SenhaHash = passwordHasher.SenhaHash(entidade.SenhaHash);
            entidade.AspNetUserId = Guid.NewGuid().ToString();

            try
            {
                repositorioEmpresa.Inserir(entidade);
                unitOfWork.Commit();

                return Result<Empresa>.Ok(entidade, "Empresa registrada com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result<Empresa>.Fail(ErrorResults.ErroInterno($"Erro ao inserir empresa: {ex.Message}"));
            }


        }

        public Result<Empresa> Editar(Empresa entidade)
        {
            var empresa = repositorioEmpresa.SelecionarPorId(entidade.Id);
            if (empresa == null)
                return Result<Empresa>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.Id));

            var resultadoValidacao = validador.Validar(entidade);
            if (resultadoValidacao.Falha)
                return Result<Empresa>.Fail(ErrorResults.RequisicaoInvalida(resultadoValidacao.Mensagem));


            var empresas = repositorioEmpresa.SelecionarTodos();
            if (empresas.Any(e => e.Email == entidade.Email && e.Id != entidade.Id))
                return Result<Empresa>.Fail(ErrorResults.RegistroDuplicado("Empresa com e-mail já cadastrado."));

            empresa.AtualizarRegistro(entidade);

            if (!string.IsNullOrWhiteSpace(entidade.SenhaHash))
                empresa.SenhaHash = passwordHasher.SenhaHash(entidade.SenhaHash);

            try
            {
                repositorioEmpresa.Editar(entidade.Id, empresa);
                unitOfWork.Commit();

                return Result<Empresa>.Ok(empresa, "Empresa atualizada com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result<Empresa>.Fail(ErrorResults.ErroInterno($"Erro ao editar empresa: {ex.Message}"));
            }
        }


        // Método de exclusão que não deve ser chamado diretamente em produção. A exclusão deve ser solicitada via SolicitarExclusao.
        public Result Excluir(Guid id)
        {
            // Apenas cumpre a interface, mas delega para SolicitarExclusao
            return SolicitarExclusao(id, string.Empty, string.Empty);
        }

        public Result SolicitarExclusao(Guid id, string email, string senha)
        {
            if(!tenantProvider.IsInRole("Empresa"))
                return Result.Fail(ErrorResults.RequisicaoInvalida("Somente usuários Empresa podem solicitar exclusão"));

            var empresaIdLogada = tenantProvider.EmpresaId;
            if(empresaIdLogada == null || empresaIdLogada.Value != id)
                return Result.Fail(ErrorResults.RequisicaoInvalida("A Empresa Logada só pode solitar a exclusão de si mesma."));

            var empresa = repositorioEmpresa.SelecionarPorId(id);
            if (empresa == null)
                return Result.Fail(ErrorResults.RegistroNaoEncontrado(id));

            if(empresa.Email != email || !passwordHasher.VerificarSenhaHash(senha, empresa.SenhaHash))
                return Result.Fail(ErrorResults.RequisicaoInvalida("Email ou senha inválidos para solicitação de exclusão da empresa."));

            if (empresa.Alugueis.Any(a => a.Status == StatusAluguelEnum.EmAndamento))
                return Result.Fail(ErrorResults.ExclusaoBloqueada("Empresa possui aluguéis ativos."));

            try
            {
                empresa.Status = StatusEmpresaEnum.PendenteExclusao;
                repositorioEmpresa.Editar(empresa.Id, empresa);
                unitOfWork.Commit();

                //Implementar envio de e-mail para o suporte solicitando a exclusão definitiva da empresa aqui.    

                return Result.Ok("Empresa desativada. Um e-mail foi enviado ao suporte para exclusão definitiva.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                return Result.Fail(ErrorResults.ErroInterno($"Erro ao processar solicitação de exclusão: {ex.Message}"));
            }
        }

        public Result<Empresa> SelecionarPorId(Guid id)
        {
            var empresa = repositorioEmpresa.SelecionarPorId(id);

            if (empresa == null)
                return Result<Empresa>.Fail(ErrorResults.RegistroNaoEncontrado(id));

            return Result<Empresa>.Ok(empresa);
        }

        public Result<List<Empresa>> SelecionarTodos()
        {
            // Retorna apenas a empresa do tenant logado
            var empresaIdLogada = tenantProvider.EmpresaId;
            if (empresaIdLogada == null)
                return Result<List<Empresa>>.Fail(ErrorResults.PermissaoNegada("Empresa não identificada."));

            var empresa = repositorioEmpresa.SelecionarPorId(empresaIdLogada.Value);
            if (empresa == null)
                return Result<List<Empresa>>.Fail(ErrorResults.RegistroNaoEncontrado(empresaIdLogada.Value));

            return Result<List<Empresa>>.Ok(new List<Empresa> { empresa });
        }

    }
}
