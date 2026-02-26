namespace MarchMadness.Core.Models;

/// <summary>One slot in the bracket (one matchup line). Round 0 = First Four.</summary>
public class BracketSlot
{
    public string SlotId { get; set; } = string.Empty;
    public int Round { get; set; }
    public string? RegionId { get; set; }
    public int Seed1 { get; set; }
    public int Seed2 { get; set; }
    public string? WinnerTeamId { get; set; }
    public int GameIndex { get; set; }
}
