namespace MarchMadness.Core.Models;

public class GameResult
{
    public string GameId { get; set; } = string.Empty;
    public string WinnerId { get; set; } = string.Empty;
    public int ScoreHome { get; set; }
    public int ScoreAway { get; set; }
}
