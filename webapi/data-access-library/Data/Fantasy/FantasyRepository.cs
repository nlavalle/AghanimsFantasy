namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Service to fetch and transform the Postgres data to read/write fantasy draft data for the webapi
/// example is to fetch all of the current scores, vs adding a new draft. Controllers should handle none of the business
/// logic 
/// </summary>
public class FantasyRepository : IFantasyRepository
{
    private readonly ILogger<FantasyRepository> _logger;
    private AghanimsFantasyContext _dbContext;
    public FantasyRepository(ILogger<FantasyRepository> logger, AghanimsFantasyContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    #region Fantasy

    public async Task<List<FantasyPlayerPoints>> FantasyPlayerPointsByFantasyLeagueAsync(int FantasyLeagueId, int limit)
    {
        _logger.LogInformation($"Fetching Fantasy Points for Fantasy League Id: {FantasyLeagueId}");

        var fantasyPlayerPointsByLeagueQuery = _dbContext.FantasyPlayerPointsView
            .Include(fppv => fppv.FantasyPlayer)
                .ThenInclude(fp => fp.DotaAccount)
            .Include(fppv => fppv.FantasyPlayer)
                .ThenInclude(fp => fp.Team)
            .Include(fppv => fppv.FantasyMatchPlayer)
                .ThenInclude(fmp => fmp!.Hero)
            .Where(fpp => fpp.FantasyLeagueId == FantasyLeagueId)
            .Where(fpp => fpp.FantasyMatchPlayer != null)
            .Where(fpp => fpp.FantasyMatchPlayer!.GcMetadataPlayerParsed && fpp.FantasyMatchPlayer!.MatchDetailPlayerParsed)
            .OrderByDescending(fpp => fpp.FantasyMatchPlayer!.Match.MatchId)
            .ThenBy(fpp => fpp.FantasyMatchPlayer!.Team!.Name)
            .ThenBy(fpp => fpp.FantasyPlayer.TeamPosition)
            .Take(limit);

        _logger.LogDebug($"Match Details Query: {fantasyPlayerPointsByLeagueQuery.ToQueryString()}");

        return await fantasyPlayerPointsByLeagueQuery.ToListAsync();
    }

    public async Task<FantasyPlayerTopHeroes> GetFantasyPlayerTopHeroesAsync(long FantasyPlayerId)
    {
        _logger.LogInformation($"Getting Player Averages");

        FantasyPlayer? fantasyPlayer = await _dbContext.FantasyPlayers.FindAsync(FantasyPlayerId);

        if (fantasyPlayer == null)
        {
            // No player found
            return new FantasyPlayerTopHeroes();
        }

        var heroes = await _dbContext.Heroes.ToListAsync();

        var topHeroIds = await _dbContext.FantasyMatches
                .SelectMany(
                    md => md.Players,
                    (left, right) => new { Match = left, MatchPlayer = right }
                )
                .Where(mdp => mdp.MatchPlayer.Account != null && mdp.MatchPlayer.Account.Id == fantasyPlayer.DotaAccount.Id)
                .Where(mdp => mdp.MatchPlayer.Hero != null)
                .Where(mdp => mdp.Match.RadiantWin != null)
                .GroupBy(match => match.MatchPlayer.Hero)
                .Select(group => new
                {
                    HeroId = group.Key!.Id,
                    Count = group.Count(),
                    Wins = group.Where(g => (g.Match.RadiantWin!.Value && !g.MatchPlayer.DotaTeamSide) ||
                    (!g.Match.RadiantWin!.Value && g.MatchPlayer.DotaTeamSide)).Count(),
                    Losses = group.Where(g => (!g.Match.RadiantWin!.Value && !g.MatchPlayer.DotaTeamSide) ||
                    (g.Match.RadiantWin!.Value && g.MatchPlayer.DotaTeamSide)).Count()
                })
                .OrderByDescending(group => group.Count)
                .Take(3)
                .ToArrayAsync();

        var topHeroes = topHeroIds.Select(thi => new TopHeroCount
        {
            Hero = heroes.First(h => h.Id == thi.HeroId),
            Count = thi.Count,
            Wins = thi.Wins,
            Losses = thi.Losses
        })
        .ToArray();

        return new FantasyPlayerTopHeroes
        {
            FantasyPlayer = fantasyPlayer,
            FantasyPlayerId = fantasyPlayer.Id,
            TopHeroes = topHeroes
        };
    }

    public async Task<List<FantasyNormalizedAveragesTable>> GetFantasyNormalizedAveragesAsync(long FantasyPlayerId)
    {
        _logger.LogInformation($"Getting Player Averages");

        return await _dbContext.FantasyNormalizedAverages
                .Where(fnp => fnp.FantasyPlayer.Id == FantasyPlayerId)
                .ToListAsync();
    }

    public async Task<List<MetadataSummary>> MetadataSummariesByFantasyLeagueAsync(FantasyLeague FantasyLeague)
    {
        _logger.LogInformation($"Fetching Metadata for Fantasy League Id: {FantasyLeague.Id}");

        List<MetadataSummary> metadataSummaries = await _dbContext.MetadataSummaries
            .Where(ms => ms.FantasyLeagueId == FantasyLeague.Id)
            .Include(ms => ms.FantasyPlayer)
            .ToListAsync();

        return metadataSummaries;
    }

    #endregion Fantasy

    #region Match

    public async Task<List<MatchHighlights>> GetLastNMatchHighlights(int FantasyLeagueId, int MatchCount)
    {
        _logger.LogInformation($"Getting {MatchCount} Match Highlights for Fantasy League ID: {FantasyLeagueId}");

        var matchHighlightsQuery = _dbContext.MatchHighlightsView
                .Include(mhv => mhv.FantasyPlayer)
                .Where(mhv => mhv.FantasyLeagueId == FantasyLeagueId)
                .OrderByDescending(mhv => mhv.StartTime)
                .Take(MatchCount);

        _logger.LogDebug($"Match Highlights Query: {matchHighlightsQuery.ToQueryString()}");

        return await matchHighlightsQuery.ToListAsync();
    }
    #endregion Match
}