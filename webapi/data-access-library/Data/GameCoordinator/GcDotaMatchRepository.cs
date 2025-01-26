namespace DataAccessLibrary.Data;

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SteamKit2.GC.Dota.Internal;

/// <summary>
/// Service to fetch and transform the Postgres data to read/write fantasy draft data for the webapi
/// example is to fetch all of the current scores, vs adding a new draft. Controllers should handle none of the business
/// logic 
/// </summary>
public class GcDotaMatchRepository : IGcDotaMatchRepository
{
    private readonly ILogger<GcDotaMatchRepository> _logger;
    private AghanimsFantasyContext _dbContext;
    public GcDotaMatchRepository(ILogger<GcDotaMatchRepository> logger, AghanimsFantasyContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public IQueryable<CMsgDOTAMatch> GetQueryable()
    {
        return _dbContext.GcDotaMatches;
    }

    public async Task<CMsgDOTAMatch?> GetByIdAsync(ulong MatchId)
    {
        _logger.LogDebug($"Fetching Single GcDotaMatch {MatchId}");

        return await _dbContext.GcDotaMatches.FindAsync(MatchId);
    }

    public async Task<List<CMsgDOTAMatch>> GetAllAsync()
    {
        _logger.LogDebug($"Get GcDotaMatches");

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