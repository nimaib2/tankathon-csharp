using MarchMadness.Core.Interfaces;
using MarchMadness.Core.Models;

namespace MarchMadness.Application.Services;

public class BracketSimulationService
{
    private readonly IBracketSimulationEngine _engine;

    public BracketSimulationService(IBracketSimulationEngine engine)
    {
        _engine = engine;
    }

    public Task<Bracket> SimulateFullBracketAsync(Bracket bracket, CancellationToken cancellationToken = default)
        => _engine.SimulateFullBracketAsync(bracket, cancellationToken);
}
