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

    #region FantasyLeague

    public async Task<List<FantasyLeague>> GetFantasyLeaguesAsync(bool? IsActive)
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

    public async Task AddFantasyLeagueAsync(FantasyLeague newFantasyLeague)
    {
        _logger.LogInformation($"Adding new Fantasy League {newFantasyLeague.Name}");

        await _dbContext.FantasyLeagues.AddAsync(newFantasyLeague);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task DeleteFantasyLeagueAsync(FantasyLeague deleteFantasyLeague)
    {
        _logger.LogInformation($"Removing Fantasy League {deleteFantasyLeague.Name}");

        _dbContext.FantasyLeagues.Remove(deleteFantasyLeague);
        await _dbContext.SaveChangesAsync();

        return;
    }
    #endregion FantasyLeague


    #region FantasyPlayer

    public async Task<List<FantasyPlayer>> GetFantasyPlayersAsync(int? FantasyLeagueId)
    {
        _logger.LogInformation($"Get Fantasy Players by Fantasy League Id: {FantasyLeagueId}");

        var fantasyPlayerLeagueQuery = _dbContext.FantasyPlayers
            .Include(fp => fp.Team)
            .Include(fp => fp.DotaAccount)
            .Where(fp => fp.FantasyLeagueId == FantasyLeagueId || FantasyLeagueId == null)
            .OrderBy(fp => fp.Team.Name)
                .ThenBy(fp => fp.DotaAccount.Name);

        _logger.LogDebug($"Get Fantasy Players by Fantasy League Query: {fantasyPlayerLeagueQuery.ToQueryString()}");

        return await fantasyPlayerLeagueQuery.ToListAsync();
    }

    public async Task<FantasyPlayer?> GetFantasyPlayerAsync(int FantasyPlayerId)
    {
        _logger.LogInformation($"Get Single Fantasy Player {FantasyPlayerId}");

        return await _dbContext.FantasyPlayers.FindAsync(FantasyPlayerId);
    }

    public async Task AddFantasyPlayerAsync(FantasyPlayer newFantasyPlayer)
    {
        _logger.LogInformation($"Adding new Fantasy Player {newFantasyPlayer.DotaAccount.Name}");

        await _dbContext.FantasyPlayers.AddAsync(newFantasyPlayer);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task DeleteFantasyPlayerAsync(FantasyPlayer deleteFantasyPlayer)
    {
        _logger.LogInformation($"Removing Fantasy League {deleteFantasyPlayer.DotaAccount.Name}");

        _dbContext.FantasyPlayers.Remove(deleteFantasyPlayer);
        await _dbContext.SaveChangesAsync();

        return;
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
            .OrderByDescending(fpp => fpp.FantasyMatchPlayer!.MatchId)
            .ThenBy(fpp => fpp.FantasyMatchPlayer!.Team!.Name)
            .ThenBy(fpp => fpp.FantasyPlayer.TeamPosition)
            .Take(limit);

        _logger.LogDebug($"Match Details Query: {fantasyPlayerPointsByLeagueQuery.ToQueryString()}");

        return await fantasyPlayerPointsByLeagueQuery.ToListAsync();
    }

    public async Task<IEnumerable<FantasyPlayerPoints>> FantasyPlayerPointsByFantasyDraftAsync(long UserDiscordAccountId, int FantasyLeagueId, int limit)
    {
        _logger.LogInformation($"Fetching Fantasy Points for Fantasy League Id: {FantasyLeagueId}");

        List<FantasyPlayer> fantasyDraftPlayers = await _dbContext.FantasyDrafts
            .Where(fd => fd.FantasyLeagueId == FantasyLeagueId && fd.DiscordAccountId == UserDiscordAccountId)
            .Include(fd => fd.DraftPickPlayers)
                .ThenInclude(dpp => dpp.FantasyPlayer)
            .SelectMany(fd => fd.DraftPickPlayers)
            .Where(dpp => dpp.FantasyPlayer != null)
            .Select(dpp => dpp.FantasyPlayer!)
            .ToListAsync();

        var fantasyPlayerPointsByLeagueQuery = _dbContext.FantasyPlayerPointsView
            .Include(fppv => fppv.FantasyPlayer)
                .ThenInclude(fp => fp.DotaAccount)
            .Include(fppv => fppv.FantasyPlayer)
                .ThenInclude(fp => fp.Team)
            .Include(fppv => fppv.FantasyMatchPlayer)
                .ThenInclude(fmp => fmp!.Hero)
            .Where(fpp => fpp.FantasyLeagueId == FantasyLeagueId)
            .Where(fpp => fantasyDraftPlayers.Contains(fpp.FantasyPlayer))
            .Where(fpp => fpp.FantasyMatchPlayer != null)
            .OrderByDescending(fpp => fpp.FantasyMatchPlayer!.MatchId)
            .ThenBy(fpp => fpp.FantasyMatchPlayer!.Team!.Name)
            .ThenBy(fpp => fpp.FantasyPlayer.TeamPosition)
            .Take(limit);

        _logger.LogDebug($"Match Details Query: {fantasyPlayerPointsByLeagueQuery.ToQueryString()}");

        return await fantasyPlayerPointsByLeagueQuery.ToListAsync();
    }

    public async Task<List<FantasyPlayerPointTotals>> FantasyPlayerPointTotalsByFantasyLeagueAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching Fantasy Point Totals for Fantasy League Id: {FantasyLeagueId}");

        var fantasyPlayerTotalsQuery = _dbContext.FantasyPlayerPointTotalsView
            .Include(fppt => fppt.FantasyPlayer)
            .Where(fppt => fppt.FantasyLeagueId == FantasyLeagueId)
            .OrderByDescending(fppt => (double)fppt.TotalMatchFantasyPoints); // double casted needed for Sqlite: https://learn.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations

        _logger.LogDebug($"Match Details Query: {fantasyPlayerTotalsQuery.ToQueryString()}");

        return await fantasyPlayerTotalsQuery.ToListAsync();
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
                .Where(mdp => mdp.MatchPlayer.Account != null && mdp.MatchPlayer.Account.Id == fantasyPlayer.DotaAccountId)
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
                .Where(fnp => fnp.FantasyPlayerId == FantasyPlayerId)
                .ToListAsync();
    }
    #endregion FantasyPlayer

    public async Task<List<FantasyDraft>> FantasyDraftsByUserLeagueAsync(long UserDiscordAccountId, int FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching User {UserDiscordAccountId} Fantasy Draft for Fantasy League Id: {FantasyLeagueId}");

        var fantasyDraftsUserQuery = _dbContext.FantasyDrafts
            .Where(fd => fd.FantasyLeagueId == FantasyLeagueId && fd.DiscordAccountId == UserDiscordAccountId)
            .Include(fd => fd.DraftPickPlayers.Where(dpp => dpp.FantasyPlayer != null))
                .ThenInclude(dpp => dpp.FantasyPlayer)
                .ThenInclude(fp => fp!.Team)
            .Include(fd => fd.DraftPickPlayers.Where(dpp => dpp.FantasyPlayer != null))
                .ThenInclude(dpp => dpp.FantasyPlayer)
                .ThenInclude(fp => fp!.DotaAccount);

        _logger.LogDebug($"Fantasy Drafts by User and Fantasy League Query: {fantasyDraftsUserQuery.ToQueryString()}");

        return await fantasyDraftsUserQuery.ToListAsync();
    }

    public async Task<List<FantasyDraftPointTotals>> FantasyDraftPointsByFantasyLeagueAsync(int FantasyLeagueId)
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


    public async Task<List<MetadataSummary>> MetadataSummariesByFantasyLeagueAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching Metadata for Fantasy League Id: {FantasyLeagueId}");

        List<MetadataSummary> metadataSummaries = await _dbContext.MetadataSummaries
            .Where(ms => ms.FantasyLeagueId == FantasyLeagueId)
            .Include(ms => ms.FantasyPlayer)
            .ToListAsync();

        return metadataSummaries;
    }

    #endregion Fantasy

    #region FantasyMatch
    public async Task<FantasyMatch?> GetFantasyMatchAsync(long FantasyMatchId)
    {
        _logger.LogInformation($"Fetching Single Fantasy Match {FantasyMatchId}");

        return await _dbContext.FantasyMatches.FindAsync(FantasyMatchId);
    }

    public async Task AddFantasyMatchAsync(FantasyMatch newFantasyMatch)
    {
        _logger.LogInformation($"Adding new Fantasy Match {newFantasyMatch.MatchId}");

        await _dbContext.FantasyMatches.AddAsync(newFantasyMatch);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task AddFantasyMatchesAsync(IEnumerable<FantasyMatch> newFantasyMatches)
    {
        _logger.LogInformation($"Adding {newFantasyMatches.Count()} new Fantasy Matches");

        await _dbContext.FantasyMatches.AddRangeAsync(newFantasyMatches);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task UpdateFantasyMatchesAsync(IEnumerable<FantasyMatch> updateFantasyMatches)
    {
        _logger.LogInformation($"Updating {updateFantasyMatches.Count()} Fantasy Matches");

        _dbContext.FantasyMatches.UpdateRange(updateFantasyMatches);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task<List<FantasyMatch>> GetFantasyMatchesNotGcDetailParsed(int takeAmount)
    {
        _logger.LogInformation($"Getting Fantasy Matches not GC Match Detail Parsed");

        var matchDetailsToParse = _dbContext.FantasyMatches.Where(
                fm => !fm.MatchDetailParsed && _dbContext.GcDotaMatches.Any(md => (long)md.match_id == fm.MatchId))
                .Take(takeAmount);

        _logger.LogDebug($"GetFantasyMatchesNotGcDetailParsed SQL Query: {matchDetailsToParse.ToQueryString()}");

        return await matchDetailsToParse.ToListAsync();
    }

    public async Task<List<FantasyMatch>> GetFantasyMatchesNotDetailParsed(int takeAmount)
    {
        _logger.LogInformation($"Getting Fantasy Matches not Match Detail Parsed");

        var matchDetailsToParse = _dbContext.FantasyMatches.Where(
                fm => !fm.MatchDetailParsed && _dbContext.MatchDetails.Any(md => md.MatchId == fm.MatchId))
                .Take(takeAmount);

        _logger.LogDebug($"GetFantasyMatchesNotDetailParsed SQL Query: {matchDetailsToParse.ToQueryString()}");

        return await matchDetailsToParse.ToListAsync();
    }

    #endregion FantasyMatch

    #region FantasyMatchPlayer
    public async Task AddFantasyMatchPlayerAsync(FantasyMatchPlayer newFantasyMatchPlayer)
    {
        _logger.LogInformation($"Adding new Fantasy Match Player for Match {newFantasyMatchPlayer.MatchId}");

        await _dbContext.FantasyMatchPlayers.AddAsync(newFantasyMatchPlayer);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task UpdateFantasyMatchPlayersAsync(IEnumerable<FantasyMatchPlayer> updateFantasyMatchPlayers)
    {
        _logger.LogInformation($"Updating {updateFantasyMatchPlayers.Count()} Fantasy Match Players");

        _dbContext.FantasyMatchPlayers.UpdateRange(updateFantasyMatchPlayers);
        await _dbContext.SaveChangesAsync();

        return;
    }


    public async Task<List<FantasyMatchPlayer>> GetFantasyMatchPlayersNotGcDetailParsed(int takeAmount)
    {
        _logger.LogInformation($"Getting Fantasy Match Players not GC Match Detail Parsed");

        var matchDetailsToParse = _dbContext.FantasyMatchPlayers.Where(
                fm => !fm.GcMetadataPlayerParsed && _dbContext.GcMatchMetadata.Any(md => md.MatchId == fm.MatchId))
                .Take(takeAmount);

        _logger.LogDebug($"GetFantasyMatchPlayersNotGcDetailParsed SQL Query: {matchDetailsToParse.ToQueryString()}");

        return await matchDetailsToParse.ToListAsync();
    }
    #endregion FantasyMatchPlayer

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