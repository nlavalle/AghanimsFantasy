namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models.GameCoordinator;
using SteamKit2.GC.Dota.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DataAccessLibrary.Models.Fantasy;

/// <summary>
/// Service to fetch and transform the Postgres data to read/write fantasy draft data for the webapi
/// example is to fetch all of the current scores, vs adding a new draft. Controllers should handle none of the business
/// logic 
/// </summary>
public class GameCoordinatorRepository
{
    private readonly ILogger<GameCoordinatorRepository> _logger;
    private AghanimsFantasyContext _dbContext;
    public GameCoordinatorRepository(ILogger<GameCoordinatorRepository> logger, AghanimsFantasyContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    #region CMsgDotaMatch

    #endregion CMsgDotaMatch

    #region GcMatchMetadata
    public async Task AddGcMatchMetadata(GcMatchMetadata newGcMetadata)
    {
        _logger.LogInformation($"Adding GcMatchMetadata {newGcMetadata.MatchId}");
        await _dbContext.GcMatchMetadata.AddAsync(newGcMetadata);
        await _dbContext.SaveChangesAsync();
    }
    #endregion GcMatchMetadata

    public async Task<GcMatchMetadataPlayer> GetMatchMetadataPlayerAsync(FantasyMatchPlayer fantasyMatchPlayer)
    {
        _logger.LogInformation($"Getting Match Metadata Player for FantasyMatchPlayer: {fantasyMatchPlayer.Id}");

        var matchMetadataPlayerQuery = _dbContext.GcMatchMetadata
            .Where(md => md.MatchId == fantasyMatchPlayer.Match.MatchId)
            .SelectMany(md => md.Teams,
            (left, right) => new { Match = left, Team = right })
            .SelectMany(mdt => mdt.Team.Players,
            (left, right) => new { Match = left.Match, Team = left.Team, Player = right })
            .Where(mdp => mdp.Match.MatchId == fantasyMatchPlayer.Match.MatchId && mdp.Player.PlayerSlot == fantasyMatchPlayer.PlayerSlot)
            .Select(result => result.Player);

        _logger.LogDebug($"GetMatchMetadataPlayerAsync SQL Query: {matchMetadataPlayerQuery.ToQueryString()}");

        return await matchMetadataPlayerQuery.FirstAsync();
    }

    public async Task<GcMatchMetadata?> GetMatchMetadataAsync(long MatchId)
    {
        _logger.LogInformation($"Getting Match Metadata for Match: {MatchId}");

        var matchMetadataQuery = _dbContext.GcMatchMetadata
                .Where(md => md.MatchId == MatchId);

        _logger.LogDebug($"Match Metadata Query: {matchMetadataQuery.ToQueryString()}");

        return await matchMetadataQuery.FirstOrDefaultAsync();
    }
}