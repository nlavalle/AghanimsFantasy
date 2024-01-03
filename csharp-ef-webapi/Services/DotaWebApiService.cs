using csharp_ef_webapi.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace csharp_ef_webapi.Services;
public class DotaWebApiService : BackgroundService
{
    private readonly string _baseApiUrl;
    private readonly string _econApiUrl;
    private readonly string _steamKey;
    private readonly HttpClient _httpClient;
    private readonly ILogger<DotaWebApiService> _logger;
    private const long _delayBetweenRequests = 10_000_000 / 2; // Constant for ticks in a second divided by 2
    IConfiguration _configuration;
    IServiceProvider _serviceProvider;
    private Timer leagueTimer;
    private Timer matchDetailsTimer;
    private Timer teamsTimer;
    private Timer heroesTimer;

    public DotaWebApiService(ILogger<DotaWebApiService> logger, IConfiguration configuration, IServiceProvider serviceProvider, HttpClient httpClient)
    {
        _logger = logger;
        _configuration = configuration;
        _serviceProvider = serviceProvider;

        _baseApiUrl = _configuration.GetSection("DotaWebApi").GetValue<string>("BaseUrl");
        _econApiUrl = _configuration.GetSection("DotaWebApi").GetValue<string>("EconUrl");
        _steamKey = Environment.GetEnvironmentVariable("STEAM_KEY") ?? "";

        _httpClient = httpClient;
        _logger.LogInformation("Dota WebApi Service started");

    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        // We're running this once a day because the live games should get us the updates
        leagueTimer = new Timer(GetLeagueHistoryDataCallback, null, TimeSpan.Zero, TimeSpan.FromMinutes(30));

        // We're running this more frequently because it should typically find no new match details and skip
        matchDetailsTimer = new Timer(GetMatchDetailDataCallback, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

        // Heroes update like once a year
        heroesTimer = new Timer(GetHeroesDataCallback, null, TimeSpan.Zero, TimeSpan.FromDays(1));

        // Teams shouldn't change much but once a day
        teamsTimer = new Timer(GetTeamsDataCallback, null, TimeSpan.Zero, TimeSpan.FromDays(1));

        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async void GetLeagueHistoryDataCallback(object? state)
    {
        // Get all the Match Histories for the leagues, this is intended to be run first
        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                AghanimsFantasyContext _dbContext = scope.ServiceProvider.GetRequiredService<AghanimsFantasyContext>();

                List<League> leagues = _dbContext.Leagues.Where(l => l.isActive).ToList();
                _logger.LogInformation($"Fetching league matches for {leagues.Count()} leagues");

                List<Task<List<MatchHistory>>> fetchMatchHistoryTasks = new List<Task<List<MatchHistory>>>();

                foreach (League league in leagues)
                {
                    fetchMatchHistoryTasks.Add(GetMatchHistoryAsync(league.id));
                }

                await Task.WhenAll(fetchMatchHistoryTasks);

                foreach (MatchHistory match in fetchMatchHistoryTasks.SelectMany(t => t.Result).ToList())
                {
                    if (_dbContext.MatchHistory.FirstOrDefault(m => m.MatchId == match.MatchId) == null)
                    {
                        // Set Players Match IDs since it's not in json
                        foreach (MatchHistoryPlayer player in match.Players)
                        {
                            player.MatchId = match.MatchId;
                        }
                        _dbContext.MatchHistory.Add(match);
                    }
                }
                await _dbContext.SaveChangesAsync();
            }

            _logger.LogInformation($"League Match History fetch done");

            // Call the match detail manually for each job of this since we might have new matches to fetch
            GetMatchDetailDataCallback(null);
        }
        catch (Exception ex)
        {
            // Handle exceptions here
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private async void GetMatchDetailDataCallback(object? state)
    {
        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                AghanimsFantasyContext _dbContext = scope.ServiceProvider.GetRequiredService<AghanimsFantasyContext>();

                // Find all the match histories without match detail rows and add tasks to fetch them all
                List<MatchHistory> matchesWithoutDetails = _dbContext.MatchHistory
                    .GroupJoin(
                        _dbContext.MatchDetails,
                        match => match.MatchId,
                        details => details.MatchId,
                        (match, details) => new { Match = match, Details = details })
                    .SelectMany(
                        m => m.Details.DefaultIfEmpty(),
                        (match, details) => new { match.Match, Details = details }
                    )
                    .Where(joinResult => joinResult.Details == null)
                    .Select(joinResult => joinResult.Match)
                    .ToList();

                if (matchesWithoutDetails.Count() > 0)
                {
                    _logger.LogInformation($"Fetching {matchesWithoutDetails.Count()} new match details.");
                    List<Task<MatchDetail?>> fetchMatchDetailsTasks = new List<Task<MatchDetail?>>();

                    foreach (MatchHistory match in matchesWithoutDetails)
                    {
                        fetchMatchDetailsTasks.Add(GetMatchDetailsAsync(match.MatchId));
                    }

                    await Task.WhenAll(fetchMatchDetailsTasks);

                    foreach (MatchDetail? matchDetail in fetchMatchDetailsTasks.Select(t => t.Result))
                    {
                        if (matchDetail != null)
                        {
                            if (_dbContext.MatchDetails.FirstOrDefault(m => m.MatchId == matchDetail.MatchId) == null)
                            {
                                // Set PicksBans Match IDs since it's not in json
                                foreach (MatchDetailsPicksBans picksBans in matchDetail.PicksBans)
                                {
                                    picksBans.MatchId = matchDetail.MatchId;
                                }

                                // Set Players Match IDs since it's not in json
                                foreach (MatchDetailsPlayer player in matchDetail.Players)
                                {
                                    player.MatchId = matchDetail.MatchId;
                                    // Set Players Ability Upgrade PlayerIDs since it's not in json
                                    foreach (MatchDetailsPlayersAbilityUpgrade abilities in player.AbilityUpgrades)
                                    {
                                        abilities.PlayerId = player.Id;
                                    }
                                }

                                _dbContext.MatchDetails.Add(matchDetail);
                            }
                        }
                    }
                    await _dbContext.SaveChangesAsync();

                    _logger.LogInformation($"Missing match details fetch done");

                }
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions here
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private async void GetHeroesDataCallback(object? state)
    {
        try
        {
            _logger.LogInformation($"Fetching heroes");

            using (var scope = _serviceProvider.CreateScope())
            {
                AghanimsFantasyContext _dbContext = scope.ServiceProvider.GetRequiredService<AghanimsFantasyContext>();

                List<Hero> heroes = new List<Hero>();
                heroes = await GetHeroesAsync();

                foreach (Hero hero in heroes)
                {
                    if (_dbContext.Heroes.FirstOrDefault(h => h.Id == hero.Id) == null)
                    {
                        _dbContext.Heroes.Add(hero);
                    }
                    else
                    {
                        Hero updateHero = _dbContext.Heroes.First(h => h.Id == hero.Id);
                        updateHero.Name = hero.Name;
                        _dbContext.Heroes.Update(updateHero);
                    }
                }
                await _dbContext.SaveChangesAsync();
            }

            _logger.LogInformation($"Hero fetch done");
        }
        catch (Exception ex)
        {
            // Handle exceptions here
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private async void GetTeamsDataCallback(object? state)
    {
        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                AghanimsFantasyContext _dbContext = scope.ServiceProvider.GetRequiredService<AghanimsFantasyContext>();
                // Find all the distinct teams from the league match histories
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
                        fetchTeamsTasks.Add(GetTeamAsync(teamId));
                    }

                    await Task.WhenAll(fetchTeamsTasks);

                    foreach (Team team in fetchTeamsTasks.SelectMany(t => t.Result).ToList())
                    {
                        if (_dbContext.Teams.FirstOrDefault(t => t.Id == team.Id) == null)
                        {

                            _dbContext.Teams.Add(team);
                        }
                    }
                    await _dbContext.SaveChangesAsync();

                }

                _logger.LogInformation($"Missing team details fetch done");

            }

        }
        catch (Exception ex)
        {
            // Handle exceptions here
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }


    private async Task<List<MatchHistory>> GetMatchHistoryAsync(int leagueId)
    {
        List<MatchHistory> matches = new List<MatchHistory>();
        bool endOfLeague = false;
        long? startMatchId = null;
        while (!endOfLeague)
        {
            await RateLimiter.WaitNextTaskScheduleAsync(_delayBetweenRequests);

            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, _baseApiUrl);
            UriBuilder uriBuilder = new UriBuilder($"{_baseApiUrl}/GetMatchHistory/v1");
            uriBuilder.Query = $"key={_steamKey}&league_id={leagueId}&matches_requested=100";

            if (startMatchId.HasValue)
            {
                uriBuilder.Query += $"&start_at_match_id={startMatchId}";
            }

            httpRequest.RequestUri = uriBuilder.Uri;

            HttpResponseMessage response = await _httpClient.SendAsync(httpRequest);
            _logger.LogInformation($"Request submitted at {DateTime.Now.Ticks}");
            response.EnsureSuccessStatusCode();

            JToken responseRawJToken = JToken.Parse(await response.Content.ReadAsStringAsync());

            // Read and deserialize the matches from the json response
            JToken responseObject = responseRawJToken["result"] ?? "{}";
            int resultsRemaining = (responseObject["results_remaining"] ?? 0).Value<int>();
            JToken matchesJson = responseObject["matches"] ?? "[]";

            List<MatchHistory> matchResponse = JsonConvert.DeserializeObject<List<MatchHistory>>(matchesJson.ToString()) ?? new List<MatchHistory>();

            foreach (MatchHistory match in matchResponse)
            {
                match.LeagueId = leagueId;
            }

            matches.AddRange(matchResponse.Where(m => m.MatchId != startMatchId));

            if (resultsRemaining > 0)
            {
                // Response returns latest matches first
                startMatchId = matchResponse.OrderByDescending(m => m.MatchId).Last().MatchId;
            }
            else
            {
                endOfLeague = true;
            }
        }

        return matches;
    }

    private async Task<MatchDetail?> GetMatchDetailsAsync(long matchId)
    {
        await RateLimiter.WaitNextTaskScheduleAsync(_delayBetweenRequests);

        HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, _baseApiUrl);
        UriBuilder uriBuilder = new UriBuilder($"{_baseApiUrl}/GetMatchDetails/v1");
        uriBuilder.Query = $"key={_steamKey}&match_id={matchId}";

        httpRequest.RequestUri = uriBuilder.Uri;

        HttpResponseMessage response = await _httpClient.SendAsync(httpRequest);
        _logger.LogInformation($"Request submitted at {DateTime.Now.Ticks}");
        response.EnsureSuccessStatusCode();

        JToken responseRawJToken = JToken.Parse(await response.Content.ReadAsStringAsync());

        // Read and deserialize the matches from the json response
        JToken responseObject = responseRawJToken["result"] ?? "{}";

        MatchDetail? matchResponse = JsonConvert.DeserializeObject<MatchDetail>(responseObject.ToString());

        if (matchResponse != null)
        {
            matchResponse.MatchId = matchId;
        }

        return matchResponse;
    }

