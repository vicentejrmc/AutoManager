using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloAluguel;
using AutoManager.Dominio.ModuloAutomoveis;
using AutoManager.Dominio.ModuloCliente;
using AutoManager.Dominio.ModuloCondutor;
using AutoManager.Dominio.ModuloFuncionario;
using AutoManager.Dominio.ModuloGrupoAutomovel;
using AutoManager.Dominio.ModuloPlanoCobranca;
using AutoManager.Dominio.ModuloPrecoCombustivel;
using AutoManager.Dominio.ModuloTaxaServico;

namespace AutoManager.Dominio.ModuloEmpresa;

public class Empresa : EntidadeBase<Empresa>
{
    public string Usuario { get; set; }
    public string Email { get; set; }
    public string SenhaHash { get; set; } 
    public string AspNetUserId { get; set; }
    public ICollection<Funcionario> Funcionarios { get; set; } = new List<Funcionario>();
    public ICollection<Aluguel> Alugueis { get; set; } = new List<Aluguel>();
    public ICollection<Automovel> Automoveis { get; set; } = new List<Automovel>();
    public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
    public ICollection<Condutor> Condutores { get; set; } = new List<Condutor>();
    public ICollection<GrupoAutomovel> GruposAutomoveis { get; set; } = new List<GrupoAutomovel>();
    public ICollection<PlanoCobranca> PlanosCobranca { get; set; } = new List<PlanoCobranca>();
    public ICollection<PrecoCombustivel> PrecoCombustiveis { get; set; } = new List<PrecoCombustivel>();
    public ICollection<TaxaServico> TaxasServico { get; set; } = new List<TaxaServico>();

    public Empresa() {}

    public Empresa(
        string usuario,
        string email,
        string senhadHash,
        string aspNetUserId,
        ICollection<Funcionario> funcionarios,
        ICollection<Aluguel> alugueis,
        ICollection<Automovel> automoveis,
        ICollection<Cliente> clientes,
        ICollection<Condutor> condutores,
        ICollection<GrupoAutomovel> gruposAutomoveis,
        ICollection<PlanoCobranca> planosCobranca,
        ICollection<PrecoCombustivel> precoCombustiveis,
        ICollection<TaxaServico> taxasServico
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
        GruposAutomoveis = gruposAutomoveis;
        PlanosCobranca = planosCobranca;
        PrecoCombustiveis = precoCombustiveis;
        TaxasServico = taxasServico;
    }

    public override void AtualizarRegistro(Empresa registroAtualizado)
    {
        Usuario = registroAtualizado.Usuario;
        Email = registroAtualizado.Email;
        SenhadHash = registroAtualizado.SenhadHash;
        AspNetUserId = registroAtualizado.AspNetUserId;
    }
}
