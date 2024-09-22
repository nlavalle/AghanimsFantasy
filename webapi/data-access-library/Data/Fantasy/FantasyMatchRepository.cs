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
public class FantasyMatchRepository : IFantasyMatchRepository
{
    private readonly ILogger<FantasyMatchRepository> _logger;
    private AghanimsFantasyContext _dbContext;
    public FantasyMatchRepository(ILogger<FantasyMatchRepository> logger, AghanimsFantasyContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<List<FantasyMatch>> GetNotGcDetailParsed(int takeAmount)
    {
        _logger.LogInformation($"Getting Fantasy Matches not GC Match Detail Parsed");

        var matchDetailsToParse = _dbContext.FantasyMatches.Where(
                fm => !fm.MatchDetailParsed && _dbContext.GcDotaMatches.Any(md => (long)md.match_id == fm.MatchId))
                .Take(takeAmount);

        _logger.LogDebug($"GetFantasyMatchesNotGcDetailParsed SQL Query: {matchDetailsToParse.ToQueryString()}");

        return await matchDetailsToParse.ToListAsync();
    }

    public async Task<List<FantasyMatch>> GetNotDetailParsed(int takeAmount)
    {
        _logger.LogInformation($"Getting Fantasy Matches not Match Detail Parsed");

        var matchDetailsToParse = _dbContext.FantasyMatches.Where(
                fm => !fm.MatchDetailParsed && _dbContext.MatchDetails.Any(md => md.MatchId == fm.MatchId))
                .Take(takeAmount);

        _logger.LogDebug($"GetFantasyMatchesNotDetailParsed SQL Query: {matchDetailsToParse.ToQueryString()}");

        return await matchDetailsToParse.ToListAsync();
    }

    public async Task<FantasyMatch?> GetByIdAsync(long FantasyMatchId)
    {
        _logger.LogInformation($"Fetching Single Fantasy Match {FantasyMatchId}");

        return await _dbContext.FantasyMatches.FindAsync(FantasyMatchId);
    }

    public async Task<IEnumerable<FantasyMatch>> GetAllAsync()
    {
        _logger.LogInformation($"Get Fantasy Matches");

        return await _dbContext.FantasyMatches.ToListAsync();
    }

    public async Task AddAsync(FantasyMatch addFantasyMatch)
    {
        _logger.LogInformation($"Adding new Fantasy Match {addFantasyMatch.MatchId}");

        await _dbContext.FantasyMatches.AddAsync(addFantasyMatch);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task AddRangeAsync(IEnumerable<FantasyMatch> newFantasyMatches)
    {
        _logger.LogInformation($"Adding {newFantasyMatches.Count()} new Fantasy Matches");

        await _dbContext.FantasyMatches.AddRangeAsync(newFantasyMatches);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task DeleteAsync(FantasyMatch deleteFantasyMatch)
    {
        _logger.LogInformation($"Removing Fantasy Match {deleteFantasyMatch.MatchId}");

        _dbContext.FantasyMatches.Remove(deleteFantasyMatch);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task UpdateAsync(FantasyMatch updateFantasyMatch)
    {
        _logger.LogInformation($"Updating Fantasy Match {updateFantasyMatch.MatchId}");

        _dbContext.Entry(updateFantasyMatch).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task UpdateRangeAsync(IEnumerable<FantasyMatch> updateFantasyMatches)
    {
        _logger.LogInformation($"Updating {updateFantasyMatches.Count()} Fantasy Matches");

        _dbContext.FantasyMatches.UpdateRange(updateFantasyMatches);
        await _dbContext.SaveChangesAsync();

        return;
    }
}