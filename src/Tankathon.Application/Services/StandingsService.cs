using Tankathon.Core.Interfaces;
using Tankathon.Core.Models;

namespace Tankathon.Application.Services;

public class StandingsService : IStandingsService
{
    private readonly INbaStandingsProvider _standingsProvider;

    public StandingsService(INbaStandingsProvider standingsProvider)
    {
        _standingsProvider = standingsProvider;
    }

    public async Task<IReadOnlyList<TeamStanding>> GetLotteryStandingsAsync(string season, CancellationToken cancellationToken = default)
    {
        var standings = await _standingsProvider.GetLotteryStandingsAsync(season, cancellationToken);
        // TODO: attach odds from LotteryOdds table by rank; optionally cache in DB
        return standings;
    }
}
