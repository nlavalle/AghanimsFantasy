namespace DataAccessLibrary.Data.Facades;

using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class FantasyPointsFacade
{
    private readonly ILogger<FantasyPointsFacade> _logger;
    private readonly AghanimsFantasyContext _dbContext;

    public FantasyPointsFacade(
        ILogger<FantasyPointsFacade> logger,
        AghanimsFantasyContext dbContext
    )
    {
        _logger = logger;
        _dbContext = dbContext;
    }


    public async Task<List<FantasyPlayerPoints>> GetFantasyPlayerPointsByMatchAsync(long MatchId)
    {
        return await _dbContext.FantasyPlayerPointsView
            .Where(fppv => fppv.FantasyMatchPlayer!.Match!.MatchId == MatchId)
            .Include(fppv => fppv.FantasyMatchPlayer)
            .ToListAsync();
    }

    public async Task<List<FantasyPlayerPoints>> GetFantasyPlayerPointsByMatchesAsync(IEnumerable<FantasyMatch> FantasyMatches)
    {
        return await _dbContext.FantasyPlayerPointsView
            .Where(fppv => fppv.FantasyMatchPlayerId != null && FantasyMatches.Any(mi => mi == fppv.FantasyMatchPlayer!.Match))
            .Include(fppv => fppv.FantasyMatchPlayer)
            .ToListAsync();
    }

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
            .OrderByDescending(fpp => fpp.FantasyMatchPlayer!.FantasyMatchId)
            .ThenBy(fpp => fpp.FantasyMatchPlayer!.Team!.Name)
            .ThenBy(fpp => fpp.FantasyPlayer.TeamPosition)
            .Take(limit);

        _logger.LogDebug($"Match Details Query: {fantasyPlayerPointsByLeagueQuery.ToQueryString()}");

        return await fantasyPlayerPointsByLeagueQuery.ToListAsync();
    }

    public async Task<List<FantasyPlayerTopHeroes>> GetFantasyPlayerTopHeroesAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Getting Fantasy Player Top Heroes");

        List<FantasyPlayer> fantasyPlayers = await _dbContext.FantasyPlayers.Where(fp => fp.FantasyLeagueId == FantasyLeagueId).ToListAsync();

        if (fantasyPlayers.Count == 0)
        {
            // No player found
            return new List<FantasyPlayerTopHeroes>();
        }

        var heroes = await _dbContext.Heroes.ToListAsync();

        var fantasyAccountTopCount = await _dbContext.FantasyAccountTopHeroesView
            .Where(atc => fantasyPlayers.Select(fp => fp.DotaAccountId).Contains(atc.AccountId))
            .ToListAsync();

        return fantasyPlayers.Select(fp => new FantasyPlayerTopHeroes
        {
            FantasyPlayerId = fp.Id,
            TopHeroes = fantasyAccountTopCount
                .Where(tc => tc.AccountId == fp.DotaAccountId)
                .Select(tc => new TopHeroCount
                {
                    Hero = heroes.First(h => h.Id == tc.HeroId),
                    Count = tc.Count,
                    Wins = tc.Wins,
                    Losses = tc.Losses
                })
                .ToArray()
        }).ToList();
    }

    public async Task<List<FantasyNormalizedAveragesTable>> GetFantasyNormalizedAveragesAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Getting Player Averages");

        return await _dbContext.FantasyNormalizedAverages
                .Where(fnp => fnp.FantasyPlayer.FantasyLeagueId == FantasyLeagueId)
                .ToListAsync();
    }

    public async Task<List<FantasyPlayerBudgetProbabilityTable>> GetFantasyPlayerBudgetCostsByFantasyLeague(long FantasyLeagueId)
    {
        _logger.LogInformation($"Getting Player Budget Probabilities");

        return await _dbContext.FantasyPlayerBudgetProbability
                .Where(fpbp => fpbp.FantasyLeague.Id == FantasyLeagueId)
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
}