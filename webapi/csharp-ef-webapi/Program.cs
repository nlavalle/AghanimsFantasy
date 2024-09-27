using csharp_ef_webapi.Migrations;
using csharp_ef_webapi.Services;
using DataAccessLibrary.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var vueFrontEndOrigins = "vueFrontEnd";

if (builder.Environment.IsDevelopment())
{
    //Set CORS
    builder.Services.AddCors(
        options =>
        {
            options.AddPolicy(name: vueFrontEndOrigins,
                                policy =>
                                {
                                    policy.WithOrigins("http://localhost:8080",
                                                        "http://localhost:9000")
                                        .AllowAnyHeader()
                                        .WithMethods("GET", "POST")
                                        .AllowCredentials();
                                });
        }
    );
}

if (builder.Environment.IsProduction())
{
    //Set CORS
    builder.Services.AddCors(
        options =>
        {
            options.AddPolicy(name: vueFrontEndOrigins,
                                policy =>
                                {
                                    policy.WithOrigins("https://localhost:5001")
                                        .AllowAnyHeader()
                                        .WithMethods("GET", "POST")
                                        .AllowCredentials();
                                });
        }
    );
}

// Add Database
builder.Services.AddDbContext<AghanimsFantasyContext>(
    options =>
    {
        var conn_string = builder.Configuration.GetConnectionString("AghanimsFantasyDatabase");
        conn_string = conn_string?.Replace("{SQL_HOST}", Environment.GetEnvironmentVariable("SQL_HOST")) ?? "Host=localhost;Port=5432;Database=postgres;";
        conn_string = conn_string.Replace("{SQL_USER}", Environment.GetEnvironmentVariable("SQL_USER") ?? "postgres");
        conn_string = conn_string.Replace("{SQL_PASSWORD}", Environment.GetEnvironmentVariable("SQL_PASSWORD") ?? "postgres");
        options.UseNpgsql(conn_string, b => b.MigrationsAssembly("csharp-ef-webapi"));
    }
);

// Add persistent HttpClient
builder.Services.AddHttpClient();

// Add auth services
builder.Services.AddDistributedMemoryCache(); // Required for Session
builder.Services.AddSession();
builder.Services.AddAntiforgery();
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = "OAuth";
    })
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromDays(10);
        options.Cookie.MaxAge = TimeSpan.FromDays(10);
    })
    .AddOAuth("OAuth", options =>
    {
        options.ClientId = Environment.GetEnvironmentVariable("DISCORD_APP_ID") ?? "";
        options.ClientSecret = Environment.GetEnvironmentVariable("DISCORD_APP_SECRET") ?? "";

        options.CallbackPath = new PathString("/api/auth/callback");
        options.AuthorizationEndpoint = "https://discordapp.com/api/oauth2/authorize";
        options.TokenEndpoint = "https://discordapp.com/api/oauth2/token";
        options.UserInformationEndpoint = "https://discordapp.com/api/users/@me";
        options.AccessDeniedPath = new PathString("/api/auth/accessdenied");
        options.Scope.Add("identify");
        options.SaveTokens = true;

        options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id", ClaimValueTypes.UInteger64);
        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "username", ClaimValueTypes.String);
        options.ClaimActions.MapJsonKey("urn:discord:discriminator", "discriminator", ClaimValueTypes.UInteger32);
        options.ClaimActions.MapJsonKey("urn:discord:avatar", "avatar", ClaimValueTypes.String);
        options.ClaimActions.MapJsonKey("urn:discord:verified", "verified", ClaimValueTypes.Boolean);

        // Call the discord @me endpoint to get user info to set in httpcontext
        options.Events = new OAuthEvents
        {
            OnCreatingTicket = async context =>
            {
                // Get user info from the userinfo endpoint and use it to populate user claims
                var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                response.EnsureSuccessStatusCode();

                var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;

                context.RunClaimActions(user);
            }
        };
    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Fantasy Repositories
builder.Services.AddScoped<IFantasyDraftRepository, FantasyDraftRepository>();
builder.Services.AddScoped<IFantasyLeagueRepository, FantasyLeagueRepository>();
builder.Services.AddScoped<IFantasyPlayerRepository, FantasyPlayerRepository>();
builder.Services.AddScoped<IFantasyMatchRepository, FantasyMatchRepository>();
builder.Services.AddScoped<IFantasyMatchPlayerRepository, FantasyMatchPlayerRepository>();
builder.Services.AddScoped<IFantasyRepository, FantasyRepository>();
// Add Game Coordinator Repositories
builder.Services.AddScoped<IGcMatchMetadataRepository, GcMatchMetadataRepository>();
// Add ProMetadata Repositories
builder.Services.AddScoped<IProMetadataRepository, ProMetadataRepository>();
// Add WebApi Repositories
builder.Services.AddScoped<IMatchHistoryRepository, MatchHistoryRepository>();
builder.Services.AddScoped<IMatchDetailRepository, MatchDetailRepository>();
// Add Discord Repositories
builder.Services.AddScoped<IDiscordRepository, DiscordRepository>();

// Add Scoped services to be used by controllers to limit direct repository access
builder.Services.AddScoped<DiscordWebApiService>();
builder.Services.AddScoped<FantasyService>();
builder.Services.AddScoped<FantasyServiceAdmin>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(vueFrontEndOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllers();

app.Run();
