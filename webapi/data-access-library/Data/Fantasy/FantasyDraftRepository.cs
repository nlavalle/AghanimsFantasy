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
public class FantasyDraftRepository : IFantasyDraftRepository
{
    private readonly ILogger<FantasyDraftRepository> _logger;
    private AghanimsFantasyContext _dbContext;
    public FantasyDraftRepository(ILogger<FantasyDraftRepository> logger, AghanimsFantasyContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public IQueryable<FantasyDraft> GetQueryable()
    {
        return _dbContext.FantasyDrafts;
    }

    public async Task<FantasyDraft?> GetByUserFantasyLeague(FantasyLeague fantasyLeague, DiscordUser user)
    {
        _logger.LogDebug($"Fetching Fantasy League {fantasyLeague.Name} Draft for User {user.Username}");

        var userFantasyDraftQuery = _dbContext.FantasyDrafts
                    .Include(fd => fd.DraftPickPlayers)
                    .Where(fd => fd.FantasyLeagueId == fantasyLeague.Id && fd.DiscordAccountId == user.Id);

        return await userFantasyDraftQuery.FirstOrDefaultAsync();
    }

    public async Task<List<FantasyDraft>> GetAllAsync()
    {
        _logger.LogDebug($"Get Fantasy Drafts");

        return await _dbContext.FantasyDrafts.ToListAsync();
    }

    public async Task<FantasyDraft?> GetByIdAsync(long FantasyDraftId)
    {
        _logger.LogDebug($"Fetching Single Fantasy Draft {FantasyDraftId}");

        return await _dbContext.FantasyDrafts.FindAsync(FantasyDraftId);
    }

    public async Task AddAsync(FantasyDraft addFantasyDraft)
    {
        _logger.LogInformation($"Adding new Fantasy Draft for User {addFantasyDraft.DiscordAccountId}");

        await _dbContext.FantasyDrafts.AddAsync(addFantasyDraft);

        return;
    }

    public async Task DeleteAsync(FantasyDraft deleteFantasyDraft)
    {
        _logger.LogInformation($"Removing Fantasy Draft {deleteFantasyDraft.Id}");

        _dbContext.FantasyDrafts.Remove(deleteFantasyDraft);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task UpdateAsync(FantasyDraft updateFantasyDraft)
    {
        _logger.LogInformation($"Updating Fantasy Draft {updateFantasyDraft.Id}");

        _dbContext.Entry(updateFantasyDraft).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        return;
    }
}