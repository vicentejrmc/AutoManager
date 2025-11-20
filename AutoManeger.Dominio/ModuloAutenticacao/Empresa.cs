using AutoManeger.Dominio.Compartilhado;
using AutoManeger.Dominio.ModuloFuncionario;

namespace AutoManeger.Dominio.ModuloEmpresa;

public class Empresa : EntidadeBase<Empresa>
{
    public string Usuario { get; set; }
    public string Senha { get; set; }
    public string Email { get; set; }
    public string NomeFantasia { get; set; }
    public ICollection<Funcionario> Funcionarios { get; set; } = new List<Funcionario>();

    public Empresa() {}

    public override void AtualizarRegistro(Empresa registroAtualizado)
    {
        Usuario = registroAtualizado.Usuario;
        Senha = registroAtualizado.Senha;
        Email = registroAtualizado.Email;
        NomeFantasia = registroAtualizado.NomeFantasia;
    }

    public void RegistrarFuncionario( Funcionario funcionario )
        => Funcionarios.Add( funcionario );

    public void AtivarFuncionario( Funcionario funcionario )
        => funcionario.Ativar();

    public void DesativarFuncionario( Funcionario funcionario)
        => funcionario.Desativar();

    public bool Autenticar(string usuario, string senha)
        => Usuario == usuario && Senha == senha;

    public void AlterarSenha(string novaSenha) // Service deverá comprar a senha atual antes de alterar
        => Senha = novaSenha;
}
