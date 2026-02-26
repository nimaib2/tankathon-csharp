using Tankathon.Core.Models;

namespace Tankathon.Application.Lottery;

/// <summary>
/// Runs a single weighted lottery draw for picks 1-4, then assigns 5-14 by inverse standings.
/// Uses official post-2019 odds (14% / 14% / 14% for worst 3, etc.).
/// </summary>
public class LotteryEngine
{
    // Post-2019 NBA lottery odds (rank 1 = worst team). Odds for #1 pick.
    private static readonly double[] OddsForFirst = { 0.14, 0.14, 0.14, 0.125, 0.105, 0.09, 0.075, 0.06, 0.045, 0.03, 0.02, 0.015, 0.01, 0.005 };

    public DraftOrderResult Draw(IReadOnlyList<TeamStanding> standings, int? seed = null)
    {
        if (standings.Count < 14)
            throw new ArgumentException("Need 14 teams for lottery.", nameof(standings));

        var rng = seed.HasValue ? new Random(seed.Value) : new Random();
        var remaining = standings.Select(t => (t.Rank, team: t)).ToList();
        var picks = new List<DraftPickResult>(14);

        // Draw picks 1-4 by weighted random (simplified: use cumulative odds)
        for (int pick = 1; pick <= 4 && remaining.Count > 0; pick++)
        {
            var (selectedTeam, newRemaining) = DrawOne(remaining, rng);
            picks.Add(new DraftPickResult
            {
                Pick = pick,
                TeamId = selectedTeam.TeamId,
                TeamName = selectedTeam.TeamName,
                Abbreviation = selectedTeam.Abbreviation
            });
            remaining = newRemaining;
        }

        // Picks 5-14: remaining teams in order of worst record (rank 1 first)
        int p = 5;
        foreach (var (_, team) in remaining.OrderBy(x => x.Rank))
        {
            picks.Add(new DraftPickResult
            {
                Pick = p++,
                TeamId = team.TeamId,
                TeamName = team.TeamName,
                Abbreviation = team.Abbreviation
            });
        }

        return new DraftOrderResult
        {
            Season = "",
            Picks = picks.OrderBy(x => x.Pick).ToList()
        };
    }

    private static (TeamStanding selectedTeam, List<(int Rank, TeamStanding team)> remaining) DrawOne(
        List<(int Rank, TeamStanding team)> remaining,
        Random rng)
    {
        var weights = remaining.Select(r => OddsForFirst[r.Rank - 1]).ToArray();
        double total = weights.Sum();
        double roll = rng.NextDouble() * total;
        int i = 0;
        foreach (var w in weights)
        {
            roll -= w;
            if (roll <= 0) break;
            i++;
        }
        if (i >= remaining.Count) i = remaining.Count - 1;
        var selected = remaining[i].team;
        var newRemaining = remaining.Where((_, idx) => idx != i).ToList();
        return (selected, newRemaining);
    }
}
