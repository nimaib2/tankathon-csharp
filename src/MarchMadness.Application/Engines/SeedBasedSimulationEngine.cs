using MarchMadness.Core.Interfaces;
using MarchMadness.Core.Models;

namespace MarchMadness.Application.Engines;

/// <summary>Win probability by seed difference; higher seed (lower number) favored.</summary>
public class SeedBasedSimulationEngine : IBracketSimulationEngine
{
    private static readonly Random Random = new();

    public Task<GameResult> SimulateGameAsync(BracketGame game, IReadOnlyList<Team> teams, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var home = teams.FirstOrDefault(t => t.Id == game.HomeTeamId);
        var away = teams.FirstOrDefault(t => t.Id == game.AwayTeamId);
        if (home == null || away == null)
        {
            var winnerId = game.HomeTeamId ?? game.AwayTeamId ?? string.Empty;
            return Task.FromResult(new GameResult { GameId = game.Id, WinnerId = winnerId, ScoreHome = 0, ScoreAway = 0 });
        }

        double homeWinProb = GetWinProbability(home.Seed, away.Seed);
        int scoreHome = 65 + Random.Next(20);
        int scoreAway = 65 + Random.Next(20);
        bool homeWins = Random.NextDouble() < homeWinProb;
        if (homeWins)
            scoreHome = Math.Max(scoreHome, scoreAway + 1);
        else
            scoreAway = Math.Max(scoreAway, scoreHome + 1);

        var result = new GameResult
        {
            GameId = game.Id,
            WinnerId = homeWins ? home.Id : away.Id,
            ScoreHome = scoreHome,
            ScoreAway = scoreAway
        };
        return Task.FromResult(result);
    }

    public async Task<Bracket> SimulateFullBracketAsync(Bracket bracket, CancellationToken cancellationToken = default)
    {
        var gamesByRound = bracket.Games.OrderBy(g => g.Round).ThenBy(g => g.GameIndex).ToList();
        var teams = bracket.Teams;
        var results = new Dictionary<string, GameResult>();
        var teamIdsByGame = new Dictionary<string, (string HomeId, string AwayId)>();

        foreach (var game in gamesByRound)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string? homeId = game.HomeTeamId;
            string? awayId = game.AwayTeamId;

            if (game.Round > 0 && game.SourceGame1Id != null && game.SourceGame2Id != null)
            {
                homeId = results.GetValueOrDefault(game.SourceGame1Id)?.WinnerId;
                awayId = results.GetValueOrDefault(game.SourceGame2Id)?.WinnerId;
            }

            if (string.IsNullOrEmpty(homeId) || string.IsNullOrEmpty(awayId))
                continue;

            teamIdsByGame[game.Id] = (homeId, awayId);
            var gameWithTeams = new BracketGame
            {
                Id = game.Id,
                HomeTeamId = homeId,
                AwayTeamId = awayId,
                Round = game.Round,
                GameIndex = game.GameIndex
            };
            var result = await SimulateGameAsync(gameWithTeams, teams, cancellationToken);
            results[game.Id] = result;
        }

        var updatedGames = bracket.Games.Select(g =>
        {
            if (!results.TryGetValue(g.Id, out var r)) return g;
            var (homeId, awayId) = teamIdsByGame.GetValueOrDefault(g.Id, (g.HomeTeamId ?? "", g.AwayTeamId ?? ""));
            return new BracketGame
            {
                Id = g.Id,
                Round = g.Round,
                RegionId = g.RegionId,
                HomeTeamId = homeId,
                AwayTeamId = awayId,
                WinnerId = r.WinnerId,
                ScoreHome = r.ScoreHome,
                ScoreAway = r.ScoreAway,
                GameIndex = g.GameIndex,
                SourceGame1Id = g.SourceGame1Id,
                SourceGame2Id = g.SourceGame2Id
            };
        }).ToList();

        return new Bracket
        {
            Id = bracket.Id,
            Season = bracket.Season,
            Source = bracket.Source,
            Regions = bracket.Regions,
            Teams = bracket.Teams,
            Games = updatedGames
        };
    }

    private static double GetWinProbability(int seed1, int seed2)
    {
        // Higher seed = lower number = better. Approximate historical odds.
        double diff = seed2 - seed1;
        return 0.5 + (diff * 0.03); // e.g. 1 vs 16 -> ~0.95 for 1
    }
}
