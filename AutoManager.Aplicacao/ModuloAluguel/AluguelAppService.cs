using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.ModuloAluguel;
using AutoManager.Dominio.ModuloAutenticacao;
using AutoManager.Infraestrutura.Orm.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace AutoManager.Aplicacao.ModuloAluguel
{
    public class AluguelAppService : IAppService<Aluguel>
    {
        private readonly AutoManagerDbContext dbContext;
        private readonly ValidadorAluguel validador;
        private readonly ITenantProvider tenantProvider;

        public AluguelAppService(
            AutoManagerDbContext dbContext,
            ValidadorAluguel validador,
            ITenantProvider tenantProvider)
        {
            this.dbContext = dbContext;
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

            dbContext.Alugueis.Add(entidade);
            dbContext.SaveChanges();

            return Result<Aluguel>.Ok(entidade, "Aluguel registrado com sucesso.");
        }

        public Result<Aluguel> Editar(Aluguel entidade)
        {
            var aluguel = dbContext.Alugueis.FirstOrDefault(a => a.Id == entidade.Id);
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

            dbContext.Alugueis.Update(aluguel);
            dbContext.SaveChanges();

            return Result<Aluguel>.Ok(aluguel, "Aluguel atualizado com sucesso.");
        }

        public Result Excluir(Guid id)
        {
            var aluguel = dbContext.Alugueis.FirstOrDefault(a => a.Id == id);
            if (aluguel == null)
                return Result.Fail(ErrorResults.RegistroNaoEncontrado(id));

            if (!aluguel.Ativo)
                return Result.Fail(ErrorResults.ExclusaoBloqueada("Aluguel já está inativo."));

            aluguel.Ativo = false; //desativar em vez de excluir fisicamente
            dbContext.Alugueis.Update(aluguel);
            dbContext.SaveChanges();

            return Result.Ok("Aluguel desativado com sucesso.");
        }

        public Result<Aluguel> SelecionarPorId(Guid id)
        {
            var aluguel = dbContext.Alugueis
                .Include(a => a.Empresa)
                .Include(a => a.Automovel)
                .Include(a => a.Condutor)
                .Include(a => a.PlanoDeCobranca)
                .Include(a => a.Taxas)
                .FirstOrDefault(a => a.Id == id);

            if (aluguel == null)
                return Result<Aluguel>.Fail(ErrorResults.RegistroNaoEncontrado(id));

            return Result<Aluguel>.Ok(aluguel);
        }

        public List<Aluguel> SelecionarTodos()
        {
            return dbContext.Alugueis
                .Include(a => a.Empresa)
                .Include(a => a.Automovel)
                .Include(a => a.Condutor)
                .Include(a => a.PlanoDeCobranca)
                .Include(a => a.Taxas)
                .ToList();
        }
    }
}

