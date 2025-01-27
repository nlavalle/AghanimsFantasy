namespace csharp_ef_data_loader.Services;

using DataAccessLibrary.Data;
using DataAccessLibrary.Models.WebApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SteamKit2.GC.Dota.Internal;

internal class SteamClientMatchDetailsContext : DotaOperationContext
{
    private readonly ILogger<SteamClientMatchDetailsContext> _logger;
    private readonly DotaClient _dotaClient;
    private readonly AghanimsFantasyContext _dbContext;

    public SteamClientMatchDetailsContext(
            ILogger<SteamClientMatchDetailsContext> logger,
            AghanimsFantasyContext dbContext,
            IServiceScope scope,
            Config config)
        : base(scope, config)
    {
        _dbContext = dbContext;
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

            if (!_dotaClient.IsReady())
            {
                // Connection timed out or failed
                _logger.LogError($"Was unable to connect the steam client");
                return;
            }

            // Find all the match histories without match detail rows and add tasks to fetch them all

            List<MatchHistory> matchesWithoutDetails = await GetNotInGcMatches(50);

            if (matchesWithoutDetails.Count() > 0)
            {
                var length = matchesWithoutDetails.Count;

                _logger.LogInformation($"Fetching {matchesWithoutDetails.Count()} new match details.");

                for (int i = 0; i < length; i++)
                {
                    var match = matchesWithoutDetails[i];

                    var matchDetails = _dotaClient.RequestMatchDetails((ulong)match.MatchId);

                    if (matchDetails != null)
                    {
                        if (await _dbContext.GcDotaMatches.FindAsync(matchDetails.match_id) != null)
                        {
                            _dbContext.Entry(matchDetails).State = EntityState.Modified;
                        }
                        else
                        {
                            await _dbContext.GcDotaMatches.AddAsync(matchDetails);
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

    private async Task<List<MatchHistory>> GetNotInGcMatches(int takeAmount)
    {
        _logger.LogInformation($"Getting new Match Histories not loaded into Gc Dota Matches");

        var matchesNotInGcQuery = _dbContext.MatchHistory
            .Where(
                mh => !_dbContext.GcDotaMatches
                    .Where(dm => dm.replay_state == CMsgDOTAMatch.ReplayState.REPLAY_AVAILABLE)
                    .Select(gdm => gdm.match_id)
                    .Contains((ulong)mh.MatchId))
                .OrderBy(mh => mh.MatchId)
                .Take(takeAmount);

        _logger.LogDebug($"GetMatchHistoriesNotInGcMatches SQL Query: {matchesNotInGcQuery.ToQueryString()}");

        return await matchesNotInGcQuery.ToListAsync();
    }
}
