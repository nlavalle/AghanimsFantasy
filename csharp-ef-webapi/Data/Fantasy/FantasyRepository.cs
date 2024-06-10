using csharp_ef_webapi.Models;
using csharp_ef_webapi.Models.Fantasy;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace csharp_ef_webapi.Data;

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

    public async Task<IEnumerable<FantasyLeague>> GetFantasyLeaguesAsync(bool? IsActive)
    {
        _logger.LogInformation($"Fetching All Fantasy Leagues");

        return await _dbContext.FantasyLeagues
                .Where(l => IsActive == null || l.IsActive == IsActive)
                .ToListAsync();
    }

    public async Task<FantasyLeague?> GetFantasyLeagueAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching Single Fantasy League {FantasyLeagueId}");

        return await _dbContext.FantasyLeagues.FindAsync(FantasyLeagueId);
    }

    public async Task<IEnumerable<FantasyPlayer>> FantasyPlayersByFantasyLeagueAsync(int? FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching Fantasy Players Fantasy League Id: {FantasyLeagueId}");

        var fantasyPlayerLeagueQuery = _dbContext.FantasyPlayers
            .Include(fp => fp.Team)
            .Include(fp => fp.DotaAccount)
            .Where(fp => fp.FantasyLeagueId == FantasyLeagueId || FantasyLeagueId == null)
            .OrderBy(fp => fp.Team.Name)
                .ThenBy(fp => fp.DotaAccount.Name);

        _logger.LogDebug($"Fantasy Players by Fantasy League Query: {fantasyPlayerLeagueQuery.ToQueryString()}");

        return await fantasyPlayerLeagueQuery.ToListAsync();
    }

    public async Task<IEnumerable<FantasyDraft>> FantasyDraftsByUserLeagueAsync(long UserDiscordAccountId, int FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching User {UserDiscordAccountId} Fantasy Draft for Fantasy League Id: {FantasyLeagueId}");

        var fantasyDraftsUserQuery = _dbContext.FantasyDrafts
            .Where(fd => fd.FantasyLeagueId == FantasyLeagueId && fd.DiscordAccountId == UserDiscordAccountId)
            .Include(fd => fd.DraftPickPlayers)
                .ThenInclude(fp => fp.FantasyPlayer)
                .ThenInclude(fp => fp.Team)
            .Include(fd => fd.DraftPickPlayers)
                .ThenInclude(fp => fp.FantasyPlayer)
                .ThenInclude(fp => fp.DotaAccount);

        _logger.LogDebug($"Fantasy Drafts by User and Fantasy League Query: {fantasyDraftsUserQuery.ToQueryString()}");

        return await fantasyDraftsUserQuery.ToListAsync();
    }

    public async Task<IEnumerable<FantasyPlayerPoints>> FantasyPlayerPointsByFantasyLeagueAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching Fantasy Points for Fantasy League Id: {FantasyLeagueId}");

        var fantasyPlayerPointsByLeagueQuery = _dbContext.FantasyPlayerPointsView
            .Include(fppv => fppv.FantasyPlayer)
            .Include(fppv => fppv.Match)
            .Where(fpp => fpp.FantasyLeagueId == FantasyLeagueId);

        _logger.LogDebug($"Match Details Query: {fantasyPlayerPointsByLeagueQuery.ToQueryString()}");

        return await fantasyPlayerPointsByLeagueQuery.ToListAsync();
    }

    public async Task<IEnumerable<FantasyPlayerPointTotals>> FantasyPlayerPointTotalsByFantasyLeagueAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching Fantasy Point Totals for Fantasy League Id: {FantasyLeagueId}");

        var fantasyPlayerTotalsQuery = _dbContext.FantasyPlayerPointTotalsView
            .Include(fppt => fppt.FantasyPlayer)
            .Where(fppt => fppt.FantasyLeagueId == FantasyLeagueId)
            .OrderByDescending(fppt => (double)fppt.TotalMatchFantasyPoints); // double casted needed for Sqlite: https://learn.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations

        _logger.LogDebug($"Match Details Query: {fantasyPlayerTotalsQuery.ToQueryString()}");

        return await fantasyPlayerTotalsQuery.ToListAsync();
    }

    public async Task<IEnumerable<FantasyDraftPointTotals>> FantasyDraftPointsByFantasyLeagueAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching Fantasy Points for Fantasy League Id: {FantasyLeagueId}");

        var playerTotals = await _dbContext.FantasyPlayerPointTotalsView
            .Include(fppt => fppt.FantasyPlayer)
            .Where(fppt => fppt.FantasyLeagueId == FantasyLeagueId)
            .OrderByDescending(fppt => fppt.TotalMatchFantasyPoints)
            .ToListAsync();

        var fantasyDraftPointsByLeagueQuery = await _dbContext.FantasyDrafts
            .Where(fl => fl.FantasyLeagueId == FantasyLeagueId)
            .ToListAsync();

        var fantasyDraftPoints = fantasyDraftPointsByLeagueQuery
            .Select(fd => new FantasyDraftPointTotals
            {
                FantasyDraft = fd,
                IsTeam = _dbContext.Teams.Select(t => t.Id).Any(t => t == fd.DiscordAccountId),
                TeamId = _dbContext.Teams.Select(t => t.Id).Any(t => t == fd.DiscordAccountId) ?
                    _dbContext.Teams.FirstOrDefault(t => t.Id == fd.DiscordAccountId)?.Id ?? -1 :
                    null,
                DiscordName = _dbContext.Teams.Select(t => t.Id).Any(t => t == fd.DiscordAccountId) ?
                    _dbContext.Teams.FirstOrDefault(t => t.Id == fd.DiscordAccountId)?.Name ?? "Unknown Team" :
                    _dbContext.DiscordUsers.FirstOrDefault(d => d.Id == fd.DiscordAccountId)?.Username ?? "Unknown User",
                FantasyPlayerPoints = playerTotals.Where(pt => fd.DraftPickPlayers.Select(dpp => dpp.FantasyPlayerId).Contains(pt.FantasyPlayer.Id)).ToList()
            })
            .ToList()
            ;

        return fantasyDraftPoints;
    }

    public async Task<FantasyDraftPointTotals?> FantasyDraftPointsByUserLeagueAsync(long UserDiscordAccountId, int FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching Fantasy Points for LeagueID: {FantasyLeagueId}");

        // var fantasyPlayerPoints = await FantasyPlayerPointsByFantasyLeagueAsync(FantasyLeagueId);
        // var playerTotals = AggregateFantasyPlayerPoints(fantasyPlayerPoints).ToList();
        var playerTotals = await _dbContext.FantasyPlayerPointTotalsView
            .Include(fppt => fppt.FantasyPlayer)
            .Where(fppt => fppt.FantasyLeagueId == FantasyLeagueId)
            .OrderByDescending(fppt => fppt.TotalMatchFantasyPoints)
            .ToListAsync();

        var fantasyDraftPointsByUserLeague = await _dbContext.FantasyDrafts
            .Where(fd => fd.FantasyLeagueId == FantasyLeagueId && fd.DiscordAccountId == UserDiscordAccountId)
            .FirstOrDefaultAsync();

        if (fantasyDraftPointsByUserLeague == null)
        {
            return null;
        }
        else
        {
            var fantasyDraftPoints = new FantasyDraftPointTotals
            {
                FantasyDraft = fantasyDraftPointsByUserLeague,
                IsTeam = _dbContext.Teams.Select(t => t.Id).Any(t => t == fantasyDraftPointsByUserLeague.DiscordAccountId),
                TeamId = _dbContext.Teams.Select(t => t.Id).Any(t => t == fantasyDraftPointsByUserLeague.DiscordAccountId) ?
                        _dbContext.Teams.FirstOrDefault(t => t.Id == fantasyDraftPointsByUserLeague.DiscordAccountId)?.Id ?? -1 :
                        null,
                DiscordName = _dbContext.Teams.Select(t => t.Id).Any(t => t == fantasyDraftPointsByUserLeague.DiscordAccountId) ?
                        _dbContext.Teams.FirstOrDefault(t => t.Id == fantasyDraftPointsByUserLeague.DiscordAccountId)?.Name ?? "Unknown Team" :
                        _dbContext.DiscordUsers.FirstOrDefault(d => d.Id == fantasyDraftPointsByUserLeague.DiscordAccountId)?.Username ?? "Unknown User",
                FantasyPlayerPoints = playerTotals.Where(pt => fantasyDraftPointsByUserLeague.DraftPickPlayers.Select(dpp => dpp.FantasyPlayerId).Contains(pt.FantasyPlayer.Id)).ToList()
            };

            return fantasyDraftPoints;
        }
    }

    public async Task<DateTime> GetLeagueLockedDate(int FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching Draft Locked Date for Fantasy League Id: {FantasyLeagueId}");

        return DateTimeOffset.FromUnixTimeSeconds(
                await _dbContext.FantasyLeagues.Where(l => l.Id == FantasyLeagueId).Select(l => l.FantasyDraftLocked).FirstOrDefaultAsync()
            ).DateTime;
    }

    public async Task ClearUserFantasyPlayersAsync(long UserDiscordAccountId, int FantasyLeagueId)
    {
        var updateDraft = await _dbContext.FantasyDrafts
            .Include(fd => fd.DraftPickPlayers)
            .Where(fd => fd.FantasyLeagueId == FantasyLeagueId && fd.DiscordAccountId == UserDiscordAccountId)
            .FirstOrDefaultAsync();

        if (updateDraft == null)
        {
            return;
        }

        updateDraft.DraftPickPlayers.Clear();
        _dbContext.FantasyDrafts.Update(updateDraft);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task<FantasyDraft> AddNewUserFantasyPlayerAsync(long UserDiscordAccountId, int FantasyLeagueId, long? FantasyPlayerId, int DraftOrder)
    {
        // We will receive a 0 if the user wants to clear the draft pick, so we can avoid nulls
        if (DraftOrder > 5 || DraftOrder < 1)
        {
            throw new Exception("Invalid Draft Order, must be between 1 to 5");
        }

        var updateDraft = await _dbContext.FantasyDrafts
            .Include(fd => fd.DraftPickPlayers)
            .Where(fd => fd.FantasyLeagueId == FantasyLeagueId && fd.DiscordAccountId == UserDiscordAccountId)
            .FirstOrDefaultAsync();

        if (updateDraft == null)
        {
            // User hasn't created a draft yet, so we'll create that
            updateDraft = new FantasyDraft
            {
                DiscordAccountId = UserDiscordAccountId,
                FantasyLeagueId = FantasyLeagueId,
                DraftCreated = DateTime.UtcNow,
            };
            _dbContext.FantasyDrafts.Add(updateDraft);
            await _dbContext.SaveChangesAsync();
        }

        if (FantasyPlayerId == null)
        {
            var currentDraftOrder = updateDraft.DraftPickPlayers.Where(dpp => dpp.DraftOrder == DraftOrder).FirstOrDefault();
            if (currentDraftOrder != null)
            {
                updateDraft.DraftPickPlayers.Remove(currentDraftOrder);
            }
        }
        else
        {
            FantasyPlayer fantasyPlayerLookup = await _dbContext.FantasyPlayers.FindAsync(FantasyPlayerId) ?? throw new Exception("Invalid Fantasy Player ID");

            FantasyDraftPlayer? updateFantasyDraftPlayer = await _dbContext.FantasyDrafts
                                            .Where(fd => fd.Id == updateDraft.Id)
                                            .SelectMany(fd => fd.DraftPickPlayers)
                                            .Where(fdp => fdp.DraftOrder == DraftOrder)
                                            .FirstOrDefaultAsync();

            if (updateFantasyDraftPlayer == null)
            {
                // Create FantasyDraftPlayer join table record if it doesn't exist
                updateFantasyDraftPlayer = new FantasyDraftPlayer() { FantasyPlayer = fantasyPlayerLookup, DraftOrder = DraftOrder };
            }
            else
            {
                // Otherwise remove existing draft player lookup from draft pick players
                updateDraft.DraftPickPlayers.Remove(updateFantasyDraftPlayer);
            }

            updateFantasyDraftPlayer.FantasyPlayer = fantasyPlayerLookup;
            updateDraft.DraftPickPlayers.Add(updateFantasyDraftPlayer);
        }

        updateDraft.DraftLastUpdated = DateTime.UtcNow;
        _dbContext.FantasyDrafts.Update(updateDraft);

        await _dbContext.SaveChangesAsync();

        return updateDraft;
    }
    #endregion Fantasy

    #region Match

    public async Task<IEnumerable<MatchHighlights>> GetLastNMatchHighlights(int FantasyLeagueId, int MatchCount)
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

    #region Player

    public async Task<IEnumerable<FantasyNormalizedAveragesTable>> GetFantasyNormalizedAveragesAsync(long FantasyPlayerId)
    {
        _logger.LogInformation($"Getting Player Averages");

        return await _dbContext.FantasyNormalizedAverages
                .Where(fnp => fnp.FantasyPlayerId == FantasyPlayerId)
                .ToListAsync();
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

        var topHeroIds = await _dbContext.MatchDetails
                .SelectMany(
                    md => md.Players,
                    (left, right) => new { MatchDetail = left, MatchDetailPlayer = right }
                )
                .Where(mdp => mdp.MatchDetailPlayer.AccountId == fantasyPlayer.DotaAccountId)
                .GroupBy(match => match.MatchDetailPlayer.HeroId)
                .Select(group => new
                {
                    HeroId = group.Key,
                    Count = group.Count(),
                    Wins = group.Where(g => (g.MatchDetail.RadiantWin && g.MatchDetailPlayer.TeamNumber == 0) ||
                    (!g.MatchDetail.RadiantWin && g.MatchDetailPlayer.TeamNumber == 1)).Count(),
                    Losses = group.Where(g => (!g.MatchDetail.RadiantWin && g.MatchDetailPlayer.TeamNumber == 0) ||
                    (g.MatchDetail.RadiantWin && g.MatchDetailPlayer.TeamNumber == 1)).Count()
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

    #endregion Player
}