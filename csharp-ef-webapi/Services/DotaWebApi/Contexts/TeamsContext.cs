using csharp_ef_webapi.Data;
using csharp_ef_webapi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace csharp_ef_webapi.Services;
internal class TeamsContext : DotaOperationContext
{
    private static readonly string AppendedApiPath = "GetTeamInfoByTeamID/v1";
    // private readonly DotaWebApiService _apiService;
    private readonly AghanimsFantasyContext _dbContext;
    private readonly ILogger<TeamsContext> _logger;
    private readonly HttpClient _httpClient;

    public TeamsContext(ILogger<TeamsContext> logger, HttpClient httpClient, IServiceScope scope, Config config)
        : base(scope, config)
    {
        _dbContext = scope.ServiceProvider.GetRequiredService<AghanimsFantasyContext>();
        _logger = logger;
        _httpClient = httpClient;
    }

    protected override async Task OperateAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"Fetching heroes");

                List<long> distinctTeams = _dbContext.MatchHistory
                    .Select(mh => mh.RadiantTeamId)
                    .Union(_dbContext.MatchHistory.Select(mh => mh.DireTeamId))
                    .Distinct()
                    .Where(t => t != 0)
                    .ToList();

                List<long> newTeams = distinctTeams
                    .Except(_dbContext.Teams.Select(t => t.Id)).ToList();

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
                    if (_dbContext.Teams.FirstOrDefault(t => t.Id == team.Id) == null)
                    {

                        _dbContext.Teams.Add(team);
                    }
                }
                }
                await _dbContext.SaveChangesAsync();

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

        JToken responseRawJToken = JToken.Parse(await response.Content.ReadAsStringAsync());

        // Read and deserialize the matches from the json response
        JToken responseObject = responseRawJToken["result"] ?? "{}";
        JToken teams = responseObject["teams"] ?? "{}";

        List<Team> teamResponse = JsonConvert.DeserializeObject<List<Team>>(teams.ToString()) ?? new List<Team>();

        foreach (Team team in teamResponse)
        {
            team.Id = teamId;
        }

        return teamResponse;
    }
}
