using csharp_ef_data_loader.Services;
using DataAccessLibrary.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Database
builder.Services.AddDbContext<AghanimsFantasyContext>(
    options =>
    {
        var conn_string = builder.Configuration.GetConnectionString("AghanimsFantasyDatabase");
        conn_string = conn_string?.Replace("{SQL_HOST}", Environment.GetEnvironmentVariable("SQL_HOST")) ?? "Host=localhost;Port=5432;Database=postgres;";
        conn_string = conn_string.Replace("{SQL_USER}", Environment.GetEnvironmentVariable("SQL_USER"));
        conn_string = conn_string.Replace("{SQL_PASSWORD}", Environment.GetEnvironmentVariable("SQL_PASSWORD"));
        options.UseNpgsql(conn_string);
    }
);

// Add persistent HttpClient
builder.Services.AddHttpClient();

// Add Repositories to be used by controllers
builder.Services.AddScoped<FantasyRepository>();
builder.Services.AddScoped<ProMetadataRepository>();
builder.Services.AddScoped<WebApiRepository>();
builder.Services.AddScoped<GameCoordinatorRepository>();
builder.Services.AddScoped<DiscordRepository>();

// Add Scoped Dota Client to be called for Dota Client Background Service
builder.Services.AddScoped<DotaClient>();

// Add WebApi and Steam Client Services
builder.Services.AddHostedService<DotaWebApiService>();
builder.Services.AddHostedService<DotaSteamClientService>();


var app = builder.Build();

app.UseHttpsRedirection();

app.Run();
