using System.Net;
using System.Security.Claims;
using DataAccessLibrary.Data;
using DataAccessLibrary.Models.Discord;
using Newtonsoft.Json;

namespace csharp_ef_webapi.Services;
public class DiscordWebApiService
{
    private readonly string _discordToken;
    private readonly ILogger<DiscordWebApiService> _logger;
    private readonly HttpClient _httpClient;
    private readonly IDiscordRepository _discordRepository;

    public DiscordWebApiService(
        ILogger<DiscordWebApiService> logger,
        HttpClient httpClient,
        IDiscordRepository discordRepository
    )
    {
        _logger = logger;
        _httpClient = httpClient;
        _discordRepository = discordRepository;

        _discordToken = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN") ?? "";

        _logger.LogInformation("Discord WebApi Service constructed");
    }

    public async Task<bool> CheckAdminUser(HttpContext httpContext)
    {
        // Admin only operation
        var nameId = httpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (nameId == null) throw new ArgumentException("Invalid User Claims please contact admin");

        bool getAccountId = long.TryParse(nameId.Value, out long userDiscordAccountId);
        return await _discordRepository.IsUserAdminAsync(userDiscordAccountId);
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

        return nameId != null ? await _discordRepository.GetDiscordUserAsync(userDiscordAccountId) : null;
    }

    public async Task<DiscordUser?> GetDiscordUserAsync(long userId)
    {
        return await _discordRepository.GetDiscordUserAsync(userId);
    }

    public async Task GetDiscordByIdAsync(long DiscordId)
    {
        _logger.LogInformation("Fetching new user from Discord API");

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://discord.com/api/v10/users/{DiscordId}");
            request.Headers.Add("Authorization", $"Bot {_discordToken}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var discordObject = JsonConvert.DeserializeObject<DiscordUser>(await response.Content.ReadAsStringAsync());
            if (discordObject != null && discordObject.Username != null)
            {
                await _discordRepository.AddDiscordUserAsync(discordObject);
            }
            else
            {
                _logger.LogError($"Malformed or missing Discord response for user ID {DiscordId}");
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
        }
    }
}