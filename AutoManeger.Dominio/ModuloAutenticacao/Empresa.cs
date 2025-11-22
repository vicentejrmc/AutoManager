using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloAluguel;
using AutoManager.Dominio.ModuloAutomoveis;
using AutoManager.Dominio.ModuloCliente;
using AutoManager.Dominio.ModuloCondutor;
using AutoManager.Dominio.ModuloFuncionario;

namespace AutoManager.Dominio.ModuloEmpresa;

public class Empresa : EntidadeBase<Empresa>
{
    public string Usuario { get; set; }
    public string Email { get; set; }
    public string? SenhadHash { get; set; } 
    public string AspNetUserId { get; set; }
    public ICollection<Funcionario> Funcionarios { get; set; } = new List<Funcionario>();
    public ICollection<Aluguel> Alugueis { get; set; } = new List<Aluguel>();
    public ICollection<Automovel> Automoveis { get; set; } = new List<Automovel>();
    public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
    public ICollection<Condutor> Condutores { get; set; } = new List<Condutor>();


    public Empresa() {}

    public Empresa(
        string usuario,
        string email,
        string? senhadHash,
        string aspNetUserId,
        ICollection<Funcionario> funcionarios,
        ICollection<Aluguel> alugueis,
        ICollection<Automovel> automoveis,
        ICollection<Cliente> clientes,
        ICollection<Condutor> condutores
    )
    {
        Usuario = usuario;
        Email = email;
        SenhadHash = senhadHash;
        AspNetUserId = aspNetUserId;
        Funcionarios = funcionarios;
        Alugueis = alugueis;
        Automoveis = automoveis;
        Clientes = clientes;
        Condutores = condutores;
    }

    public override void AtualizarRegistro(Empresa registroAtualizado)
    {
        Usuario = registroAtualizado.Usuario;
        Email = registroAtualizado.Email;
        SenhadHash = registroAtualizado.SenhadHash;
        AspNetUserId = registroAtualizado.AspNetUserId;
    }
}
