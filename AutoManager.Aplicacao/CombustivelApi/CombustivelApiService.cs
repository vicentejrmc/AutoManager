using AutoManager.Dominio.ModuloPrecoCombustivel;

namespace AutoManager.Aplicacao.CombustivelApi;

public class CombustivelApiService
{
    private readonly HttpClient _httpClient;

    public CombustivelApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PrecoCombustivel> ObterPrecoMedioAsync(string estado, string tipoCombustivel)
    {
        var response = await _httpClient.GetAsync($"https://combustivelapi.com.br/{estado}/{tipoCombustivel}");
        response.EnsureSuccessStatusCode();

        // var dto = await response.Content.ReadFromJsonAsync<PrecoCombustivelDto>();

        return new PrecoCombustivel
        {
            Estado = estado,
            TipoCombustivel = tipoCombustivel,
            //PrecoMedio = dto.PrecoMedio,
            DataAtualizacao = DateTime.UtcNow
        };
    }
}
