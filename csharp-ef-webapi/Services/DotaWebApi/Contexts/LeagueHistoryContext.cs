using System.Collections.Immutable;
using csharp_ef_webapi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace csharp_ef_webapi.Services;
internal class LeagueHistoryContext : DotaOperationContext
{
    private static readonly string AppendedApiPath = "GetMatchHistory/v1";
    // private readonly DotaWebApiService _apiService;
    private readonly AghanimsFantasyContext _dbContext;
    private readonly ILogger<LeagueHistoryContext> _logger;
    private readonly HttpClient _httpClient;

    // Our match history list: just going to use a locked list, contention should be very low
    private readonly List<MatchHistory> _matches = new List<MatchHistory>();

    // Status values
    private int _remainingLeagues = 0;
    private int _startedLeagues = 0;
    private int _totalLeagues = 0;
    private int _remainingMatches = 0;

    public LeagueHistoryContext(ILogger<LeagueHistoryContext> logger, HttpClient httpClient, IServiceScope scope, Config config)
        : base(scope, config)
    {
        _dbContext = scope.ServiceProvider.GetRequiredService<AghanimsFantasyContext>();
        _logger = logger;
        _httpClient = httpClient;
    }

    protected override async Task OperateAsync(CancellationToken cancellationToken)
    {
        Dictionary<string, string> query = new Dictionary<string, string>();

        var leagues = _dbContext.Leagues.Where(l => l.isActive).ToArray();
        var length = leagues.Length;

        // Knowing the length triggers a lot of stuff
        _totalLeagues = length;
        Volatile.Write(ref _remainingLeagues, length);
        _logger.LogInformation($"Fetching league matches for {length} leagues");
        Task[] tasks = new Task[length];

        for (int i = 0; i < length; i++)
        {
            var league = leagues[i];

            tasks[i] = GetMatchHistoryAsync(league.id, cancellationToken);
        }

        await Task.WhenAll(tasks);

        // Going to assume that an ImmutableSortedSet is the fastest to do Contains()
        var knownHeroes = _dbContext.Heroes.Select(x => x.Id).ToImmutableSortedSet();
        bool triedHeroFetch = false;

        // _matches is safe to use here, because this location is guaranteed to be the only user
        foreach (MatchHistory match in _matches)
        {
            if (!_dbContext.MatchHistory.Any(m => m.MatchId == match.MatchId))
            {
                // Set Players Match IDs since it's not in json
                foreach (MatchHistoryPlayer player in match.Players)
                {
                    while (!knownHeroes.Contains(player.HeroId))
                    {
                        if (!triedHeroFetch)
                        {
                            // TODO: await _apiService.StartOperation<HeroesContext>(cancellationToken, true);
                            knownHeroes = _dbContext.Heroes.Select(x => x.Id).ToImmutableSortedSet();
                            triedHeroFetch = true;
                            continue;
                        }

                        // TODO: Clarify
                        throw new Exception("Hero does not exist.");
                    }

                    player.MatchId = match.MatchId;
                }
                _dbContext.MatchHistory.Add(match);
            }
        }
        
        await _dbContext.SaveChangesAsync();

        // TODO: await _apiService.StartOperation<MatchDetailsContext>(cancellationToken, true);

        _logger.LogInformation($"League Match History fetch done");
    }

    private async Task GetMatchHistoryAsync(int leagueId, CancellationToken cancellationToken)
    {
        UriBuilder uriBuilder = new UriBuilder(_config.BaseUri);
        uriBuilder.AppendPath(AppendedApiPath);

        Dictionary<string, string> query = new Dictionary<string, string>(_config.BaseQuery);
        query["league_id"] = leagueId.ToString();
        query["matches_requested"] = "100";

        int resultsPrevious = 0;
        int resultsRemaining;
        bool started = false;
        long? startMatchId = null;

        do
        {
            if (startMatchId.HasValue)
            {
                query["start_at_match_id"] = startMatchId.Value.ToString();
            }

            uriBuilder.SetQuery(query);

            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

            await WaitNextTaskScheduleAsync(cancellationToken);
            HttpResponseMessage response = await _httpClient.SendAsync(httpRequest);
            _logger.LogInformation($"Request submitted at {DateTime.Now.Ticks}");
            response.EnsureSuccessStatusCode();

            JToken responseRawJToken = JToken.Parse(await response.Content.ReadAsStringAsync());

            // Read and deserialize the matches from the json response
            JToken responseObject = responseRawJToken["result"] ?? "{}";
            resultsRemaining = (responseObject["results_remaining"] ?? 0).Value<int>();
            JToken matchesJson = responseObject["matches"] ?? "[]";

            List<MatchHistory> matchResponse = JsonConvert.DeserializeObject<List<MatchHistory>>(matchesJson.ToString()) ?? new List<MatchHistory>();
            
            lock (_matches)
            {
                _matches.AddRange(matchResponse);
            }

            foreach (MatchHistory match in matchResponse)
            {
                // Steam returns the matches descending so we want the min then -1
                var matchId = match.MatchId;
                startMatchId = Math.Min(startMatchId ?? long.MaxValue, matchId);

                match.LeagueId = leagueId;
            }

            // -1 so we don't reprocess the same ending match ID twice
            startMatchId--;

            var resultChange = resultsRemaining - resultsPrevious;
            if (resultChange != 0)
            {
                Interlocked.Add(ref _remainingMatches, resultChange);
            }
            if (!started)
            {
                Interlocked.Increment(ref _startedLeagues);
                started = true;
            }

            resultsPrevious = resultsRemaining;
        } while (resultsRemaining > 0);

        Interlocked.Decrement(ref _remainingLeagues);

        // // If we want to do something when the process is done, we can use this instead
        // if (Interlocked.Decrement(ref _remainingLeagues) == 0)
        // {
        // }
    }

    public string GetLeagueFetchStatus()
    {
        Interlocked.MemoryBarrier();

        int totalLeagues = _totalLeagues;
        int completeLeagues = totalLeagues - _remainingLeagues;
        return $"{completeLeagues} of {totalLeagues} leagues fetched (found {_remainingMatches} matches remaining from {_startedLeagues} leagues)";
    }
}
