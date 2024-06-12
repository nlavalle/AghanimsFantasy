using System.Net;
using csharp_ef_webapi.Data;
using csharp_ef_webapi.Models.Discord;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace csharp_ef_webapi.Services;
public class DiscordWebApiService
{
    private readonly string _discordToken;
    private readonly HttpClient _httpClient;
    private readonly ILogger<DiscordWebApiService> _logger;
    private readonly AghanimsFantasyContext _dbContext;

    public DiscordWebApiService(ILogger<DiscordWebApiService> logger, AghanimsFantasyContext dbContext, HttpClient httpClient)
    {
        _logger = logger;
        _dbContext = dbContext;
        _httpClient = httpClient;

        _discordToken = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN") ?? "";

        _logger.LogInformation("Discord WebApi Service constructed");
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
                _dbContext.DiscordUsers.Add(discordObject);
                await _dbContext.SaveChangesAsync();
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