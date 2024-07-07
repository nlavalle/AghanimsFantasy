namespace DataAccessLibrary.Data;

using System.Collections.Immutable;
using DataAccessLibrary.Models.WebApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SteamKit2.GC.Dota.Internal;

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

    #region MatchHistory
    public async Task<List<MatchHistory>> GetMatchHistoriesAsync()
    {
        _logger.LogInformation($"Getting all Match Histories");

        var matchHistoryQuery = _dbContext.MatchHistory;

        _logger.LogDebug($"GetMatchHistoriesAsync SQL Query: {matchHistoryQuery.ToQueryString()}");

        return await matchHistoryQuery.ToListAsync();
    }

    public async Task<MatchHistory?> GetMatchHistoryAsync(long matchId)
    {
        _logger.LogInformation($"Getting Match History {matchId}");

        return await _dbContext.MatchHistory.FindAsync(matchId);
    }

    public async Task AddMatchHistoryAsync(MatchHistory matchHistory)
    {
        _logger.LogInformation($"Adding Match History {matchHistory.MatchId}");

        await _dbContext.MatchHistory.AddAsync(matchHistory);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task<List<MatchHistory>> GetMatchHistoryByFantasyLeagueAsync(int FantasyLeagueId)
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

    public async Task<List<MatchHistory>> GetMatchHistoriesNotInFantasyMatches(int takeAmount)
    {
        _logger.LogInformation($"Getting new Match Histories not loaded into Fantasy Match");

        var newMatchHistoryQuery = _dbContext.MatchHistory.Where(
                mh => !_dbContext.FantasyMatches.Any(fm => fm.MatchId == mh.MatchId))
                .OrderBy(mh => mh.MatchId)
                .Take(takeAmount);

        _logger.LogDebug($"Match History SQL Query: {newMatchHistoryQuery.ToQueryString()}");

        return await newMatchHistoryQuery.ToListAsync();
    }

    public async Task<List<MatchHistory>> GetMatchHistoriesNotInGcMatches(int takeAmount)
    {
        _logger.LogInformation($"Getting new Match Histories not loaded into Gc Dota Matches");

        var matchesNotInGcQuery = _dbContext.MatchHistory
            .Where(
                mh => !_dbContext.GcDotaMatches
                    .Where(dm => dm.replay_state == CMsgDOTAMatch.ReplayState.REPLAY_AVAILABLE)
                    .Select(gdm => gdm.match_id)
                    .Contains((ulong)mh.MatchId))
                .OrderBy(mh => mh.MatchId)
                .Take(takeAmount);

        _logger.LogDebug($"GetMatchHistoriesNotInGcMatches SQL Query: {matchesNotInGcQuery.ToQueryString()}");

        return await matchesNotInGcQuery.ToListAsync();
    }

    public async Task<List<MatchHistory>> GetMatchHistoriesNotInMatchDetails(int takeAmount)
    {
        _logger.LogInformation($"Getting new Match Histories not loaded into Fantasy Match");

        var newMatchHistoryQuery = _dbContext.MatchHistory
            .Where(mh => _dbContext.Leagues
                    .Where(l => l.IsActive == true)
                    .Select(l => l.Id)
                    .Contains(mh.LeagueId)
            )
            .Where(
                mh => !_dbContext.MatchDetails
                    .Where(md => _dbContext.Leagues
                        .Where(l => l.IsActive == true)
                        .Select(l => l.Id)
                        .Contains(md.LeagueId)
                    )
                    .Select(md => md.MatchId)
                    .Contains(mh.MatchId)
            )
            .OrderBy(mh => mh.MatchId)
            .Take(takeAmount);

        _logger.LogDebug($"GetMatchHistoriesNotInMatchDetails SQL Query: {newMatchHistoryQuery.ToQueryString()}");

        return await newMatchHistoryQuery.ToListAsync();
    }
    #endregion MatchHistory

    #region MatchDetail
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

    public async Task AddMatchDetailAsync(MatchDetail matchDetail)
    {
        _logger.LogInformation($"Adding Match Detail: {matchDetail.MatchId}");

        await _dbContext.MatchDetails.AddAsync(matchDetail);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task<List<MatchDetail>> GetMatchDetailsByFantasyLeagueAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Getting Match Details for Fantasy League {FantasyLeagueId}");

        var leagueMatchDetailsQuery = QueryFantasyLeagueMatchDetails(FantasyLeagueId)
            .Include(md => md.PicksBans);

        return await leagueMatchDetailsQuery.ToListAsync();
    }
    #endregion MatchDetail

    public async Task<List<MatchDetailsPlayer>> GetMatchDetailPlayersByLeagueAsync(int? LeagueId)
    {
        _logger.LogInformation($"Getting Match Details Players for League ID: {LeagueId}");

        var matchDetailPlayerLeagueQuery = QueryLeagueMatchDetails(LeagueId)
            .SelectMany(md => md.Players)
            .Where(p => p.LeaverStatus != 1); // Filter out games players left (typically false starts)

        return await matchDetailPlayerLeagueQuery.ToListAsync();
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