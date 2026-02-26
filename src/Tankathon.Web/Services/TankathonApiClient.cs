using Tankathon.Core.Models;

namespace Tankathon.Web.Services;

public class TankathonApiClient
{
    private readonly HttpClient _http;

    public TankathonApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<TeamStanding>> GetStandingsAsync(string season = "2024-25", CancellationToken cancellationToken = default)
    {
        var list = await _http.GetFromJsonAsync<List<TeamStanding>>($"api/standings?season={Uri.EscapeDataString(season)}", cancellationToken);
        return (IReadOnlyList<TeamStanding>?)list ?? Array.Empty<TeamStanding>();
    }

    public async Task<DraftOrderResult?> SimulateLotteryAsync(string season = "2024-25", int? seed = null, CancellationToken cancellationToken = default)
    {
        var response = await _http.PostAsJsonAsync("api/lottery/simulate", new { season, seed }, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DraftOrderResult>(cancellationToken);
    }

    public async Task<IReadOnlyList<DraftOrderResult>> SimulateLotteryManyAsync(string season = "2024-25", int runs = 10, CancellationToken cancellationToken = default)
    {
        var response = await _http.PostAsJsonAsync($"api/lottery/simulate-many?runs={runs}", new { season }, cancellationToken);
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<List<DraftOrderResult>>(cancellationToken);
        return (IReadOnlyList<DraftOrderResult>?)list ?? Array.Empty<DraftOrderResult>();
    }
}
