namespace csharp_ef_data_loader.Services;

using DataAccessLibrary.Data;
using DataAccessLibrary.Models.WebApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

internal class SteamClientMatchDetailsContext : DotaOperationContext
{
    private readonly ILogger<SteamClientMatchDetailsContext> _logger;
    private readonly DotaClient _dotaClient;
    private readonly WebApiRepository _webApiRepository;
    private readonly GameCoordinatorRepository _gameCoordinatorRepository;

    public SteamClientMatchDetailsContext(
            ILogger<SteamClientMatchDetailsContext> logger,
            WebApiRepository webApiRepository,
            GameCoordinatorRepository gameCoordinatorRepository,
            IServiceScope scope,
            Config config)
        : base(scope, config)
    {
        _webApiRepository = webApiRepository;
        _gameCoordinatorRepository = gameCoordinatorRepository;
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

            List<MatchHistory> matchesWithoutDetails = await _webApiRepository.GetMatchHistoriesNotInGcMatches(50);

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
                        await _gameCoordinatorRepository.UpsertGcDotaMatch(matchDetails);
                    }
                }

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
