using System.Collections.Immutable;
using csharp_ef_webapi.Data;
using SteamKit2.GC.Dota.Internal;

namespace csharp_ef_webapi.Services;
internal class SteamClientMatchDetailsContext : DotaOperationContext
{
    private readonly AghanimsFantasyContext _dbContext;
    private readonly ILogger<SteamClientMatchDetailsContext> _logger;
    private readonly DotaClient _dotaClient;

    public SteamClientMatchDetailsContext(ILogger<SteamClientMatchDetailsContext> logger, IServiceScope scope, Config config)
        : base(scope, config)
    {
        _dbContext = scope.ServiceProvider.GetRequiredService<AghanimsFantasyContext>();
        _logger = logger;
        _dotaClient = scope.ServiceProvider.GetRequiredService<DotaClient>();
    }

    protected override async Task OperateAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_dotaClient.IsReady())
            {
                // Dota client should be disconnected when task starts, stop it if something is in limbo
                _dotaClient.Disconnect();
            }

            _dotaClient.Connect();
            // Waiting for the initial client connection to steam/dota
            _dotaClient.Wait();

            // Find all the match histories without match detail rows and add tasks to fetch them all
            ImmutableSortedSet<ulong> knownMatchHistories = _dbContext.MatchHistory.Select(x => (ulong)x.MatchId).ToImmutableSortedSet();
            ImmutableSortedSet<ulong> knownMatchDetails = _dbContext.GcDotaMatches.Where(dm => dm.replay_state == CMsgDOTAMatch.ReplayState.REPLAY_AVAILABLE).Select(x => x.match_id).ToImmutableSortedSet();

            List<ulong> matchesWithoutDetails = knownMatchHistories.Except(knownMatchDetails).Take(50).ToList();

            if (matchesWithoutDetails.Count() > 0)
            {
                var length = matchesWithoutDetails.Count;

                _logger.LogInformation($"Fetching {matchesWithoutDetails.Count()} new match details.");

                for (int i = 0; i < length; i++)
                {
                    var matchId = matchesWithoutDetails[i];

                    var matchDetails = _dotaClient.RequestMatchDetails(matchId);

                    if (matchDetails != null)
                    {
                        if (_dbContext.GcDotaMatches.Any(gdm => gdm.match_id == matchDetails.match_id))
                        {
                            _dbContext.GcDotaMatches.Update(matchDetails);
                        }
                        else
                        {
                            _dbContext.GcDotaMatches.Add(matchDetails);
                        }

                    }
                }

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Steam Client Match Details fetch done");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred: {ex.Message}");
        }
        finally
        {
            // Close connection between jobs so we don't get into weird game coordinator downtime issues
            _dotaClient.Disconnect();
        }
    }
}
