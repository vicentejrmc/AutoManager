using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.ModuloAutomoveis;

namespace AutoManager.Aplicacao.ModuloAutomoveis
{
    public class ValidadorAutomovel
    {
        public virtual Result<Automovel> Validar(Automovel automovel)
        {
            if (string.IsNullOrWhiteSpace(automovel.Placa))
                return Result<Automovel>.Fail(ErrorResults.RequisicaoInvalida("Placa do automóvel é obrigatória."));

            if (string.IsNullOrWhiteSpace(automovel.Marca))
                return Result<Automovel>.Fail(ErrorResults.RequisicaoInvalida("Marca do automóvel é obrigatória."));

            if (string.IsNullOrWhiteSpace(automovel.Modelo))
                return Result<Automovel>.Fail(ErrorResults.RequisicaoInvalida("Modelo do automóvel é obrigatório."));

            if (string.IsNullOrWhiteSpace(automovel.Cor))
                return Result<Automovel>.Fail(ErrorResults.RequisicaoInvalida("Cor do automóvel é obrigatória."));

            if (automovel.Ano <= 1900 || automovel.Ano > DateTime.Now.Year + 1)
                return Result<Automovel>.Fail(ErrorResults.RequisicaoInvalida("Ano do automóvel inválido."));

            if (automovel.CapacidadeCombustivel <= 0)
                return Result<Automovel>.Fail(ErrorResults.RequisicaoInvalida("Capacidade de combustível deve ser maior que zero."));

            if (automovel.EmpresaId == Guid.Empty)
                return Result<Automovel>.Fail(ErrorResults.RequisicaoInvalida("Automóvel deve estar vinculado a uma empresa."));

            if (automovel.GrupoAutomovelId == Guid.Empty)
                return Result<Automovel>.Fail(ErrorResults.RequisicaoInvalida("Automóvel deve estar vinculado a um grupo."));

            return Result<Automovel>.Ok(automovel);
        }
    }
}
