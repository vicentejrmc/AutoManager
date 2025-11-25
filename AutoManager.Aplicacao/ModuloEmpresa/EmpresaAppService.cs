using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.ModuloEmpresa;
using AutoManager.Infraestrutura.Orm.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace AutoManager.Aplicacao.ModuloEmpresa
{
    public class EmpresaAppService : IAppService<Empresa>
    {
        private readonly AutoManagerDbContext dbContext;
        private readonly IPasswordHasher passwordHasher;
        private readonly ValidadorEmpresa validador;

        public EmpresaAppService(
            AutoManagerDbContext dbContext,
            IPasswordHasher passwordHasher,
            ValidadorEmpresa validador)
        {
            this.dbContext = dbContext;
            this.passwordHasher = passwordHasher;
            this.validador = validador;
        }

        public Result<Empresa> Inserir(Empresa entidade)
        {
            var resultadoValidacao = validador.Validar(entidade);
            if (resultadoValidacao.Falha)
                return Result<Empresa>.Fail(resultadoValidacao.Mensagem);

            if (dbContext.Empresas.Any(e => e.Email == entidade.Email))
                return Result<Empresa>.Fail(ErrorResults.RegistroDuplicado($"Empresa com e-mail {entidade.Email}"));

            entidade.Id = Guid.NewGuid();
            entidade.SenhaHash = passwordHasher.SenhaHash(entidade.SenhaHash);
            entidade.AspNetUserId = Guid.NewGuid().ToString();

            dbContext.Empresas.Add(entidade);
            dbContext.SaveChanges();

            return Result<Empresa>.Ok(entidade, "Empresa registrada com sucesso.");
        }

        public Result<Empresa> Editar(Empresa entidade)
        {
            var empresa = dbContext.Empresas.FirstOrDefault(e => e.Id == entidade.Id);
            if (empresa == null)
                return Result<Empresa>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.Id));

            var resultadoValidacao = validador.Validar(entidade);
            if (resultadoValidacao.Falha)
                return Result<Empresa>.Fail(resultadoValidacao.Mensagem);

            empresa.AtualizarRegistro(entidade);

            if (!string.IsNullOrWhiteSpace(entidade.SenhaHash))
                empresa.SenhaHash = passwordHasher.SenhaHash(entidade.SenhaHash);

            dbContext.Empresas.Update(empresa);
            dbContext.SaveChanges();

            return Result<Empresa>.Ok(empresa, "Empresa atualizada com sucesso.");
        }

        public Result Excluir(Guid id)
        {
            var empresa = dbContext.Empresas
                .Include(e => e.Alugueis)
                .FirstOrDefault(e => e.Id == id);

            if (empresa == null)
                return Result.Fail(ErrorResults.RegistroNaoEncontrado(id));

            if (empresa.Alugueis.Any(a => a.Ativo))
                return Result.Fail(ErrorResults.ExclusaoBloqueada("Empresa possui aluguéis ativos."));

            dbContext.Empresas.Remove(empresa);
            dbContext.SaveChanges();

            return Result.Ok("Empresa removida com sucesso.");
        }

        public Result<Empresa> SelecionarPorId(Guid id)
        {
            var empresa = dbContext.Empresas
                .Include(e => e.Funcionarios)
                .Include(e => e.Clientes)
                .FirstOrDefault(e => e.Id == id);

            if (empresa == null)
                return Result<Empresa>.Fail(ErrorResults.RegistroNaoEncontrado(id));

            return Result<Empresa>.Ok(empresa);
        }

        public List<Empresa> SelecionarTodos()
        {
            return dbContext.Empresas
                .Include(e => e.Funcionarios)
                .Include(e => e.Automoveis)
                .ToList();
        }
    }
}
