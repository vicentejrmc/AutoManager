using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloCondutor;
using AutoManager.Dominio.ModuloEmpresa;


namespace AutoManager.Dominio.ModuloCliente;

public abstract class Cliente : EntidadeBase<Cliente>
{
    public string Nome { get; set; }
    public string Telefone { get; set; }
    public Guid EmpresaId { get; set; }
    public Empresa Empresa { get; set; }
    public ICollection<Condutor> Condutores { get; set; } = new List<Condutor>();

    public abstract override void AtualizarRegistro(Cliente registroAtualizado);
}

// Pessoa Física
public class PessoaFisica : Cliente
{
    public string CPF { get; set; }
    public string RG { get; set; }
    public string CNH { get; set; }

    public PessoaFisica() { }

    public PessoaFisica(string nome, string telefone, string cpf, string rg, string cnh, Guid empresaId, Empresa empresa)
    {
        Nome = nome;
        Telefone = telefone;
        CPF = cpf;
        RG = rg;
        CNH = cnh;
        EmpresaId = empresaId;
        Empresa = empresa;
    }

    public override void AtualizarRegistro(Cliente registroAtualizado)
    {
        if (registroAtualizado is PessoaFisica pf)
        {
            Nome = pf.Nome;
            Telefone = pf.Telefone;
            CPF = pf.CPF;
            RG = pf.RG;
            CNH = pf.CNH;
            EmpresaId = pf.EmpresaId;
            Empresa = pf.Empresa;
        }
    }
}

// Pessoa Jurídica
public class PessoaJuridica : Cliente
{
    public string CNPJ { get; set; }

    public PessoaJuridica() { }

    public PessoaJuridica(string nome, string telefone, string cnpj, Guid empresaId, Empresa empresa)
    {
        Nome = nome;
        Telefone = telefone;
        CNPJ = cnpj;
        EmpresaId = empresaId;
        Empresa = empresa;
    }

    public override void AtualizarRegistro(Cliente registroAtualizado)
    {
        if (registroAtualizado is PessoaJuridica pj)
        {
            Nome = pj.Nome;
            Telefone = pj.Telefone;
            CNPJ = pj.CNPJ;
            EmpresaId = pj.EmpresaId;
            Empresa = pj.Empresa;
        }
    }
}

