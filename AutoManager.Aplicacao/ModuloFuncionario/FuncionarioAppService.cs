using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.ModuloFuncionario;
using AutoManager.Dominio.ModuloEmpresa;
using AutoManager.Infraestrutura.Orm.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace AutoManager.Aplicacao.ModuloFuncionario
{
    public class FuncionarioAppService : IAppService<Funcionario>
    {
        private readonly AutoManagerDbContext dbContext;
        private readonly IPasswordHasher passwordHasher;
        private readonly FuncionarioValidador validador;

        public FuncionarioAppService(
            AutoManagerDbContext dbContext,
            IPasswordHasher passwordHasher,
            FuncionarioValidador validador)
        {
            this.dbContext = dbContext;
            this.passwordHasher = passwordHasher;
            this.validador = validador;
        }

        public Result<Funcionario> Inserir(Funcionario entidade)
        {
            var resultadoValidacao = validador.Validar(entidade);
            if (resultadoValidacao.Falha)
                return Result<Funcionario>.Fail(resultadoValidacao.Mensagem);

            var empresa = dbContext.Empresas.FirstOrDefault(e => e.Id == entidade.EmpresaId);
            if (empresa == null)
                return Result<Funcionario>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.EmpresaId));

            if (dbContext.Funcionarios.Any(f => f.Email == entidade.Email && f.EmpresaId == entidade.EmpresaId))
                return Result<Funcionario>.Fail(ErrorResults.RegistroDuplicado($"Já existe um funcionário com e-mail {entidade.Email} nesta empresa."));

            entidade.Id = Guid.NewGuid();
            entidade.SenhaHash = passwordHasher.SenhaHash(entidade.SenhaHash);
            entidade.AspNetUserId = Guid.NewGuid().ToString();
            entidade.EstaAtivo = true;
            entidade.Empresa = empresa;

            dbContext.Funcionarios.Add(entidade);
            dbContext.SaveChanges();

            return Result<Funcionario>.Ok(entidade, "Funcionário registrado com sucesso.");
        }

        public Result<Funcionario> Editar(Funcionario entidade)
        {
            var funcionario = dbContext.Funcionarios.FirstOrDefault(f => f.Id == entidade.Id);
            if (funcionario == null)
                return Result<Funcionario>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.Id));

            var resultadoValidacao = validador.Validar(entidade);
            if (resultadoValidacao.Falha)
                return Result<Funcionario>.Fail(resultadoValidacao.Mensagem);

            var empresa = dbContext.Empresas.FirstOrDefault(e => e.Id == entidade.EmpresaId);
            if (empresa == null)
                return Result<Funcionario>.Fail(ErrorResults.RegistroNaoEncontrado(entidade.EmpresaId));

            funcionario.AtualizarRegistro(entidade);
            funcionario.Empresa = empresa;

            if (!string.IsNullOrWhiteSpace(entidade.SenhaHash))
                funcionario.SenhaHash = passwordHasher.SenhaHash(entidade.SenhaHash);

            dbContext.Funcionarios.Update(funcionario);
            dbContext.SaveChanges();

            return Result<Funcionario>.Ok(funcionario, "Funcionário atualizado com sucesso.");
        }

        public Result Excluir(Guid id)
        {
            var funcionario = dbContext.Funcionarios.FirstOrDefault(f => f.Id == id);
            if (funcionario == null)
                return Result.Fail(ErrorResults.RegistroNaoEncontrado(id));

            if (!funcionario.EstaAtivo)
                return Result.Fail(ErrorResults.ExclusaoBloqueada("Funcionário já está inativo."));

            funcionario.EstaAtivo = false;
            dbContext.Funcionarios.Update(funcionario);
            dbContext.SaveChanges();

            return Result.Ok("Funcionário desativado com sucesso.");
        }

        public Result<Funcionario> SelecionarPorId(Guid id)
        {
            var funcionario = dbContext.Funcionarios
                .Include(f => f.Empresa)
                .FirstOrDefault(f => f.Id == id);

            if (funcionario == null)
                return Result<Funcionario>.Fail(ErrorResults.RegistroNaoEncontrado(id));

            return Result<Funcionario>.Ok(funcionario);
        }

        public List<Funcionario> SelecionarTodos()
        {
            return dbContext.Funcionarios
                .Include(f => f.Empresa)
                .ToList();
        }
    }
}