    private async Task<List<Hero>> GetHeroesAsync()
    {
        await RateLimiter.WaitNextTaskScheduleAsync(_delayBetweenRequests);

        HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, _econApiUrl);
        UriBuilder uriBuilder = new UriBuilder($"{_econApiUrl}/GetHeroes/v1");
        uriBuilder.Query = $"key={_steamKey}";

        httpRequest.RequestUri = uriBuilder.Uri;

        HttpResponseMessage response = await _httpClient.SendAsync(httpRequest);
        _logger.LogInformation($"Request submitted at {DateTime.Now.Ticks}");
        response.EnsureSuccessStatusCode();

        JToken responseRawJToken = JToken.Parse(await response.Content.ReadAsStringAsync());

        // Read and deserialize the matches from the json response
        JToken responseObject = responseRawJToken["result"] ?? "{}";
        JToken heroesJson = responseObject["heroes"] ?? "[]";

        List<Hero> heroesResponse = JsonConvert.DeserializeObject<List<Hero>>(heroesJson.ToString()) ?? new List<Hero>();

        return heroesResponse;
    }

    private async Task<List<Team>> GetTeamAsync(long teamId)
    // GetTeamInfoByTeamID doesn't return the team ID in the response so we have to request teams_requested=1
    // to make sure we're getting the team we want
    {
        await RateLimiter.WaitNextTaskScheduleAsync(_delayBetweenRequests);

        HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, _baseApiUrl);
        UriBuilder uriBuilder = new UriBuilder($"{_baseApiUrl}/GetTeamInfoByTeamID/v1");
        uriBuilder.Query = $"key={_steamKey}&start_at_team_id={teamId}&teams_requested=1";

        httpRequest.RequestUri = uriBuilder.Uri;

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