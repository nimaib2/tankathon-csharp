using MarchMadness.Core.Interfaces;
using MarchMadness.Core.Models;

namespace MarchMadness.Application.Services;

public class BracketService
{
    private readonly IBracketSeedProvider _seedProvider;

    public BracketService(IBracketSeedProvider seedProvider)
    {
        _seedProvider = seedProvider;
    }

    public async Task<Bracket> GetBracketAsync(string season, CancellationToken cancellationToken = default)
    {
        var bracket = await _seedProvider.GetBracketAsync(season, cancellationToken);
        // If provider didn't build games, we could build them here (Round of 64 → Championship)
        return bracket;
    }
}
