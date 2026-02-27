using MarchMadness.Application.Engines;
using MarchMadness.Application.Services;
using MarchMadness.Core.Interfaces;
using MarchMadness.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Use port 5050 to avoid conflict with Web app (default 5000)
if (string.IsNullOrEmpty(builder.Configuration["ASPNETCORE_URLS"]))
    builder.WebHost.UseUrls("http://127.0.0.1:5050");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IBracketSeedProvider>(_ =>
{
    var path = Path.Combine(AppContext.BaseDirectory, "Data", "seed-data-2025-26.json");
    return new StaticBracketSeedProvider(path);
});
builder.Services.AddScoped<IBracketSimulationEngine, SeedBasedSimulationEngine>();
builder.Services.AddScoped<BracketService>();
builder.Services.AddScoped<BracketSimulationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/bracket", async (BracketService bracketService, string? season) =>
{
    var bracket = await bracketService.GetBracketAsync(season ?? "2025-26");
    return Results.Ok(new { bracket.Season, bracket.Source, bracket.Regions, bracket.Teams, bracket.Games });
}).WithName("GetBracket").Produces(200);

app.MapPost("/api/bracket/simulate", async (BracketSimulationService simulationService, BracketService bracketService, string? season) =>
{
    var bracket = await bracketService.GetBracketAsync(season ?? "2025-26");
    var simulated = await simulationService.SimulateFullBracketAsync(bracket);
    return Results.Ok(new { simulated.Season, simulated.Regions, simulated.Teams, simulated.Games });
}).WithName("SimulateBracket").Produces(200);

app.Run();
