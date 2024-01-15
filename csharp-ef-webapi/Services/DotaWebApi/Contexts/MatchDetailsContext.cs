using System.Collections.Immutable;
using csharp_ef_webapi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace csharp_ef_webapi.Services;
internal class MatchDetailsContext : DotaOperationContext
{
    private static readonly string AppendedApiPath = "GetMatchDetails/v1";
    // private readonly DotaWebApiService _apiService;
    private readonly AghanimsFantasyContext _dbContext;
    private readonly ILogger<MatchDetailsContext> _logger;
    private readonly HttpClient _httpClient;

    // Our match history list: just going to use a locked list, contention should be very low
    private readonly List<MatchDetail> _matches = new List<MatchDetail>();

    // Status values
    private int _remainingMatches = 0;
    private int _startedMatches = 0;
    private int _totalMatches = 0;

    public MatchDetailsContext(ILogger<MatchDetailsContext> logger, HttpClient httpClient, IServiceScope scope, Config config)
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
                var length = matchesWithoutDetails.Count;

                // Knowing the length triggers a lot of stuff
                _totalMatches = length;
                Volatile.Write(ref _remainingMatches, length);
                _logger.LogInformation($"Fetching {matchesWithoutDetails.Count()} new match details.");

                List<Task<MatchDetail?>> fetchMatchDetailsTasks = new List<Task<MatchDetail?>>();

                Task[] tasks = new Task[length];

                for (int i = 0; i < length; i++)
                {
                    var match = matchesWithoutDetails[i];

                    tasks[i] = GetMatchDetailsAsync(match.MatchId, cancellationToken);
                }

                await Task.WhenAll(tasks);

                // Going to assume that an ImmutableSortedSet is the fastest to do Contains()
                var knownMatches = _dbContext.MatchDetails.Select(x => x.MatchId).ToImmutableSortedSet();

                foreach (MatchDetail matchDetail in _matches)
                {
                    if (!knownMatches.Contains(matchDetail.MatchId))
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
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Missing match details fetch done");

            }
        }
        catch (Exception ex)
        {
            // Handle exceptions here
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private async Task GetMatchDetailsAsync(long matchId, CancellationToken cancellationToken)
    {
        UriBuilder uriBuilder = new UriBuilder(_config.BaseUri);
        uriBuilder.AppendPath(AppendedApiPath);

        Dictionary<string, string> query = new Dictionary<string, string>(_config.BaseQuery);
        query["match_id"] = matchId.ToString();

        uriBuilder.SetQuery(query);

        HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

        await WaitNextTaskScheduleAsync(cancellationToken);
        HttpResponseMessage response = await _httpClient.SendAsync(httpRequest);
        _logger.LogInformation($"Request submitted at {DateTime.Now.Ticks}");
        response.EnsureSuccessStatusCode();

        JToken responseRawJToken = JToken.Parse(await response.Content.ReadAsStringAsync());

        // Read and deserialize the matches from the json response
        JToken responseObject = responseRawJToken["result"] ?? "{}";

        MatchDetail? matchResponse = JsonConvert.DeserializeObject<MatchDetail>(responseObject.ToString());

        if (matchResponse != null)
        {
            lock (_matches)
            {
                _matches.Add(matchResponse);
            }
        }

        Interlocked.Add(ref _startedMatches, 1);
        Interlocked.Decrement(ref _remainingMatches);

        // // If we want to do something when the process is done, we can use this instead
        // if (Interlocked.Decrement(ref _remainingLeagues) == 0)
        // {
        // }
    }

    public string GetMatchDetailFetchStatus()
    {
        Interlocked.MemoryBarrier();

        int totalMatches = _totalMatches;
        int completeLeagues = totalMatches - _remainingMatches;
        return $"{completeLeagues} of {totalMatches} missing match details fetched";
    }
}
