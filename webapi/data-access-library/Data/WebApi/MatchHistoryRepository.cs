namespace DataAccessLibrary.Data;

using System.Threading.Tasks;
using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.ProMetadata;
using DataAccessLibrary.Models.WebApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SteamKit2.GC.Dota.Internal;

/// <summary>
/// Service to fetch and transform the Postgres data to read/write fantasy draft data for the webapi
/// example is to fetch all of the current scores, vs adding a new draft. Controllers should handle none of the business
/// logic 
/// </summary>
public class MatchHistoryRepository : IMatchHistoryRepository
{
    private readonly ILogger<MatchHistoryRepository> _logger;
    private AghanimsFantasyContext _dbContext;
    public MatchHistoryRepository(ILogger<MatchHistoryRepository> logger, AghanimsFantasyContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public IQueryable<MatchHistory> GetQueryable()
    {
        return _dbContext.MatchHistory;
    }

    public async Task<List<MatchHistory>> GetByLeagueAsync(League League)
    {
        _logger.LogInformation($"Getting Match History for League Id: {League.Id}");

        var matchHistoryQuery = _dbContext.Leagues
            .SelectMany(
                l => l.MatchHistories,
                (left, right) => new { League = left, MatchHistory = right }
            )
            .Where(l => l.League.Id == League.Id)
            .Select(l => l.MatchHistory)
            .Include(mh => mh.Players);

        _logger.LogDebug($"Match History SQL Query: {matchHistoryQuery.ToQueryString()}");

        return await matchHistoryQuery.ToListAsync();
    }

    public async Task<List<MatchHistory>> GetByFantasyLeagueAsync(FantasyLeague FantasyLeague)
    {
        _logger.LogInformation($"Getting Match History for Fantasy League Id: {FantasyLeague.Id}");

        var matchHistoryQuery = _dbContext.Leagues
            .SelectMany(
                l => l.MatchHistories,
                (left, right) => new { League = left, MatchHistory = right }
            )
            .SelectMany(
                l => l.League.FantasyLeagues,
                (left, right) => new { League = left.League, MatchHistory = left.MatchHistory, FantasyLeague = right }
            )
            .Where(l => l.FantasyLeague.Id == FantasyLeague.Id &&
                    l.MatchHistory.StartTime >= l.FantasyLeague.LeagueStartTime &&
                    l.MatchHistory.StartTime <= l.FantasyLeague.LeagueEndTime)
            .Select(l => l.MatchHistory)
            .Include(mh => mh.Players);

        _logger.LogDebug($"Match History SQL Query: {matchHistoryQuery.ToQueryString()}");

        return await matchHistoryQuery.ToListAsync();
    }

    public async Task<MatchHistory?> GetByIdAsync(long MatchId)
    {
        _logger.LogDebug($"Fetching Single Match History {MatchId}");

        return await _dbContext.MatchHistory.FindAsync(MatchId);
    }

    public async Task<List<MatchHistory>> GetAllAsync()
    {
        _logger.LogDebug($"Get Match Histories");

        return await _dbContext.MatchHistory.ToListAsync();
    }

    public async Task AddAsync(MatchHistory addMatchHistory)
    {
        _logger.LogInformation($"Adding new Match History {addMatchHistory.MatchId}");

        await _dbContext.MatchHistory.AddAsync(addMatchHistory);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task DeleteAsync(MatchHistory deleteMatchHistory)
    {
        _logger.LogInformation($"Removing Match History {deleteMatchHistory.MatchId}");

        _dbContext.MatchHistory.Remove(deleteMatchHistory);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task UpdateAsync(MatchHistory updateMatchHistory)
    {
        _logger.LogInformation($"Updating Match History {updateMatchHistory.MatchId}");

        _dbContext.Entry(updateMatchHistory).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        return;
    }
}