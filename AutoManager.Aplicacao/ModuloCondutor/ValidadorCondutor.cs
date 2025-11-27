using AutoManager.Aplicacao.Compartilhado;
using AutoManager.Dominio.ModuloCondutor;

namespace AutoManager.Aplicacao.ModuloCondutor
{
    public class ValidadorCondutor : ValidadorBase<Condutor>
    {
        public override Result<Condutor> Validar(Condutor condutor)
        {
            if (string.IsNullOrWhiteSpace(condutor.Nome))
                return Result<Condutor>.Fail(ErrorResults.RequisicaoInvalida("O nome do condutor é obrigatório."));

            if (string.IsNullOrWhiteSpace(condutor.Email))
                return Result<Condutor>.Fail(ErrorResults.RequisicaoInvalida("O e-mail do condutor é obrigatório."));

            if (string.IsNullOrWhiteSpace(condutor.CPF))
                return Result<Condutor>.Fail(ErrorResults.RequisicaoInvalida("O CPF do condutor é obrigatório."));

            if (string.IsNullOrWhiteSpace(condutor.CNH))
                return Result<Condutor>.Fail(ErrorResults.RequisicaoInvalida("A CNH do condutor é obrigatória."));

            if (condutor.ValidadeCNH == default)
                return Result<Condutor>.Fail(ErrorResults.RequisicaoInvalida("A validade da CNH deve ser informada."));

            if (condutor.ValidadeCNH < DateTime.Today)
                return Result<Condutor>.Fail(ErrorResults.RequisicaoInvalida("A CNH do condutor está vencida."));

            if (string.IsNullOrWhiteSpace(condutor.Telefone))
                return Result<Condutor>.Fail(ErrorResults.RequisicaoInvalida("O telefone do condutor é obrigatório."));

            if (condutor.ClienteId == Guid.Empty)
                return Result<Condutor>.Fail(ErrorResults.RequisicaoInvalida("O condutor deve estar vinculado a um cliente."));

            if (condutor.EmpresaId == Guid.Empty)
                return Result<Condutor>.Fail(ErrorResults.RequisicaoInvalida("O condutor deve estar vinculado a uma empresa."));

            return Result<Condutor>.Ok(condutor);
        }
    }
}
