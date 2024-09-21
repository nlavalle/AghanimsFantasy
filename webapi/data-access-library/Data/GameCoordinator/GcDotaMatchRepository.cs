namespace DataAccessLibrary.Data;

using System.Threading.Tasks;
using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.ProMetadata;
using DataAccessLibrary.Models.WebApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SteamKit2.GC.Dota.Internal;

/// <summary>
/// Service to fetch and transform the Postgres data to read/write fantasy draft data for the webapi
/// example is to fetch all of the current scores, vs adding a new draft. Controllers should handle none of the business
/// logic 
/// </summary>
public class GcDotaMatchRepository : IRepository<CMsgDOTAMatch, ulong>
{
    private readonly ILogger<GcDotaMatchRepository> _logger;
    private AghanimsFantasyContext _dbContext;
    public GcDotaMatchRepository(ILogger<GcDotaMatchRepository> logger, AghanimsFantasyContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<List<(CMsgDOTAMatch, CMsgDOTAMatch.Player)>> GetNotInFantasyMatchPlayers(int takeAmount)
    {
        _logger.LogInformation($"Getting new GcDotaMatches not loaded into Fantasy Match Players");

        var newGcDotaMatchQuery = _dbContext.GcDotaMatches
                .SelectMany(match => match.players,
                (left, right) => new { Match = left, MatchPlayer = right })
                .Where(gcmp =>
                    !_dbContext.FantasyMatchPlayers.Any(fmp => fmp.Match.MatchId == (long)gcmp.Match.match_id && fmp.Account != null && fmp.Account.Id == gcmp.MatchPlayer.account_id)
                    && (gcmp.Match.match_outcome == EMatchOutcome.k_EMatchOutcome_RadVictory || gcmp.Match.match_outcome == EMatchOutcome.k_EMatchOutcome_DireVictory) // Filter out cancelled games
                )
                .Take(takeAmount);

        _logger.LogDebug($"GetGcDotaMatchesNotInFantasyMatchPlayers SQL Query: {newGcDotaMatchQuery.ToQueryString()}");

        var result = await newGcDotaMatchQuery.ToListAsync();

        return result.Select(rs => (rs.Match, rs.MatchPlayer)).ToList();
    }

    public async Task<List<CMsgDOTAMatch>> GetNotInGcMetadata(int takeAmount)
    {
        _logger.LogInformation($"Getting new GcDotaMatches not loaded into Fantasy Match Players");

        var newGcDotaMatchQuery = _dbContext.GcDotaMatches
                .Where(match => _dbContext.Leagues
                        .Where(league => league.IsActive)
                        .Select(league => league.Id)
                        .Contains((int)match.leagueid)
                )
                .Where(gcdm => gcdm.replay_state == CMsgDOTAMatch.ReplayState.REPLAY_AVAILABLE &&
                    !_dbContext.GcMatchMetadata.Any(gcmm => (ulong)gcmm.MatchId == gcdm.match_id))
                .Take(takeAmount);

        _logger.LogDebug($"GetGcDotaMatchesNotInGcMetadata SQL Query: {newGcDotaMatchQuery.ToQueryString()}");

        return await newGcDotaMatchQuery.ToListAsync();
    }

    public async Task<CMsgDOTAMatch?> GetByIdAsync(ulong MatchId)
    {
        _logger.LogInformation($"Fetching Single GcDotaMatch {MatchId}");

        return await _dbContext.GcDotaMatches.FindAsync(MatchId);
    }

    public async Task<IEnumerable<CMsgDOTAMatch>> GetAllAsync()
    {
        _logger.LogInformation($"Get GcDotaMatches");

        return await _dbContext.GcDotaMatches.ToListAsync();
    }

    public async Task AddAsync(CMsgDOTAMatch addGcDotaMatch)
    {
        _logger.LogInformation($"Adding new GcDotaMatch {addGcDotaMatch.match_id}");

        await _dbContext.GcDotaMatches.AddAsync(addGcDotaMatch);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task DeleteAsync(CMsgDOTAMatch deleteGcDotaMatch)
    {
        _logger.LogInformation($"Removing GcDotaMatch {deleteGcDotaMatch.match_id}");

        _dbContext.GcDotaMatches.Remove(deleteGcDotaMatch);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task UpdateAsync(CMsgDOTAMatch updateGcDotaMatch)
    {
        _logger.LogInformation($"Updating GcDotaMatch {updateGcDotaMatch.match_id}");

        _dbContext.Entry(updateGcDotaMatch).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        return;
    }
}