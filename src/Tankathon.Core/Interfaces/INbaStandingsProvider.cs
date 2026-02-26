using Tankathon.Core.Models;

namespace Tankathon.Core.Interfaces;

/// <summary>
/// Fetches current (or historical) NBA lottery standings from an external source or file.
/// Implement in Infrastructure (e.g. NbaApiStandingsProvider or CsvStandingsProvider).
/// </summary>
public interface INbaStandingsProvider
{
    Task<IReadOnlyList<TeamStanding>> GetLotteryStandingsAsync(string season, CancellationToken cancellationToken = default);
}
