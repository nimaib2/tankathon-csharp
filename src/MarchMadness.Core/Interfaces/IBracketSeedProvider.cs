using MarchMadness.Core.Models;

namespace MarchMadness.Core.Interfaces;

public interface IBracketSeedProvider
{
    Task<Bracket> GetBracketAsync(string season, CancellationToken cancellationToken = default);
}
