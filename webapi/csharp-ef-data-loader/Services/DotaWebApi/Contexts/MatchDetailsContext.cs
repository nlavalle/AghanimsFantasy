namespace csharp_ef_data_loader.Services;

using DataAccessLibrary.Data;
using DataAccessLibrary.Models.WebApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

internal class MatchDetailsContext : DotaOperationContext
{
    private static readonly string AppendedApiPath = "GetMatchDetails/v1";
    // private readonly DotaWebApiService _apiService;
    private readonly ILogger<MatchDetailsContext> _logger;
    private readonly HttpClient _httpClient;
    private readonly WebApiRepository _webApiRepository;

    // Our match history list: just going to use a locked list, contention should be very low
    private readonly List<MatchDetail> _matches = new List<MatchDetail>();

    // Status values
    private int _remainingMatches = 0;
    private int _startedMatches = 0;
    private int _totalMatches = 0;

    public MatchDetailsContext(
            ILogger<MatchDetailsContext> logger,
            HttpClient httpClient,
            WebApiRepository webApiRepository,
            IServiceScope scope,
            Config config
        )
        : base(scope, config)
    {
        _logger = logger;
        _httpClient = httpClient;
        _webApiRepository = webApiRepository;
    }

    protected override async Task OperateAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Find all the match histories without match detail rows and add tasks to fetch them all
            List<MatchHistory> matchesWithoutDetails = await _webApiRepository.GetMatchHistoriesNotInMatchDetails(50);

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

                foreach (MatchDetail matchDetail in _matches)
                {
                    if (await _webApiRepository.GetMatchDetailAsync(matchDetail.MatchId) == null)
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

                        await _webApiRepository.AddMatchDetailAsync(matchDetail);
                    }
                }

                _logger.LogInformation($"Missing match details fetch done");

            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred: {ex.Message}");
        }
    }

    private async Task GetMatchDetailsAsync(long matchId, CancellationToken cancellationToken)
    {
        try
        {
            UriBuilder uriBuilder = new UriBuilder(_config.BaseUri);
            uriBuilder.AppendPath(AppendedApiPath);

            Dictionary<string, string> query = new Dictionary<string, string>(_config.ConfigSettings);
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
                matchResponse.MatchId = matchId;

                lock (_matches)
                {
                    _matches.Add(matchResponse);
                }
            }

            // // If we want to do something when the process is done, we can use this instead
            // if (Interlocked.Decrement(ref _remainingLeagues) == 0)
            // {
            // }
        }
        catch (HttpRequestException badResponse)
        {
            _logger.LogWarning($"Bad response received for match: {matchId}. Full error response message: {badResponse.Message}");
        }
        finally
        {
            Interlocked.Add(ref _startedMatches, 1);
            Interlocked.Decrement(ref _remainingMatches);
        }


    }

    public string GetMatchDetailFetchStatus()
    {
        Interlocked.MemoryBarrier();

        int totalMatches = _totalMatches;
        int completeLeagues = totalMatches - _remainingMatches;
        return $"{completeLeagues} of {totalMatches} missing match details fetched";
    }
}
