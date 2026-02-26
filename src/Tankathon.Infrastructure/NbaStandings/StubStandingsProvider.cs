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
            Create(1, "SAC", "Sacramento Kings", 13, 46),
            Create(2, "IND", "Indiana Pacers", 15, 44),
            Create(3, "BKN", "Brooklyn Nets", 15, 42),
            Create(4, "WAS", "Washington Wizards", 16, 41),
            Create(5, "ATL", "Atlanta Hawks", 25, 35),
            Create(6, "UTA", "Utah Jazz", 18, 40),
            Create(7, "DAL", "Dallas Mavericks", 21, 36),
            Create(8, "MEM", "Memphis Grizzlies", 21, 35),
            Create(9, "CHI", "Chicago Bulls", 24, 35),
            Create(10, "MIL", "Milwaukee Bucks", 25, 31),
            Create(11, "OKC", "Oklahoma City Thunder", 42, 20),
            Create(12, "POR", "Portland Trail Blazers", 28, 31),
            Create(13, "CHA", "Charlotte Hornets", 28, 31),
            Create(14, "SAS", "San Antonio Spurs", 29, 31)
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
