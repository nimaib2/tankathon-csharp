using System.Text.Json;
using MarchMadness.Core.Interfaces;
using MarchMadness.Core.Models;

namespace MarchMadness.Infrastructure;

public class StaticBracketSeedProvider : IBracketSeedProvider
{
    private readonly string _dataPath;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public StaticBracketSeedProvider(string? dataPath = null)
    {
        _dataPath = dataPath ?? Path.Combine(AppContext.BaseDirectory, "Data", "seed-data-2025-26.json");
    }

    public async Task<Bracket> GetBracketAsync(string season, CancellationToken cancellationToken = default)
    {
        await using var stream = File.OpenRead(_dataPath);
        var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var root = doc.RootElement;

        var regions = root.GetProperty("regions").EnumerateArray()
            .Select(r => new Region { Id = r.GetProperty("id").GetString()!, Name = r.GetProperty("name").GetString()! })
            .ToList();

        var teams = root.GetProperty("teams").EnumerateArray()
            .Select(t => new Team
            {
                Id = t.GetProperty("id").GetString()!,
                Name = t.GetProperty("name").GetString()!,
                ShortName = t.TryGetProperty("shortName", out var sn) ? sn.GetString() ?? "" : "",
                Conference = t.TryGetProperty("conference", out var c) ? c.GetString() ?? "" : "",
                Seed = t.GetProperty("seed").GetInt32(),
                RegionId = t.GetProperty("regionId").GetString()!,
                IsFirstFour = t.TryGetProperty("isFirstFour", out var ff) && ff.GetBoolean(),
                FirstFourSlot = t.TryGetProperty("firstFourSlot", out var fs) ? fs.GetString() : null
            })
            .ToList();

        var teamByRegionAndSeed = teams.Where(x => !x.IsFirstFour).GroupBy(t => t.RegionId).ToDictionary(g => g.Key, g => g.ToDictionary(t => t.Seed));

        // Round 1 matchups per region: (1,16), (8,9), (4,13), (5,12), (2,15), (7,10), (3,14), (6,11)
        var r1Matchups = new[] { (1, 16), (8, 9), (4, 13), (5, 12), (2, 15), (7, 10), (3, 14), (6, 11) };
        var regionIds = regions.Select(r => r.Id).ToList();
        var games = new List<BracketGame>();
        var gameIndex = 0;

        // Round 1: 32 games
        foreach (var regionId in regionIds)
        {
            if (!teamByRegionAndSeed.TryGetValue(regionId, out var bySeed)) continue;
            foreach (var (s1, s2) in r1Matchups)
            {
                var homeId = bySeed.GetValueOrDefault(s1)?.Id;
                var awayId = bySeed.GetValueOrDefault(s2)?.Id;
                games.Add(new BracketGame
                {
                    Id = $"r1-{gameIndex}",
                    Round = 1,
                    RegionId = regionId,
                    HomeTeamId = homeId,
                    AwayTeamId = awayId,
                    GameIndex = gameIndex++
                });
            }
        }

        // Rounds 2-6: build from previous round winners
        int r1Count = 32, r2Count = 16, r3Count = 8, r4Count = 4, r5Count = 2;
        int r2Start = r1Count, r3Start = r2Start + r2Count, r4Start = r3Start + r3Count, r5Start = r4Start + r4Count;

        for (int i = 0; i < r2Count; i++)
            games.Add(new BracketGame { Id = $"r2-{i}", Round = 2, RegionId = regionIds[i / 4], GameIndex = i, SourceGame1Id = games[i * 2].Id, SourceGame2Id = games[i * 2 + 1].Id });
        for (int i = 0; i < r3Count; i++)
            games.Add(new BracketGame { Id = $"r3-{i}", Round = 3, RegionId = regionIds[i / 2], GameIndex = i, SourceGame1Id = games[r2Start + i * 2].Id, SourceGame2Id = games[r2Start + i * 2 + 1].Id });
        for (int i = 0; i < r4Count; i++)
            games.Add(new BracketGame { Id = $"r4-{i}", Round = 4, RegionId = regionIds[i], GameIndex = i, SourceGame1Id = games[r3Start + i * 2].Id, SourceGame2Id = games[r3Start + i * 2 + 1].Id });
        for (int i = 0; i < r5Count; i++)
            games.Add(new BracketGame { Id = $"r5-{i}", Round = 5, RegionId = null, GameIndex = i, SourceGame1Id = games[r4Start + i * 2].Id, SourceGame2Id = games[r4Start + i * 2 + 1].Id });
        games.Add(new BracketGame { Id = "r6-0", Round = 6, RegionId = null, GameIndex = 0, SourceGame1Id = games[r5Start].Id, SourceGame2Id = games[r5Start + 1].Id });

        return new Bracket
        {
            Id = $"bracket-{season}",
            Season = root.TryGetProperty("season", out var s) ? s.GetString() ?? season : season,
            Source = root.TryGetProperty("source", out var src) ? src.GetString() : null,
            Regions = regions,
            Teams = teams,
            Games = games
        };
    }
}
