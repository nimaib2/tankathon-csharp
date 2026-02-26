namespace MarchMadness.Core.Models;

public class Team
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string Conference { get; set; } = string.Empty;
    public int Seed { get; set; }
    public string RegionId { get; set; } = string.Empty;
    public bool IsFirstFour { get; set; }
    /// <summary>First Four slot label, e.g. "11a", "11b", "16a", "16b".</summary>
    public string? FirstFourSlot { get; set; }
}
