using Tankathon.Core.Interfaces;
using Tankathon.Core.Models;

namespace Tankathon.Infrastructure.NbaStandings;

/// <summary>
/// Returns sample lottery standings for development. Replace with a real provider (API or CSV) later.
/// </summary>
public class StubStandingsProvider : INbaStandingsProvider
{
    public Task<IReadOnlyList<TeamStanding>> GetLotteryStandingsAsync(string season, CancellationToken cancellationToken = default)
    {
        var standings = new List<TeamStanding>
        {
            Create(1, "DET", "Detroit Pistons", 14, 68),
            Create(2, "WAS", "Washington Wizards", 15, 67),
            Create(3, "CHA", "Charlotte Hornets", 21, 61),
            Create(4, "POR", "Portland Trail Blazers", 23, 59),
            Create(5, "TOR", "Toronto Raptors", 25, 57),
            Create(6, "UTA", "Utah Jazz", 31, 51),
            Create(7, "BKN", "Brooklyn Nets", 32, 50),
            Create(8, "ATL", "Atlanta Hawks", 36, 46),
            Create(9, "HOU", "Houston Rockets", 41, 41),
            Create(10, "CHI", "Chicago Bulls", 39, 43),
            Create(11, "OKC", "Oklahoma City Thunder", 42, 40),
            Create(12, "SAS", "San Antonio Spurs", 22, 60),
            Create(13, "MEM", "Memphis Grizzlies", 27, 55),
            Create(14, "NOP", "New Orleans Pelicans", 49, 33)
        };
        return Task.FromResult<IReadOnlyList<TeamStanding>>(standings);
    }

    private static TeamStanding Create(int rank, string abbr, string name, int wins, int losses)
    {
        int total = wins + losses;
        double pct = total > 0 ? (double)wins / total : 0;
        return new TeamStanding
        {
            Rank = rank,
            TeamId = abbr,
            TeamName = name,
            Abbreviation = abbr,
            Wins = wins,
            Losses = losses,
            WinPercentage = Math.Round(pct, 3),
            OddsForFirstPick = GetOddsForFirst(rank),
            OddsForTopFour = GetOddsTopFour(rank)
        };
    }

    private static double GetOddsForFirst(int rank)
    {
        double[] odds = { 0.14, 0.14, 0.14, 0.125, 0.105, 0.09, 0.075, 0.06, 0.045, 0.03, 0.02, 0.015, 0.01, 0.005 };
        return rank >= 1 && rank <= 14 ? odds[rank - 1] : 0;
    }

    private static double GetOddsTopFour(int rank)
    {
        double[] odds = { 0.521, 0.521, 0.521, 0.482, 0.421, 0.372, 0.319, 0.263, 0.199, 0.134, 0.09, 0.067, 0.045, 0.023 };
        return rank >= 1 && rank <= 14 ? odds[rank - 1] : 0;
    }
}
