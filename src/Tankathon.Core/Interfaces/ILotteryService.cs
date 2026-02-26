using Tankathon.Core.Models;

namespace Tankathon.Core.Interfaces;

/// <summary>
/// Runs single or multiple lottery simulations given standings.
/// </summary>
public interface ILotteryService
{
    Task<DraftOrderResult> SimulateAsync(string season, IReadOnlyList<TeamStanding>? standingsOverride = null, int? seed = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DraftOrderResult>> SimulateManyAsync(string season, int runs, IReadOnlyList<TeamStanding>? standingsOverride = null, CancellationToken cancellationToken = default);
}
