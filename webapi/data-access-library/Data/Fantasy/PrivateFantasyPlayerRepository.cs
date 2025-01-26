namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models.Fantasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Service to fetch and transform the Postgres data to read/write fantasy draft data for the webapi
/// example is to fetch all of the current scores, vs adding a new draft. Controllers should handle none of the business
/// logic 
/// </summary>
public class PrivateFantasyPlayerRepository : IPrivateFantasyPlayerRepository
{
    private readonly ILogger<PrivateFantasyPlayerRepository> _logger;
    private AghanimsFantasyContext _dbContext;
    public PrivateFantasyPlayerRepository(ILogger<PrivateFantasyPlayerRepository> logger, AghanimsFantasyContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public IQueryable<FantasyPrivateLeaguePlayer> GetQueryable()
    {
        return _dbContext.FantasyPrivateLeaguePlayers;
    }

    public async Task<List<FantasyPrivateLeaguePlayer>> GetAllAsync()
    {
        _logger.LogDebug($"Get Private Fantasy League Players");

        return await _dbContext.FantasyPrivateLeaguePlayers.ToListAsync();
    }

    public async Task<FantasyPrivateLeaguePlayer?> GetByIdAsync(int id)
    {
        _logger.LogDebug($"Fetching Single Private Fantasy League Player {id}");

        return await _dbContext.FantasyPrivateLeaguePlayers.FindAsync(id);
    }

    public async Task<FantasyPrivateLeaguePlayer?> GetByDiscordIdAsync(long id)
    {
        _logger.LogDebug($"Fetching Single Private Fantasy League Player {id}");

        return await _dbContext.FantasyPrivateLeaguePlayers.FirstOrDefaultAsync(fplp => fplp.DiscordUserId == id);
    }

    public async Task<List<FantasyPrivateLeaguePlayer>> GetByFantasyLeagueAsync(int FantasyLeagueId)
    {
        _logger.LogDebug($"Fetching Players for Fantasy League Id {FantasyLeagueId}");

        return await _dbContext.FantasyPrivateLeaguePlayers.Where(fplp => fplp.FantasyLeagueId == FantasyLeagueId).ToListAsync();
    }

    public async Task AddAsync(FantasyPrivateLeaguePlayer entity)
    {
        _logger.LogInformation($"Adding new Private Fantasy League Player {entity.Id}");

        await _dbContext.FantasyPrivateLeaguePlayers.AddAsync(entity);

        return;
    }

    public async Task DeleteAsync(FantasyPrivateLeaguePlayer entity)
    {
        _logger.LogInformation($"Removing Discord User {entity.Id}");

        _dbContext.FantasyPrivateLeaguePlayers.Remove(entity);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task UpdateAsync(FantasyPrivateLeaguePlayer entity)
    {
        _logger.LogInformation($"Updating Fantasy Draft {entity.Id}");

        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        return;
    }
}