namespace MarchMadness.Core.Models;

public class Bracket
{
    public string Id { get; set; } = string.Empty;
    public string Season { get; set; } = string.Empty;
    public string? Source { get; set; }
    public IReadOnlyList<Region> Regions { get; set; } = Array.Empty<Region>();
    public IReadOnlyList<Team> Teams { get; set; } = Array.Empty<Team>();
    public IReadOnlyList<BracketGame> Games { get; set; } = Array.Empty<BracketGame>();
}
