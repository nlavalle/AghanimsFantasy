using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccessLibrary.Facades;

public class FantasyPointsFacade
{
    private readonly ILogger<FantasyPointsFacade> _logger;
    private readonly IProMetadataRepository _proMetadataRepository;
    private readonly IFantasyPlayerRepository _fantasyPlayerRepository;
    private readonly IFantasyMatchRepository _fantasyMatchRepository;
    private readonly IFantasyViewsRepository _fantasyViewsRepository;

    public FantasyPointsFacade(
        ILogger<FantasyPointsFacade> logger,
        IProMetadataRepository proMetadataRepository,
        IFantasyViewsRepository fantasyViewsRepository,
        IFantasyMatchRepository fantasyMatchRepository,
        IFantasyPlayerRepository fantasyPlayerRepository
    )
    {
        _logger = logger;
        _proMetadataRepository = proMetadataRepository;
        _fantasyPlayerRepository = fantasyPlayerRepository;
        _fantasyMatchRepository = fantasyMatchRepository;
        _fantasyViewsRepository = fantasyViewsRepository;
    }


    public async Task<List<FantasyPlayerPoints>> GetFantasyPlayerPointsByMatchAsync(long MatchId)
    {
        return await _fantasyViewsRepository.GetPlayerPointsQueryable()
            .Where(fppv => fppv.FantasyMatchPlayer!.Match!.MatchId == MatchId)
            .Include(fppv => fppv.FantasyMatchPlayer)
            .ToListAsync();
    }

    public async Task<List<FantasyPlayerPoints>> GetFantasyPlayerPointsByMatchesAsync(IEnumerable<FantasyMatch> FantasyMatches)
    {
        return await _fantasyViewsRepository.GetPlayerPointsQueryable()
            .Where(fppv => fppv.FantasyMatchPlayerId != null && FantasyMatches.Any(mi => mi == fppv.FantasyMatchPlayer!.Match))
            .Include(fppv => fppv.FantasyMatchPlayer)
            .ToListAsync();
    }

    public async Task<List<FantasyPlayerPoints>> FantasyPlayerPointsByFantasyLeagueAsync(int FantasyLeagueId, int limit)
    {
        _logger.LogInformation($"Fetching Fantasy Points for Fantasy League Id: {FantasyLeagueId}");

        var fantasyPlayerPointsByLeagueQuery = _fantasyViewsRepository.GetPlayerPointsQueryable()
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

    public async Task<FantasyPlayerTopHeroes> GetFantasyPlayerTopHeroesAsync(long FantasyPlayerId)
    {
        _logger.LogInformation($"Getting Player Averages");

        FantasyPlayer? fantasyPlayer = await _fantasyPlayerRepository.GetByIdAsync(FantasyPlayerId);

        if (fantasyPlayer == null)
        {
            // No player found
            return new FantasyPlayerTopHeroes();
        }

        var heroes = await _proMetadataRepository.GetHeroesAsync();

        var topHeroIds = await _fantasyMatchRepository.GetQueryable()
                .SelectMany(
                    md => md.Players,
                    (left, right) => new { Match = left, MatchPlayer = right }
                )
                .Where(mdp => mdp.MatchPlayer.Account != null && mdp.MatchPlayer.Account.Id == fantasyPlayer.DotaAccount!.Id)
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

        return await _fantasyViewsRepository.GetFantasyNormalizedAveragesQueryable()
                .Where(fnp => fnp.FantasyPlayer.Id == FantasyPlayerId)
                .ToListAsync();
    }

    public async Task<List<MetadataSummary>> MetadataSummariesByFantasyLeagueAsync(FantasyLeague FantasyLeague)
    {
        _logger.LogInformation($"Fetching Metadata for Fantasy League Id: {FantasyLeague.Id}");

        List<MetadataSummary> metadataSummaries = await _fantasyViewsRepository.GetMetadataSummariesQueryable()
            .Where(ms => ms.FantasyLeagueId == FantasyLeague.Id)
            .Include(ms => ms.FantasyPlayer)
            .ToListAsync();

        return metadataSummaries;
    }

    public async Task<List<MatchHighlights>> GetLastNMatchHighlights(int FantasyLeagueId, int MatchCount)
    {
        _logger.LogInformation($"Getting {MatchCount} Match Highlights for Fantasy League ID: {FantasyLeagueId}");

        var matchHighlightsQuery = _fantasyViewsRepository.GetMatchHighlightsQueryable()
                .Include(mhv => mhv.FantasyPlayer)
                .Where(mhv => mhv.FantasyLeagueId == FantasyLeagueId)
                .OrderByDescending(mhv => mhv.StartTime)
                .Take(MatchCount);

        _logger.LogDebug($"Match Highlights Query: {matchHighlightsQuery.ToQueryString()}");

        return await matchHighlightsQuery.ToListAsync();
    }
}