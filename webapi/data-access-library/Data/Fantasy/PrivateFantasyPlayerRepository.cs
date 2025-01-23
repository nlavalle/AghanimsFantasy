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

    public async Task<List<FantasyLeague>> GetPrivateFantasyLeaguesAsync(long DiscordAccountId)
    {
        return await _dbContext.fantasyPrivateLeaguePlayers.Where(fplp => fplp.DiscordUserId == DiscordAccountId).Select(fplp => fplp.FantasyLeague!).Distinct().ToListAsync();
    }

    public async Task<FantasyPrivateLeaguePlayer?> GetFantasyPrivateLeaguePlayerAsync(int FantasyPrivateLeaguePlayerId)
    {
        return await _dbContext.fantasyPrivateLeaguePlayers.FindAsync(FantasyPrivateLeaguePlayerId);
    }

    public async Task<List<FantasyPrivateLeaguePlayer>> GetFantasyPrivateLeaguePlayersAsync(int FantasyLeagueId)
    {
        return await _dbContext.fantasyPrivateLeaguePlayers.Where(fplp => fplp.FantasyLeagueId == FantasyLeagueId).ToListAsync();
    }

    public async Task AddPrivateFantasyPlayerAsync(FantasyPrivateLeaguePlayer newPrivateFantasyLeaguePlayer)
    {
        _logger.LogInformation($"Adding new Private Fantasy Player {newPrivateFantasyLeaguePlayer.DiscordUserId} to Fantasy League {newPrivateFantasyLeaguePlayer.FantasyLeagueId}");

        await _dbContext.fantasyPrivateLeaguePlayers.AddAsync(newPrivateFantasyLeaguePlayer);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task UpdatePrivateFantasyPlayerAsync(FantasyPrivateLeaguePlayer updatePrivateFantasyLeaguePlayer)
    {
        _logger.LogInformation($"Updating Private Fantasy Player {updatePrivateFantasyLeaguePlayer.Id}");

        _dbContext.Entry(updatePrivateFantasyLeaguePlayer).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task DeletePrivateFantasyPlayerAsync(FantasyPrivateLeaguePlayer deletePrivateFantasyLeaguePlayer)
    {
        _logger.LogInformation($"Removing Private Fantasy Player {deletePrivateFantasyLeaguePlayer.Id}");

        _dbContext.fantasyPrivateLeaguePlayers.Remove(deletePrivateFantasyLeaguePlayer);
        await _dbContext.SaveChangesAsync();

        return;
    }
}