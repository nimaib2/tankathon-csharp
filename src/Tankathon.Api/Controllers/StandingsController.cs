using Microsoft.AspNetCore.Mvc;
using Tankathon.Core.Interfaces;

namespace Tankathon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StandingsController : ControllerBase
{
    private readonly IStandingsService _standingsService;

    public StandingsController(IStandingsService standingsService)
    {
        _standingsService = standingsService;
    }

    /// <summary>
    /// GET /api/standings?season=2025-26
    /// Returns current lottery standings (worst 14 teams) with odds.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string season = "2025-26", CancellationToken cancellationToken = default)
    {
        var standings = await _standingsService.GetLotteryStandingsAsync(season, cancellationToken);
        return Ok(standings);
    }
}
