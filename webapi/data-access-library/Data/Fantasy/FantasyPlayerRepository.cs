namespace DataAccessLibrary.Data;

using System.Threading.Tasks;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Service to fetch and transform the Postgres data to read/write fantasy draft data for the webapi
/// example is to fetch all of the current scores, vs adding a new draft. Controllers should handle none of the business
/// logic 
/// </summary>
public class FantasyPlayerRepository : IFantasyPlayerRepository
{
    private readonly ILogger<FantasyPlayerRepository> _logger;
    private AghanimsFantasyContext _dbContext;
    public FantasyPlayerRepository(ILogger<FantasyPlayerRepository> logger, AghanimsFantasyContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<FantasyPlayer>> GetFantasyLeaguePlayersAsync(FantasyLeague FantasyLeague)
    {
        _logger.LogInformation($"Get Fantasy Players by Fantasy League Id: {FantasyLeague.Id}");

        var fantasyPlayerLeagueQuery = _dbContext.FantasyPlayers
            .Include(fp => fp.FantasyLeague)
            .Include(fp => fp.Team)
            .Include(fp => fp.DotaAccount)
            .Where(fp => fp.FantasyLeagueId == FantasyLeague.Id)
            .OrderBy(fp => fp.Team!.Name)
                .ThenBy(fp => fp.DotaAccount!.Name);

        _logger.LogDebug($"Get Fantasy Players by Fantasy League Query: {fantasyPlayerLeagueQuery.ToQueryString()}");

        return await fantasyPlayerLeagueQuery.ToListAsync();
    }

    public async Task<FantasyPlayer?> GetByIdAsync(long FantasyPlayerId)
    {
        _logger.LogInformation($"Fetching Single Fantasy Player {FantasyPlayerId}");

        return await _dbContext.FantasyPlayers.FindAsync(FantasyPlayerId);
    }

    public async Task<IEnumerable<FantasyPlayer>> GetAllAsync()
    {
        _logger.LogInformation($"Get Fantasy Players");

        var fantasyPlayerLeagueQuery = _dbContext.FantasyPlayers
            .Include(fp => fp.Team)
            .Include(fp => fp.DotaAccount)
            .OrderBy(fp => fp.Team!.Name)
                .ThenBy(fp => fp.DotaAccount!.Name);

        _logger.LogDebug($"Get Fantasy Players by Fantasy League Query: {fantasyPlayerLeagueQuery.ToQueryString()}");

        return await fantasyPlayerLeagueQuery.ToListAsync();
    }

    public async Task AddAsync(FantasyPlayer addFantasyPlayer)
    {
        _logger.LogInformation($"Adding new Fantasy Player {addFantasyPlayer.DotaAccountId}");

        await _dbContext.FantasyPlayers.AddAsync(addFantasyPlayer);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task DeleteAsync(FantasyPlayer deleteFantasyPlayer)
    {
        _logger.LogInformation($"Removing Fantasy Player {deleteFantasyPlayer.DotaAccountId}");

        _dbContext.FantasyPlayers.Remove(deleteFantasyPlayer);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task UpdateAsync(FantasyPlayer updateFantasyPlayer)
    {
        _logger.LogInformation($"Updating Fantasy Player {updateFantasyPlayer.DotaAccountId}");

        _dbContext.Entry(updateFantasyPlayer).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        return;
    }
}