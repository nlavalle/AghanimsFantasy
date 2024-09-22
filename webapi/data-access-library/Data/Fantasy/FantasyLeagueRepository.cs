namespace DataAccessLibrary.Data;

using System.Threading.Tasks;
using DataAccessLibrary.Models.Discord;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Service to fetch and transform the Postgres data to read/write fantasy draft data for the webapi
/// example is to fetch all of the current scores, vs adding a new draft. Controllers should handle none of the business
/// logic 
/// </summary>
public class FantasyLeagueRepository : IFantasyLeagueRepository
{
    private readonly ILogger<FantasyLeagueRepository> _logger;
    private AghanimsFantasyContext _dbContext;
    public FantasyLeagueRepository(ILogger<FantasyLeagueRepository> logger, AghanimsFantasyContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<FantasyLeague>> GetAccessibleFantasyLeaguesAsync(DiscordUser? user)
    {
        return await _dbContext.FantasyLeagues
                .Include(fl => fl.FantasyPrivateLeaguePlayers)
                .Include(fl => fl.League)
                .Where(fl => !fl.IsPrivate || (user != null && fl.FantasyPrivateLeaguePlayers.Any(p => p.DiscordUser == user)))
                .ToListAsync();
    }

    public async Task<FantasyLeague?> GetAccessibleFantasyLeagueAsync(int FantasyLeagueId, DiscordUser? user)
    {
        return await _dbContext.FantasyLeagues
                .Include(fl => fl.FantasyPrivateLeaguePlayers)
                .Include(fl => fl.League)
                .Where(fl => !fl.IsPrivate || (user != null && fl.FantasyPrivateLeaguePlayers.Any(p => p.DiscordUser == user)))
                .Where(fl => fl.Id == FantasyLeagueId)
                .FirstOrDefaultAsync();
    }

    public async Task<DateTime> GetLeagueLockedDateAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching Draft Locked Date for Fantasy League Id: {FantasyLeagueId}");

        return DateTimeOffset.FromUnixTimeSeconds(
                await _dbContext.FantasyLeagues.Where(l => l.Id == FantasyLeagueId).Select(l => l.FantasyDraftLocked).FirstOrDefaultAsync()
            ).DateTime;
    }

    public async Task<FantasyLeague?> GetByIdAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching Single Fantasy League {FantasyLeagueId}");

        return await _dbContext.FantasyLeagues.FindAsync(FantasyLeagueId);
    }

    public async Task<IEnumerable<FantasyLeague>> GetAllAsync()
    {
        _logger.LogInformation($"Fetching All Fantasy Leagues");

        return await _dbContext.FantasyLeagues
                .ToListAsync();
    }

    public async Task AddAsync(FantasyLeague addFantasyLeague)
    {
        _logger.LogInformation($"Adding new Fantasy League {addFantasyLeague.Name}");

        await _dbContext.FantasyLeagues.AddAsync(addFantasyLeague);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task DeleteAsync(FantasyLeague deleteFantasyLeague)
    {
        _logger.LogInformation($"Removing Fantasy League {deleteFantasyLeague.Name}");

        _dbContext.FantasyLeagues.Remove(deleteFantasyLeague);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task UpdateAsync(FantasyLeague updateFantasyLeague)
    {
        _logger.LogInformation($"Updating Fantasy League {updateFantasyLeague.Name}");

        _dbContext.Entry(updateFantasyLeague).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        return;
    }
}
