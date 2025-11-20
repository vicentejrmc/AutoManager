using AutoManager.Dominio.Compartilhado;

namespace AutoManeger.Dominio.ModuloFuncionario;

public class Funcionario : EntidadeBase<Funcionario>
{

    public string Nome { get; set; }
    public DateTime DataAdmissao { get; set; }
    public decimal Salario { get; set; }
    public int EmpresaId { get; set; }
    public bool EstaAtivo { get; set; }

    public Funcionario() {}

    public Funcionario(string nome, DateTime dataAdmissao, decimal salario, int empresaId, bool estaAtivo)
    {
        Nome = nome;
        DataAdmissao = dataAdmissao;
        Salario = salario;
        EmpresaId = empresaId;
        EstaAtivo = estaAtivo;
    }

    public override void AtualizarRegistro(Funcionario registroAtualizado)
    {
        Nome = registroAtualizado.Nome;
        DataAdmissao = registroAtualizado.DataAdmissao;
        Salario = registroAtualizado.Salario;
        EmpresaId = registroAtualizado.EmpresaId;
        EstaAtivo = registroAtualizado.EstaAtivo;
    }
    
    public void EditarDados(string nome, DateTime dataAdmissao, decimal salario)
    {
        Nome = nome;
        DataAdmissao = dataAdmissao;
        Salario = salario;
    }

    public void EditarPropriosDados(string nome)
    {
        Nome = nome;
    }

    public void Desativar()
    {
        EstaAtivo = false;
    }

    public void Ativar()
    {
        EstaAtivo = true;
    }

}
