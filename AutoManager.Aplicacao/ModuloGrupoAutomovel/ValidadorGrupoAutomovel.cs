using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.ModuloGrupoAutomovel;

namespace AutoManager.Aplicacao.ModuloGrupoAutomovel
{
    public class ValidadorGrupoAutomovel : ValidadorBase<GrupoAutomovel>
    {
        public override Result<GrupoAutomovel> Validar(GrupoAutomovel entidade)
        {
            if (string.IsNullOrWhiteSpace(entidade.Nome))
                return Result<GrupoAutomovel>.Fail(ErrorResults.RequisicaoInvalida("O nome do grupo de automóvel é obrigatório."));

            if (entidade.EmpresaId == Guid.Empty)
                return Result<GrupoAutomovel>.Fail(ErrorResults.RequisicaoInvalida("O grupo de automóvel deve estar vinculado a uma empresa."));

            return Result<GrupoAutomovel>.Ok(entidade);
        }
    }
}
