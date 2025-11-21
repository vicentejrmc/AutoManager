using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloEmpresa;
using AutoManager.Dominio.ModuloGrupoAutomovel;

namespace AutoManager.Dominio.ModuloAutomoveis;

public class Automovel : EntidadeBase<Automovel>
{
    public string Placa { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public string Cor { get; set; }
    public string TipoCombustivel { get; set; }
    public int CapacidadeCombustivel { get; set; }
    public int Ano { get; set; }
    public string FotoUrl { get; set; }
    public Guid GrupoAutomovelId { get; set; }
    public GrupoAutomovel GrupoAutomovel { get; set; }
    public Guid EmpresaId { get; set; }
    public Empresa Empresa { get; set; }

    public Automovel(){}

    public Automovel(
        string placa,
        string marca,
        string modelo, 
        string cor,
        string tipoCombustivel,
        int capacidadeCombustivel,
        int ano,
        string fotoUrl,
        Guid grupoAutomovelId,
        GrupoAutomovel grupoAutomovel,
        Guid empresaId
        )
    {
        Placa = placa;
        Marca = marca;
        Modelo = modelo;
        Cor = cor;
        TipoCombustivel = tipoCombustivel;
        CapacidadeCombustivel = capacidadeCombustivel;
        Ano = ano;
        FotoUrl = fotoUrl;
        GrupoAutomovelId = grupoAutomovelId;
        GrupoAutomovel = grupoAutomovel;
        EmpresaId = empresaId;
    }

    public override void AtualizarRegistro(Automovel registroAtualizado)
    {
        Placa = registroAtualizado.Placa;
        Marca = registroAtualizado.Marca;
        Modelo = registroAtualizado.Modelo;
        Cor = registroAtualizado.Cor;
        TipoCombustivel = registroAtualizado.TipoCombustivel;
        CapacidadeCombustivel = registroAtualizado.CapacidadeCombustivel;
        Ano = registroAtualizado.Ano;
        FotoUrl = registroAtualizado.FotoUrl;
        GrupoAutomovelId = registroAtualizado.GrupoAutomovelId;
        EmpresaId = registroAtualizado.EmpresaId;
    }
}
