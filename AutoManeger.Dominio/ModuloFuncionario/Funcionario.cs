using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloEmpresa;
using System;

namespace AutoManager.Dominio.ModuloFuncionario;

public class Funcionario : EntidadeBase<Funcionario>
{

    public string Nome { get; set; }
    public DateTime DataAdmissao { get; set; }
    public decimal Salario { get; set; }
    public Guid EmpresaId { get; set; }
    public Empresa Empresa { get; set; }
    public bool EstaAtivo { get; set; }

    public Funcionario() {}

    public Funcionario(
        string nome,
        DateTime dataAdmissao,
        decimal salario,
        Guid empresaId,
        Empresa empresa,
        bool estaAtivo
        )
    {
        Nome = nome;
        DataAdmissao = dataAdmissao;
        Salario = salario;
        EmpresaId = empresaId;
        Empresa = empresa;
        EstaAtivo = estaAtivo;
    }

    public override void AtualizarRegistro(Funcionario registroAtualizado)
    {
        Nome = registroAtualizado.Nome;
        DataAdmissao = registroAtualizado.DataAdmissao;
        Salario = registroAtualizado.Salario;
        EmpresaId = registroAtualizado.EmpresaId;
        Empresa = registroAtualizado.Empresa;
        EstaAtivo = registroAtualizado.EstaAtivo;
    }
}
