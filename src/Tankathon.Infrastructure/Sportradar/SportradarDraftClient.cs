using RestSharp;

namespace Tankathon.Infrastructure.Sportradar;

/// <summary>
/// Client for Sportradar NBA draft API (e.g. top prospects).
/// Configure API key via options or environment when moving off trial.
/// </summary>
public class SportradarDraftClient
{
    private readonly RestClient _client;

    public SportradarDraftClient()
    {
        var options = new RestClientOptions("https://api.sportradar.com/draft/nba/trial/v1/en/2025/top_prospects.json");
        _client = new RestClient(options);
    }

    public async Task<string?> GetTopProspectsJsonAsync(CancellationToken cancellationToken = default)
    {
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        var response = await _client.GetAsync(request, cancellationToken);
        return response.Content;
    }
}
