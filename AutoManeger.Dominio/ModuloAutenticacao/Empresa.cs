using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloAutenticacao;
using AutoManager.Dominio.ModuloFuncionario;

namespace AutoManager.Dominio.ModuloEmpresa;

public class Empresa : EntidadeBase<Empresa>
{
    public string Usuario { get; set; }
    public string Email { get; set; }
    public string? SenhadHash { get; set; }     
    public ICollection<Funcionario> Funcionarios { get; set; } = new List<Funcionario>();

    public Empresa() {}

    public Empresa(string usuario, string email, string? senhadHash, ICollection<Funcionario> funcionarios)
    {
        Usuario = usuario;
        Email = email;
        SenhadHash = senhadHash;
        Funcionarios = funcionarios;
    }

    public override void AtualizarRegistro(Empresa registroAtualizado)
    {
        Usuario = registroAtualizado.Usuario;
        Email = registroAtualizado.Email;
        SenhadHash = registroAtualizado.SenhadHash;
        Funcionarios = registroAtualizado.Funcionarios;
    }
}
