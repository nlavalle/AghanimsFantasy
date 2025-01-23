using csharp_ef_data_loader.Services;
using DataAccessLibrary.Data;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile($"appsettings.{environment}.json", optional: false);

// Add Database
builder.Services.AddDbContext<AghanimsFantasyContext>(
    options =>
    {
        var conn_string = builder.Configuration.GetConnectionString("AghanimsFantasyDatabase");
        conn_string = conn_string?.Replace("{SQL_HOST}", Environment.GetEnvironmentVariable("SQL_HOST")) ?? "Host=localhost;Port=5432;Database=postgres;";
        conn_string = conn_string.Replace("{SQL_USER}", Environment.GetEnvironmentVariable("SQL_USER"));
        conn_string = conn_string.Replace("{SQL_PASSWORD}", Environment.GetEnvironmentVariable("SQL_PASSWORD"));
        options.UseNpgsql(conn_string, b => b.MigrationsAssembly("csharp-ef-webapi"));
    }
);

// Add persistent HttpClient
builder.Services.AddHttpClient();

// Add Fantasy Repositories
builder.Services.AddScoped<IFantasyDraftRepository, FantasyDraftRepository>();
builder.Services.AddScoped<IFantasyLeagueRepository, FantasyLeagueRepository>();
builder.Services.AddScoped<IFantasyPlayerRepository, FantasyPlayerRepository>();
builder.Services.AddScoped<IFantasyMatchRepository, FantasyMatchRepository>();
builder.Services.AddScoped<IFantasyMatchPlayerRepository, FantasyMatchPlayerRepository>();
builder.Services.AddScoped<IFantasyRepository, FantasyRepository>();
// Add Game Coordinator Repositories
builder.Services.AddScoped<IGcMatchMetadataRepository, GcMatchMetadataRepository>();
builder.Services.AddScoped<IGcDotaMatchRepository, GcDotaMatchRepository>();
builder.Services.AddScoped<GameCoordinatorRepository>();
// Add ProMetadata Repositories
builder.Services.AddScoped<IProMetadataRepository, ProMetadataRepository>();
// Add WebApi Repositories
builder.Services.AddScoped<IMatchHistoryRepository, MatchHistoryRepository>();
builder.Services.AddScoped<IMatchDetailRepository, MatchDetailRepository>();
// Add Discord Repositories
builder.Services.AddScoped<IDiscordRepository, DiscordRepository>();

// Add Scoped Dota Client to be called for Dota Client Background Service
builder.Services.AddScoped<DotaClient>();

// Add WebApi and Steam Client Services
builder.Services.AddHostedService<DotaWebApiService>();
builder.Services.AddHostedService<DotaSteamClientService>();

// Add Discord Bot services
builder.Services.AddSingleton(new DiscordSocketConfig
{
    GatewayIntents = Discord.GatewayIntents.AllUnprivileged | Discord.GatewayIntents.MessageContent
});
builder.Services.AddSingleton<DiscordSocketClient>();
builder.Services.AddHostedService<DiscordBotService>();


var app = builder.Build();

app.Run();
