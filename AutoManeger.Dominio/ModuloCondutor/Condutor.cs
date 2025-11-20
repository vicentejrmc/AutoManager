using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloCliente;

namespace AutoManager.Dominio.ModuloCondutor;

public class Condutor : EntidadeBase<Condutor>
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public string CPF { get; set; }
    public string CNH { get; set; }
    public DateTime ValidadeCNH { get; set; }
    public string Telefone { get; set; }
    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; }

    public override void AtualizarRegistro(Condutor registroAtualizado)
    {
        Nome = registroAtualizado.Nome;
        Email = registroAtualizado.Email;
        CPF = registroAtualizado.CPF;
        CNH = registroAtualizado.CNH;
        ValidadeCNH = registroAtualizado.ValidadeCNH;
        Telefone = registroAtualizado.Telefone;
    }
}
