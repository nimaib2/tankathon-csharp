namespace Tankathon.Core.Models;

/// <summary>
/// Full result of a lottery simulation: order of picks 1-14.
/// </summary>
public class DraftOrderResult
{
    public string Season { get; set; } = string.Empty;
    public IReadOnlyList<DraftPickResult> Picks { get; set; } = Array.Empty<DraftPickResult>();
}
