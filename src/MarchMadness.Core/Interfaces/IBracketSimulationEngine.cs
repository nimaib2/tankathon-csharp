using MarchMadness.Core.Models;

namespace MarchMadness.Core.Interfaces;

public interface IBracketSimulationEngine
{
    Task<GameResult> SimulateGameAsync(BracketGame game, IReadOnlyList<Team> teams, CancellationToken cancellationToken = default);
    Task<Bracket> SimulateFullBracketAsync(Bracket bracket, CancellationToken cancellationToken = default);
}
