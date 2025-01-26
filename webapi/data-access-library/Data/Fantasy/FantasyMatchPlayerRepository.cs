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

    public IQueryable<FantasyMatchPlayer> GetQueryable()
    {
        return _dbContext.FantasyMatchPlayers;
    }

    public async Task<FantasyMatchPlayer?> GetByIdAsync(int FantasyMatchPlayerId)
    {
        _logger.LogDebug($"Fetching Single Fantasy Match Player {FantasyMatchPlayerId}");

        return await _dbContext.FantasyMatchPlayers.FindAsync(FantasyMatchPlayerId);
    }

    public async Task<List<FantasyMatchPlayer>> GetAllAsync()
    {
        _logger.LogDebug($"Get Fantasy Match Players");

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
}