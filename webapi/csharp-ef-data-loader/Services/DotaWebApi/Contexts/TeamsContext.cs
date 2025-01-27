namespace csharp_ef_data_loader.Services;

using DataAccessLibrary.Data;
using DataAccessLibrary.Models.ProMetadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

internal class TeamsContext : DotaOperationContext
{
    private static readonly string AppendedApiPath = "GetTeamInfoByTeamID/v1";
    private readonly ILogger<TeamsContext> _logger;
    private readonly HttpClient _httpClient;
    private readonly AghanimsFantasyContext _dbContext;

    public TeamsContext(
            ILogger<TeamsContext> logger,
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
            _logger.LogInformation($"Fetching heroes");
            List<long> newTeams = await _dbContext.MatchHistory
            .Select(mh => mh.RadiantTeamId)
            .Union(_dbContext.MatchHistory.Select(mh => mh.DireTeamId))
            .Distinct()
            .Where(t => t != 0)
            .Where(
                mh => !_dbContext.Teams
                    .Select(t => t.Id)
                    .Contains(mh))
            .ToListAsync();

            if (newTeams.Count() > 0)
            {
                _logger.LogInformation($"Fetching {newTeams.Count()} new team details.");
                List<Task<List<Team>>> fetchTeamsTasks = new List<Task<List<Team>>>();

                foreach (long teamId in newTeams)
                {
                    fetchTeamsTasks.Add(GetTeamAsync(teamId, cancellationToken));
                }

                await Task.WhenAll(fetchTeamsTasks);

                foreach (Team team in fetchTeamsTasks.SelectMany(t => t.Result).ToList())
                {
                    await _dbContext.Teams.AddAsync(team);
                }
            }

            _logger.LogInformation($"Missing team details fetch done");
        }
        catch (Exception ex)
        {
            // Handle exceptions here
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }


    // GetTeamInfoByTeamID doesn't return the team ID in the response so we have to request teams_requested=1
    // to make sure we're getting the team we want
    private async Task<List<Team>> GetTeamAsync(long teamId, CancellationToken cancellationToken)
    {
        UriBuilder uriBuilder = new UriBuilder(_config.BaseUri);
        uriBuilder.AppendPath(AppendedApiPath);

        Dictionary<string, string> query = new Dictionary<string, string>(_config.ConfigSettings);
        query["start_at_team_id"] = teamId.ToString();
        query["teams_requested"] = "1";

        uriBuilder.SetQuery(query);

        HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

        await WaitNextTaskScheduleAsync(cancellationToken);

        HttpResponseMessage response = await _httpClient.SendAsync(httpRequest);
        _logger.LogInformation($"Request submitted at {DateTime.Now.Ticks}");
        response.EnsureSuccessStatusCode();

        JsonDocument responseRawJDocument = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        // Read and deserialize the matches from the json response
        JsonElement resultElement;
        if (!responseRawJDocument.RootElement.TryGetProperty("result", out resultElement)) throw new Exception("Result element not found in response");
        JsonElement teamsElement;
        if (!resultElement.TryGetProperty("teams", out teamsElement)) throw new Exception("Teams element not found in result response");

        List<Team> teamResponse = JsonSerializer.Deserialize<List<Team>>(teamsElement.GetRawText()) ?? throw new Exception("Unable to deserialize response json to Teams");

        foreach (Team team in teamResponse)
        {
            team.Id = teamId;
        }

        return teamResponse;
    }
}
