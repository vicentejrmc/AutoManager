using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloCliente;
using AutoManager.Dominio.ModuloEmpresa;

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
    public Guid EmpresaId { get; set; }
    public Empresa Empresa { get; set; }

    public Condutor() {}

    public Condutor(
        string nome,
        string email,
        string cPF,
        string cNH,
        DateTime validadeCNH,
        string telefone,
        Guid clienteId,
        Cliente cliente,
        Guid empresaId,
        Empresa empresa
    )
    {
        Nome = nome;
        Email = email;
        CPF = cPF;
        CNH = cNH;
        ValidadeCNH = validadeCNH;
        Telefone = telefone;
        ClienteId = clienteId;
        Cliente = cliente;
        EmpresaId = empresaId;
        Empresa = empresa;
    }

    public override void AtualizarRegistro(Condutor registroAtualizado)
    {
        Nome = registroAtualizado.Nome;
        Email = registroAtualizado.Email;
        CPF = registroAtualizado.CPF;
        CNH = registroAtualizado.CNH;
        ValidadeCNH = registroAtualizado.ValidadeCNH;
        Telefone = registroAtualizado.Telefone;
        ClienteId = registroAtualizado.ClienteId;
        Cliente = registroAtualizado.Cliente;
        EmpresaId = registroAtualizado.EmpresaId;
        Empresa = registroAtualizado.Empresa;
    }
}
