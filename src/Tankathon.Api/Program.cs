using Tankathon.Application.Lottery;
using Tankathon.Application.Services;
using Tankathon.Core.Interfaces;
using Tankathon.Infrastructure.NbaStandings;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Application & infrastructure
builder.Services.AddScoped<IStandingsService, StandingsService>();
builder.Services.AddScoped<ILotteryService, LotteryService>();
builder.Services.AddSingleton<LotteryEngine>();
builder.Services.AddScoped<INbaStandingsProvider, StubStandingsProvider>();

// CORS (enable when you add a frontend)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors();
// Only redirect to HTTPS when we're actually listening on HTTPS (e.g. production)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.MapControllers();

app.Run();
