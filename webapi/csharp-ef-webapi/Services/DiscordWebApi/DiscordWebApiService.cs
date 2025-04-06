using System.Net;
using System.Security.Claims;
using System.Text.Json;
using DataAccessLibrary.Data;
using DataAccessLibrary.Data.Facades;
using DataAccessLibrary.Models.Discord;
using Microsoft.EntityFrameworkCore;

namespace csharp_ef_webapi.Services;
public class DiscordWebApiService
{
    private readonly string _discordToken;
    private readonly ILogger<DiscordWebApiService> _logger;
    private readonly HttpClient _httpClient;
    private readonly AghanimsFantasyContext _dbContext;
    private readonly AuthFacade _authFacade;
    private readonly DiscordFacade _discordFacade;

    public DiscordWebApiService(
        ILogger<DiscordWebApiService> logger,
        HttpClient httpClient,
        AghanimsFantasyContext dbContext,
        AuthFacade authFacade,
        DiscordFacade discordFacade
    )
    {
        _logger = logger;
        _httpClient = httpClient;
        _dbContext = dbContext;
        _authFacade = authFacade;
        _discordFacade = discordFacade;

        _discordToken = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN") ?? "";

        _logger.LogDebug("Discord WebApi Service constructed");
    }

    public async Task<bool> CheckAdminUser(HttpContext httpContext)
    {
        // Admin only operation
        var nameId = httpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (nameId == null) throw new ArgumentException("Invalid User Claims please contact admin");

        bool getAccountId = long.TryParse(nameId.Value, out long userDiscordAccountId);
        return await _authFacade.IsUserAdminAsync(userDiscordAccountId);
    }

    public async Task<bool> CheckPrivateFantasyAdminUser(long userDiscordAccountId)
    {
        return await _authFacade.IsUserPrivateFantasyAdminAsync(userDiscordAccountId);
    }

    public async Task<DiscordUser?> LookupHttpContextUser(HttpContext httpContext)
    {
        if (!httpContext?.User?.Identity?.IsAuthenticated ?? false)
        {
            // Authorize should take care of this but just in case
            return null;
        }

        var nameId = httpContext!.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        bool getAccountId = long.TryParse(httpContext!.User!.Claims!.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault()!.Value, out long userDiscordAccountId);
        if (!getAccountId)
        {
            throw new ArgumentException("Could not retrieve user discord ID");
        }

        DiscordUser? discordUser = await _dbContext.DiscordUsers.FindAsync(userDiscordAccountId);

        if (discordUser == null)
        {
            discordUser = await AddNewDiscordUserByIdAsync(userDiscordAccountId);
        }

        return discordUser;
    }

    public async Task<DiscordUser?> GetDiscordUserAsync(long userId)
    {
        return await _dbContext.DiscordUsers.FindAsync(userId);
    }

    public async Task<DiscordUser?> GetDiscordUserAsync(string discordUsername)
    {
        return await _dbContext.DiscordUsers.FirstOrDefaultAsync(du => du.Username == discordUsername);
    }

    public async Task<DiscordUser?> AddNewDiscordUserByIdAsync(long DiscordId)
    {
        _logger.LogInformation("Fetching new user from Discord API");

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://discord.com/api/v10/users/{DiscordId}");
            request.Headers.Add("Authorization", $"Bot {_discordToken}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            JsonDocument responseRawJDocument = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var discordObject = JsonSerializer.Deserialize<DiscordUser>(responseRawJDocument.RootElement.GetRawText(), new JsonSerializerOptions { NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString });
            if (discordObject != null && discordObject.Username != null)
            {
                await _dbContext.DiscordUsers.AddAsync(discordObject);
                await _dbContext.SaveChangesAsync();
                return discordObject;
            }
            else
            {
                _logger.LogError($"Malformed or missing Discord response for user ID {DiscordId}");
                return null;
            }
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                _logger.LogError($"401 Unauthorized error on Discord call");
            }
            else
            {
                _logger.LogError($"{ex.StatusCode} error on Discord call. Full message: {ex.Message}");
            }
            return null;
        }
    }
}