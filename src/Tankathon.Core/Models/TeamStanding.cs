namespace Tankathon.Core.Models;

/// <summary>
/// Represents a team's position in the lottery standings (inverse standings: worst = rank 1).
/// </summary>
public class TeamStanding
{
    public int Rank { get; set; }
    public string TeamId { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public int Wins { get; set; }
    public int Losses { get; set; }
    public double WinPercentage { get; set; }
    public double OddsForFirstPick { get; set; }
    public double OddsForTopFour { get; set; }
}
