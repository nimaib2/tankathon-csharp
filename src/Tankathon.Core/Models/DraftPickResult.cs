namespace Tankathon.Core.Models;

/// <summary>
/// Single pick in a lottery result (pick number and team).
/// </summary>
public class DraftPickResult
{
    public int Pick { get; set; }
    public string TeamId { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
}
