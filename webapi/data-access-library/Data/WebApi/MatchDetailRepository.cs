namespace DataAccessLibrary.Data;

using System.Threading.Tasks;
using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.ProMetadata;
using DataAccessLibrary.Models.WebApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Service to fetch and transform the Postgres data to read/write fantasy draft data for the webapi
/// example is to fetch all of the current scores, vs adding a new draft. Controllers should handle none of the business
/// logic 
/// </summary>
public class MatchDetailRepository : IMatchDetailRepository
{
    private readonly ILogger<MatchDetailRepository> _logger;
    private AghanimsFantasyContext _dbContext;
    public MatchDetailRepository(ILogger<MatchDetailRepository> logger, AghanimsFantasyContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public IQueryable<MatchDetail> GetQueryable()
    {
        return _dbContext.MatchDetails;
    }

    public async Task<List<MatchDetailsPlayer>> GetByLeagueAsync(League? League)
    {
        _logger.LogDebug($"Getting Match Details Players for League ID: {League?.Id ?? 0}");

        var matchDetailPlayerLeagueQuery = QueryLeagueMatchDetails(League?.Id)
            .SelectMany(md => md.Players)
            .Where(p => p.LeaverStatus != 1); // Filter out games players left (typically false starts)

        return await matchDetailPlayerLeagueQuery.ToListAsync();
    }

    public async Task<List<MatchDetail>> GetByFantasyLeagueAsync(FantasyLeague FantasyLeague)
    {
        _logger.LogDebug($"Getting Match Details for Fantasy League {FantasyLeague.Id}");

        var leagueMatchDetailsQuery = QueryFantasyLeagueMatchDetails(FantasyLeague.Id)
            .Include(md => md.PicksBans);

        return await leagueMatchDetailsQuery.ToListAsync();
    }

    public async Task<MatchDetail?> GetByIdAsync(long MatchId)
    {
        _logger.LogDebug($"Getting Match Detail for Match: {MatchId}");

        var matchDetailsQuery = _dbContext.MatchDetails
                .Where(md => md.MatchId == MatchId)
                .Include(md => md.PicksBans)
                .Include(md => md.Players).ThenInclude(p => p.AbilityUpgrades);

        _logger.LogDebug($"Get Match Detail Query: {matchDetailsQuery.ToQueryString()}");

        return await matchDetailsQuery.FirstOrDefaultAsync();
    }

    public async Task<List<MatchDetail>> GetAllAsync()
    {
        _logger.LogInformation($"Get Match Details");

        return await _dbContext.MatchDetails.ToListAsync();
    }

    public async Task AddAsync(MatchDetail addMatchDetail)
    {
        _logger.LogInformation($"Adding new Match Detail {addMatchDetail.MatchId}");

        await _dbContext.MatchDetails.AddAsync(addMatchDetail);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task DeleteAsync(MatchDetail deleteMatchDetail)
    {
        _logger.LogInformation($"Removing Match Detail {deleteMatchDetail.MatchId}");

        _dbContext.MatchDetails.Remove(deleteMatchDetail);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task UpdateAsync(MatchDetail updateMatchDetail)
    {
        _logger.LogInformation($"Updating Match Detail {updateMatchDetail.MatchId}");

        _dbContext.Entry(updateMatchDetail).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        return;
    }

    private IQueryable<MatchDetail> QueryLeagueMatchDetails(int? LeagueId)
    {
        // This logic is awkward so I'd rather do it all once here and return a Queryable
        var leagueMatchesQuery = _dbContext.Leagues
            .SelectMany(
                l => l.MatchDetails,
                (left, right) => new { League = left, MatchDetail = right }
            )
            .Where(l => l.League.Id == LeagueId || LeagueId == null)
            .Select(l => l.MatchDetail);

        return leagueMatchesQuery;
    }

    private IQueryable<MatchDetail> QueryFantasyLeagueMatchDetails(int FantasyLeagueId)
    {
        // This logic is awkward so I'd rather do it all once here and return a Queryable
        var fantasyLeagueMatchesQuery = _dbContext.Leagues
            .SelectMany(
                l => l.FantasyLeagues,
                (left, right) => new { League = left, FantasyLeague = right }
            )
            .SelectMany(
                l => l.League.MatchDetails,
                (left, right) => new { left.League, left.FantasyLeague, MatchDetail = right }
            )
            .Where(l => l.FantasyLeague.Id == FantasyLeagueId)
            .Where(l =>
                l.MatchDetail.StartTime >= l.FantasyLeague.LeagueStartTime &&
                l.MatchDetail.StartTime <= l.FantasyLeague.LeagueEndTime
            )
            .Select(l => l.MatchDetail);

        return fantasyLeagueMatchesQuery;
    }
}