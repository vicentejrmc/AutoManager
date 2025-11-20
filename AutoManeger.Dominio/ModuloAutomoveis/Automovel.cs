using AutoManager.Dominio.Compartilhado;
using AutoManager.Dominio.ModuloGrupoAutomovel;

namespace AutoManager.Dominio.ModuloAutomoveis;

public class Automovel : EntidadeBase<Automovel>
{
    //propriedades
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

    //construtores
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
        GrupoAutomovel grupoAutomovel)
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
    }

    //Métodos
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
    }

    public void EditarDados(
        string placa,
        string marca,
        string modelo,
        string cor,
        string tipoCombustivel,
        int capacidadeCombustivel,
        int ano, string fotoUrl,
        Guid grupoAutomovelId,
        bool aluguelEmAberto
    )
    {
        if(aluguelEmAberto)
            throw new InvalidOperationException("Não é possível editar um automóvel com aluguel em aberto.");

        Placa = placa;
        Marca = marca;
        Modelo = modelo;
        Cor = cor;
        TipoCombustivel = tipoCombustivel;
        CapacidadeCombustivel = capacidadeCombustivel;
        Ano = ano;
        FotoUrl = fotoUrl;
        GrupoAutomovelId = grupoAutomovelId;
    }

    public void ExcluirAutomovel(bool aluguelEmAberto)
    {
        if(aluguelEmAberto)
            throw new InvalidOperationException("Não é possível excluir um automóvel com aluguel em aberto.");
        // logica de exclusão será implementada no Service Esse é um Método de validação extra da propria entidade
    }

    public string VisualizarResumoAutomovel()
    {
        return $"Id: {Id}, Placa: {Placa}, Marca: {Marca}, Modelo: {Modelo}, Cor: {Cor}, Combustível: {TipoCombustivel}";
    }


}
