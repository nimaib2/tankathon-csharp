using Microsoft.AspNetCore.Mvc;
using Tankathon.Core.Interfaces;
using Tankathon.Core.Models;

namespace Tankathon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LotteryController : ControllerBase
{
    private readonly ILotteryService _lotteryService;

    public LotteryController(ILotteryService lotteryService)
    {
        _lotteryService = lotteryService;
    }

    /// <summary>
    /// POST /api/lottery/simulate
    /// Body (optional): { "season": "2025-26", "standingsOverride": [...], "seed": 12345 }
    /// Runs one lottery simulation and returns draft order 1-14.
    /// </summary>
    [HttpPost("simulate")]
    public async Task<IActionResult> Simulate([FromBody] SimulateRequest? request, CancellationToken cancellationToken = default)
    {
        var season = request?.Season ?? "2025-26";
        var result = await _lotteryService.SimulateAsync(season, request?.StandingsOverride, request?.Seed, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// POST /api/lottery/simulate-many?runs=100
    /// Runs multiple simulations. Returns list of draft order results (for aggregate stats client-side).
    /// </summary>
    [HttpPost("simulate-many")]
    public async Task<IActionResult> SimulateMany([FromQuery] int runs = 10, [FromBody] SimulateRequest? request = null, CancellationToken cancellationToken = default)
    {
        if (runs < 1 || runs > 1000) return BadRequest("runs must be between 1 and 1000.");
        var season = request?.Season ?? "2025-26";
        var results = await _lotteryService.SimulateManyAsync(season, runs, request?.StandingsOverride, cancellationToken);
        return Ok(results);
    }
}

public class SimulateRequest
{
    public string? Season { get; set; }
    public IReadOnlyList<TeamStanding>? StandingsOverride { get; set; }
    public int? Seed { get; set; }
}
