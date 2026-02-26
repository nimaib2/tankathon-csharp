namespace MarchMadness.Core.Models;

/// <summary>Round: 0 = First Four, 1 = Round of 64, 2 = 32, 3 = Sweet 16, 4 = Elite Eight, 5 = Final Four, 6 = Championship.</summary>
public class BracketGame
{
    public string Id { get; set; } = string.Empty;
    public int Round { get; set; }
    public string? RegionId { get; set; }
    public string? HomeTeamId { get; set; }
    public string? AwayTeamId { get; set; }
    public string? WinnerId { get; set; }
    public int? ScoreHome { get; set; }
    public int? ScoreAway { get; set; }
    public DateTimeOffset? PlayedAt { get; set; }
    /// <summary>Index within the round for ordering (e.g. game 1 of Round 1).</summary>
    public int GameIndex { get; set; }
    /// <summary>For rounds &gt; 1: the two games whose winners feed this game.</summary>
    public string? SourceGame1Id { get; set; }
    public string? SourceGame2Id { get; set; }
}
