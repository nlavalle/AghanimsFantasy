using csharp_ef_webapi.Models;
using csharp_ef_webapi.Models.WebApi;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace csharp_ef_webapi.Data;

/// <summary>
/// Service to fetch and transform the Postgres data to read/write fantasy draft data for the webapi
/// example is to fetch all of the current scores, vs adding a new draft. Controllers should handle none of the business
/// logic 
/// </summary>
public class WebApiRepository : IWebApiRepository
{
    private readonly ILogger<WebApiRepository> _logger;
    private AghanimsFantasyContext _dbContext;
    public WebApiRepository(ILogger<WebApiRepository> logger, AghanimsFantasyContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<MatchHistory>> GetMatchHistoryByFantasyLeagueAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Getting Match History for Fantasy League Id: {FantasyLeagueId}");

        var matchHistoryQuery = _dbContext.Leagues
            .SelectMany(
                l => l.MatchHistories,
                (left, right) => new { League = left, MatchHistory = right }
            )
            .SelectMany(
                l => l.League.FantasyLeagues,
                (left, right) => new { League = left.League, MatchHistory = left.MatchHistory, FantasyLeague = right }
            )
            .Where(l => l.FantasyLeague.Id == FantasyLeagueId &&
                    l.MatchHistory.StartTime >= l.FantasyLeague.LeagueStartTime &&
                    l.MatchHistory.StartTime <= l.FantasyLeague.LeagueEndTime)
            .Select(l => l.MatchHistory)
            .Include(mh => mh.Players);

        _logger.LogDebug($"Match History SQL Query: {matchHistoryQuery.ToQueryString()}");

        return await matchHistoryQuery.ToListAsync();
    }

    public async Task<MatchDetail?> GetMatchDetailAsync(long MatchId)
    {
        _logger.LogInformation($"Getting Match Detail for Match: {MatchId}");

        var matchDetailsQuery = _dbContext.MatchDetails
                .Where(md => md.MatchId == MatchId)
                .Include(md => md.PicksBans)
                .Include(md => md.Players).ThenInclude(p => p.AbilityUpgrades);

        _logger.LogDebug($"Get Match Detail Query: {matchDetailsQuery.ToQueryString()}");

        return await matchDetailsQuery.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<MatchDetailsPlayer>> GetMatchDetailPlayersByLeagueAsync(int? LeagueId)
    {
        _logger.LogInformation($"Getting Match Details Players for League ID: {LeagueId}");

        var matchDetailPlayerLeagueQuery = QueryLeagueMatchDetails(LeagueId)
            .SelectMany(md => md.Players)
            .Where(p => p.LeaverStatus != 1); // Filter out games players left (typically false starts)

        return await matchDetailPlayerLeagueQuery.ToListAsync();
    }

    public async Task<IEnumerable<MatchDetail>> GetMatchDetailsByFantasyLeagueAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Getting Match Details for Fantasy League {FantasyLeagueId}");

        var leagueMatchDetailsQuery = QueryFantasyLeagueMatchDetails(FantasyLeagueId)
            .Include(md => md.PicksBans);

        return await leagueMatchDetailsQuery.ToListAsync();
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