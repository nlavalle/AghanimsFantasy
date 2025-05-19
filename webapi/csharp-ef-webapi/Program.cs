using csharp_ef_webapi.Services;
using DataAccessLibrary.Data;
using DataAccessLibrary.Data.Facades;
using DataAccessLibrary.Data.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var vueFrontEndOrigins = "vueFrontEnd";

if (builder.Environment.IsDevelopment())
{
    //Set CORS for Dev
    builder.Services.AddCors(
        options =>
        {
            options.AddPolicy(name: vueFrontEndOrigins,
                                policy =>
                                {
                                    policy.WithOrigins("https://localhost:5001", "https://localhost:5173")
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

builder.Services.AddIdentity<AghanimsFantasyUser, IdentityRole>()
    .AddEntityFrameworkStores<AghanimsFantasyContext>()
    .AddApiEndpoints();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"/webapi-data"));

// Add persistent HttpClient
builder.Services.AddHttpClient();

// Add auth services
builder.Services.AddDistributedMemoryCache(); // Required for Session
builder.Services.AddOutputCache(options =>
{
    // Invalidate cache after 5min
    options.DefaultExpirationTimeSpan = TimeSpan.FromMinutes(5);
});
builder.Services.AddSession();
builder.Services.AddAntiforgery();
builder.Services
    .AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") ?? string.Empty;
        options.ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET") ?? string.Empty;
        options.SignInScheme = IdentityConstants.ExternalScheme;
        // options.CallbackPath = new PathString("/signin-google"); // Default callback path
    })
    .AddOAuth("Discord", options =>
    {
        options.ClientId = Environment.GetEnvironmentVariable("DISCORD_APP_ID") ?? string.Empty;
        options.ClientSecret = Environment.GetEnvironmentVariable("DISCORD_APP_SECRET") ?? string.Empty;
        options.SignInScheme = IdentityConstants.ExternalScheme;

        string authEndpoint = QueryHelpers.AddQueryString("https://discordapp.com/api/oauth2/authorize", "prompt", "none");

        options.AuthorizationEndpoint = authEndpoint;
        options.CallbackPath = new PathString("/api/auth/callback");
        options.TokenEndpoint = "https://discordapp.com/api/oauth2/token";
        options.UserInformationEndpoint = "https://discordapp.com/api/users/@me";
        options.AccessDeniedPath = new PathString("/api/auth/accessdenied");
        options.Scope.Add("identify");
        options.SaveTokens = true;

        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;

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

// Add Scoped services to be used by controllers
builder.Services.AddScoped<FantasyService>();
builder.Services.AddScoped<FantasyServiceAdmin>();
builder.Services.AddScoped<FantasyServicePrivateFantasyAdmin>();

// Add Scoped data services to be used by controllers
builder.Services.AddScoped<AuthFacade>();
builder.Services.AddScoped<DiscordFacade>();
builder.Services.AddScoped<FantasyDraftFacade>();
builder.Services.AddScoped<FantasyMatchFacade>();
builder.Services.AddScoped<FantasyPointsFacade>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName.Equals("Local"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(vueFrontEndOrigins);
app.UseOutputCache();

app.UseAuthentication();
app.UseAuthorization();
app.MapGroup("/identity").MapIdentityApi<AghanimsFantasyUser>();

app.UseSession();

app.MapControllers();

app.Run();
