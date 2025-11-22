using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloAluguel;
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

    public Empresa() {}

    public Empresa(
        string usuario,
        string email,
        string? senhadHash,
        string aspNetUserId,
        ICollection<Funcionario> funcionarios,
        ICollection<Aluguel> alugueis
    )
    {
        Usuario = usuario;
        Email = email;
        SenhadHash = senhadHash;
        AspNetUserId = aspNetUserId;
        Funcionarios = funcionarios;
        Alugueis = alugueis;
    }

    public override void AtualizarRegistro(Empresa registroAtualizado)
    {
        Usuario = registroAtualizado.Usuario;
        Email = registroAtualizado.Email;
        SenhadHash = registroAtualizado.SenhadHash;
        AspNetUserId = registroAtualizado.AspNetUserId;
        Funcionarios = registroAtualizado.Funcionarios;
        Alugueis = registroAtualizado.Alugueis;
    }
}
