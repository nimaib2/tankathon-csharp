using Tankathon.Core.Models;

namespace Tankathon.Core.Interfaces;

/// <summary>
/// Serves lottery standings (from cache/DB or provider) with odds attached.
/// </summary>
public interface IStandingsService
{
    Task<IReadOnlyList<TeamStanding>> GetLotteryStandingsAsync(string season, CancellationToken cancellationToken = default);
}
