using Tankathon.Core.Interfaces;
using Tankathon.Core.Models;
using Tankathon.Application.Lottery;

namespace Tankathon.Application.Services;

public class LotteryService : ILotteryService
{
    private readonly IStandingsService _standingsService;
    private readonly LotteryEngine _lotteryEngine;

    public LotteryService(IStandingsService standingsService, LotteryEngine lotteryEngine)
    {
        _standingsService = standingsService;
        _lotteryEngine = lotteryEngine;
    }

    public async Task<DraftOrderResult> SimulateAsync(string season, IReadOnlyList<TeamStanding>? standingsOverride = null, int? seed = null, CancellationToken cancellationToken = default)
    {
        var standings = standingsOverride ?? await _standingsService.GetLotteryStandingsAsync(season, cancellationToken);
        var result = _lotteryEngine.Draw(standings, seed);
        result.Season = season;
        return result;
    }

    public async Task<IReadOnlyList<DraftOrderResult>> SimulateManyAsync(string season, int runs, IReadOnlyList<TeamStanding>? standingsOverride = null, CancellationToken cancellationToken = default)
    {
        var standings = standingsOverride ?? await _standingsService.GetLotteryStandingsAsync(season, cancellationToken);
        var results = new List<DraftOrderResult>(runs);
        for (int i = 0; i < runs; i++)
        {
            var result = _lotteryEngine.Draw(standings);
            result.Season = season;
            results.Add(result);
        }
        return results;
    }
}
