using MarchMadness.Application.Engines;
using MarchMadness.Application.Services;
using MarchMadness.Core.Interfaces;
using MarchMadness.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<IBracketSeedProvider>(_ =>
{
    var path = Path.Combine(AppContext.BaseDirectory, "Data", "seed-data-2025-26.json");
    return new StaticBracketSeedProvider(path);
});
builder.Services.AddScoped<IBracketSimulationEngine, SeedBasedSimulationEngine>();
builder.Services.AddScoped<BracketService>();
builder.Services.AddScoped<BracketSimulationService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<MarchMadness.Web.Components.App.App>()
    .AddInteractiveServerRenderMode();

app.Run();
