namespace DataAccessLibrary.Data;

using System.Threading.Tasks;
using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Service to fetch and transform the Postgres data to read/write fantasy draft data for the webapi
/// example is to fetch all of the current scores, vs adding a new draft. Controllers should handle none of the business
/// logic 
/// </summary>
public class FantasyMatchPlayerRepository : IFantasyMatchPlayerRepository
{
    private readonly ILogger<FantasyMatchPlayerRepository> _logger;
    private AghanimsFantasyContext _dbContext;
    public FantasyMatchPlayerRepository(ILogger<FantasyMatchPlayerRepository> logger, AghanimsFantasyContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<List<FantasyMatchPlayer>> GetNotGcDetailParsed(int takeAmount)
    {
        _logger.LogInformation($"Getting Fantasy Match Players not GC Match Detail Parsed");

        var matchDetailsToParse = _dbContext.FantasyMatchPlayers.Where(
                fm => !fm.GcMetadataPlayerParsed && _dbContext.GcMatchMetadata.Any(md => md.MatchId == fm.FantasyMatchId))
                .Take(takeAmount);

        _logger.LogDebug($"GetFantasyMatchPlayersNotGcDetailParsed SQL Query: {matchDetailsToParse.ToQueryString()}");

        return await matchDetailsToParse.ToListAsync();
    }

    public async Task<FantasyMatchPlayer?> GetByIdAsync(int FantasyMatchPlayerId)
    {
        _logger.LogInformation($"Fetching Single Fantasy Match Player {FantasyMatchPlayerId}");

        return await _dbContext.FantasyMatchPlayers.FindAsync(FantasyMatchPlayerId);
    }

    public async Task<IEnumerable<FantasyMatchPlayer>> GetAllAsync()
    {
        _logger.LogInformation($"Get Fantasy Match Players");

        return await _dbContext.FantasyMatchPlayers.ToListAsync();
    }

    public async Task AddAsync(FantasyMatchPlayer addFantasyMatchPlayer)
    {
        _logger.LogInformation($"Adding new Fantasy Match Player {addFantasyMatchPlayer.AccountId} for Match {addFantasyMatchPlayer.FantasyMatchId}");

        await _dbContext.FantasyMatchPlayers.AddAsync(addFantasyMatchPlayer);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task DeleteAsync(FantasyMatchPlayer deleteFantasyMatchPlayer)
    {
        _logger.LogInformation($"Removing Fantasy Match Player {deleteFantasyMatchPlayer.Id}");

        _dbContext.FantasyMatchPlayers.Remove(deleteFantasyMatchPlayer);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task UpdateAsync(FantasyMatchPlayer updateFantasyMatchPlayer)
    {
        _logger.LogInformation($"Updating Fantasy Match Player {updateFantasyMatchPlayer.Id}");

        _dbContext.Entry(updateFantasyMatchPlayer).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task UpdateRangeAsync(IEnumerable<FantasyMatchPlayer> updateFantasyMatchPlayers)
    {
        _logger.LogInformation($"Updating {updateFantasyMatchPlayers.Count()} Fantasy Match Players");

        _dbContext.FantasyMatchPlayers.UpdateRange(updateFantasyMatchPlayers);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task<List<FantasyPlayerPoints>> GetFantasyPlayerPointsByMatchAsync(long MatchId)
    {
        return await _dbContext.FantasyPlayerPointsView
            .Where(fppv => fppv.FantasyMatchPlayer!.Match!.MatchId == MatchId)
            .Include(fppv => fppv.FantasyMatchPlayer)
            .ToListAsync();
    }

    public async Task<List<FantasyPlayerPoints>> GetFantasyPlayerPointsByMatchesAsync(IEnumerable<long> MatchIds)
    {
        return await _dbContext.FantasyPlayerPointsView
            .Where(fppv => MatchIds.Any(mi => mi == fppv.FantasyMatchPlayer!.Match!.MatchId))
            .Include(fppv => fppv.FantasyMatchPlayer)
            .ToListAsync();
    }
}