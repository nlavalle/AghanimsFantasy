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
public class FantasyLeagueWeightRepository : IFantasyLeagueWeightRepository
{
    private readonly ILogger<FantasyLeagueWeightRepository> _logger;
    private AghanimsFantasyContext _dbContext;
    public FantasyLeagueWeightRepository(ILogger<FantasyLeagueWeightRepository> logger, AghanimsFantasyContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public IQueryable<FantasyLeagueWeight> GetQueryable()
    {
        return _dbContext.FantasyLeagueWeights;
    }

    public async Task<FantasyLeagueWeight?> GetByIdAsync(int FantasyLeagueWeightId)
    {
        _logger.LogDebug($"Fetching Single Fantasy League Weight {FantasyLeagueWeightId}");

        return await _dbContext.FantasyLeagueWeights.FindAsync(FantasyLeagueWeightId);
    }

    public async Task<List<FantasyLeagueWeight>> GetAllAsync()
    {
        _logger.LogDebug($"Fetching All Fantasy League Weights");

        return await _dbContext.FantasyLeagueWeights
                .ToListAsync();
    }

    public async Task AddAsync(FantasyLeagueWeight addFantasyLeagueWeight)
    {
        _logger.LogInformation($"Adding new Fantasy League Weight for Fantasy League ID {addFantasyLeagueWeight.FantasyLeagueId}");

        await _dbContext.FantasyLeagueWeights.AddAsync(addFantasyLeagueWeight);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task DeleteAsync(FantasyLeagueWeight deleteFantasyLeagueWeight)
    {
        _logger.LogInformation($"Removing Fantasy League Weight ID {deleteFantasyLeagueWeight.Id}");

        _dbContext.FantasyLeagueWeights.Remove(deleteFantasyLeagueWeight);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task UpdateAsync(FantasyLeagueWeight updateFantasyLeagueWeight)
    {
        _logger.LogInformation($"Updating Fantasy League Weight ID {updateFantasyLeagueWeight.Id}");

        _dbContext.Entry(updateFantasyLeagueWeight).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        return;
    }
}
