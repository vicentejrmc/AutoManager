using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloFuncionario;

namespace AutoManager.Dominio.ModuloEmpresa;

public class Empresa : EntidadeBase<Empresa>
{
    public string Usuario { get; set; }
    public string Email { get; set; }
    public string? SenhadHash { get; set; } 
    public string AspNetUserId { get; set; }
    public ICollection<Funcionario> Funcionarios { get; set; } = new List<Funcionario>();

    public Empresa() {}

    public Empresa(string usuario, string email, string? senhadHash, string aspNetUserId ,ICollection<Funcionario> funcionarios)
    {
        Usuario = usuario;
        Email = email;
        SenhadHash = senhadHash;
        AspNetUserId = aspNetUserId;
        Funcionarios = funcionarios;
    }

    public override void AtualizarRegistro(Empresa registroAtualizado)
    {
        Usuario = registroAtualizado.Usuario;
        Email = registroAtualizado.Email;
        SenhadHash = registroAtualizado.SenhadHash;
        AspNetUserId = registroAtualizado.AspNetUserId;
        Funcionarios = registroAtualizado.Funcionarios;
    }
}
