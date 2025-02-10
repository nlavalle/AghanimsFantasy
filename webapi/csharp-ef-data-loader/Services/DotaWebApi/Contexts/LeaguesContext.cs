namespace csharp_ef_data_loader.Services;

using DataAccessLibrary.Data;
using DataAccessLibrary.Models.ProMetadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

internal class LeaguesContext : DotaOperationContext
{
    private static readonly string AppendedApiPath = "/webapi/IDOTA2League/GetLeagueInfoList/v001";
    private readonly ILogger<LeaguesContext> _logger;
    private readonly HttpClient _httpClient;
    private readonly AghanimsFantasyContext _dbContext;

    public LeaguesContext(
            ILogger<LeaguesContext> logger,
            HttpClient httpClient,
            AghanimsFantasyContext dbContext,
            IServiceScope scope,
            Config config)
        : base(scope, config)
    {
        _logger = logger;
        _httpClient = httpClient;
        _dbContext = dbContext;
    }

    protected override async Task OperateAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"Update leagues");
            List<League> leaguesResponse = await GetLeaguesAsync(cancellationToken);
            List<League> currentLeagues = await _dbContext.Leagues.ToListAsync();
            List<League> newLeagues = leaguesResponse.Where(l => l.Tier >= 2
                && !currentLeagues.Contains(l)
                && DateTime.UnixEpoch.AddSeconds(l.StartTimestamp) >= DateTime.UtcNow)
            .ToList();

            if (newLeagues.Count() > 0)
            {
                _logger.LogInformation($"Adding {newLeagues.Count()} new leagues");
                foreach (League league in newLeagues)
                {
                    await _dbContext.Leagues.AddAsync(league);
                }

                await _dbContext.SaveChangesAsync();
            }

            _logger.LogInformation($"New league details fetch done");
        }
        catch (Exception ex)
        {
            // Handle exceptions here
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private async Task<List<League>> GetLeaguesAsync(CancellationToken cancellationToken)
    {
        UriBuilder uriBuilder = new UriBuilder(new Uri("https://www.dota2.com"));
        uriBuilder.AppendPath(AppendedApiPath);

        Dictionary<string, string> query = new Dictionary<string, string>(_config.ConfigSettings);

        HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

        await WaitNextTaskScheduleAsync(cancellationToken);

        HttpResponseMessage response = await _httpClient.SendAsync(httpRequest);
        _logger.LogInformation($"Request submitted at {DateTime.Now.Ticks}");
        response.EnsureSuccessStatusCode();

        JsonDocument responseRawJDocument = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        // Read and deserialize the matches from the json response
        // JsonElement resultElement;
        // if (!responseRawJDocument.RootElement.TryGetProperty("result", out resultElement)) throw new Exception("Result element not found in response");
        JsonElement leaguesElement;
        if (!responseRawJDocument.RootElement.TryGetProperty("infos", out leaguesElement)) throw new Exception("Teams element not found in result response");

        List<League> leagueResponse = JsonSerializer.Deserialize<List<League>>(leaguesElement.GetRawText()) ?? throw new Exception("Unable to deserialize response json to Leagues");

        foreach (League league in leagueResponse)
        {
            league.IsActive = false;
            league.IsScheduled = false;
        }

        return leagueResponse;
    }
}
