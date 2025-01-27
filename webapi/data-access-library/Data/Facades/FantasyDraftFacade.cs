namespace DataAccessLibrary.Data.Facades;

using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Discord;
using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.ProMetadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class FantasyDraftFacade
{
    private readonly ILogger<FantasyDraftFacade> _logger;
    private readonly AghanimsFantasyContext _dbContext;

    public FantasyDraftFacade(
        ILogger<FantasyDraftFacade> logger,
        AghanimsFantasyContext dbContext
    )
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<FantasyDraft?> GetByUserFantasyLeague(FantasyLeague fantasyLeague, DiscordUser user)
    {
        _logger.LogDebug($"Fetching Fantasy League {fantasyLeague.Name} Draft for User {user.Username}");

        var userFantasyDraftQuery = _dbContext.FantasyDrafts
                    .Include(fd => fd.DraftPickPlayers)
                    .Where(fd => fd.FantasyLeagueId == fantasyLeague.Id && fd.DiscordAccountId == user.Id);

        return await userFantasyDraftQuery.FirstOrDefaultAsync();
    }

    public async Task<FantasyDraft> AddPlayerPickAsync(FantasyDraft fantasyDraft, FantasyPlayer fantasyPlayerPick)
    {
        FantasyDraftPlayer? updateFantasyDraftPlayer = fantasyDraft.DraftPickPlayers
                                        .Where(fdp => fdp.DraftOrder == fantasyPlayerPick.TeamPosition)
                                        .FirstOrDefault();

        if (updateFantasyDraftPlayer == null)
        {
            // Create FantasyDraftPlayer join table record if it doesn't exist
            updateFantasyDraftPlayer = new FantasyDraftPlayer()
            {
                FantasyPlayerId = fantasyPlayerPick.Id,
                FantasyPlayer = fantasyPlayerPick,
                DraftOrder = fantasyPlayerPick.TeamPosition
            };
        }
        else
        {
            // Otherwise remove existing draft player lookup from draft pick players
            fantasyDraft.DraftPickPlayers.Remove(updateFantasyDraftPlayer);
        }

        updateFantasyDraftPlayer.FantasyPlayer = fantasyPlayerPick;
        fantasyDraft.DraftPickPlayers.Add(updateFantasyDraftPlayer);

        fantasyDraft.DraftLastUpdated = DateTime.UtcNow;
        _dbContext.Entry(fantasyDraft).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        return fantasyDraft;
    }

    public async Task<FantasyDraft> ClearPositionPick(FantasyDraft fantasyDraft, int PickPosition)
    {
        if (PickPosition > 5 || PickPosition < 1)
        {
            throw new Exception("Invalid Draft Order, must be between 1 to 5");
        }

        var currentDraftOrder = fantasyDraft.DraftPickPlayers.Where(dpp => dpp.DraftOrder == PickPosition).FirstOrDefault();
        if (currentDraftOrder != null)
        {
            fantasyDraft.DraftPickPlayers.Remove(currentDraftOrder);
        }

        fantasyDraft.DraftLastUpdated = DateTime.UtcNow;
        _dbContext.Entry(fantasyDraft).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        return fantasyDraft;
    }

    public async Task ClearPicksAsync(FantasyDraft fantasyDraft)
    {
        fantasyDraft.DraftPickPlayers.Clear();
        _dbContext.Entry(fantasyDraft).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task<FantasyDraftPointTotals?> DraftPointTotalAsync(FantasyDraft fantasyDraft)
    {
        _logger.LogInformation($"Fetching Fantasy Points for LeagueID: {fantasyDraft.FantasyLeagueId}");

        var fantasyLeague = fantasyDraft.FantasyLeague ?? await _dbContext.FantasyLeagues.FindAsync(fantasyDraft.FantasyLeagueId) ?? throw new ArgumentException("Fantasy Draft has invalid fantasy league ID");

        var playerTotals = await FantasyPlayerPointTotalsByFantasyLeagueAsync(fantasyLeague);
        Team? teamDiscordIdLookup = null;
        DiscordUser? discordUserLookup = null;

        if (fantasyDraft.DiscordAccountId.HasValue)
        {
            teamDiscordIdLookup = await _dbContext.Teams.FindAsync(fantasyDraft.DiscordAccountId.Value);
            discordUserLookup = await _dbContext.DiscordUsers.FindAsync(fantasyDraft.DiscordAccountId.Value);
        }

        var fantasyDraftPoints = new FantasyDraftPointTotals
        {
            FantasyDraft = fantasyDraft,
            DiscordName = discordUserLookup?.Username ?? "Unknown User",
            FantasyPlayerPoints = playerTotals.Where(pt => fantasyDraft.DraftPickPlayers.Select(dpp => dpp.FantasyPlayer!.Id).Contains(pt.FantasyPlayer.Id)).ToList()
        };

        return fantasyDraftPoints;
    }

    public async Task<List<FantasyDraftPointTotals>> AllDraftPointTotalsByFantasyLeagueAsync(FantasyLeague FantasyLeague)
    {
        _logger.LogInformation($"Fetching Fantasy Points for Fantasy League Id: {FantasyLeague.Id}");

        var playerTotals = await FantasyPlayerPointTotalsByFantasyLeagueAsync(FantasyLeague);

        var allTeams = await _dbContext.Teams.ToListAsync();
        var discordUsers = await _dbContext.DiscordUsers.ToListAsync();

        var fantasyDraftPointsByLeague = await _dbContext.FantasyDrafts
            .Where(fl => fl.FantasyLeagueId == FantasyLeague.Id)
            .ToListAsync();

        var fantasyDraftPoints = fantasyDraftPointsByLeague
            .Where(fd => fd.DraftPickPlayers.Count > 0)
            .Select(fd => new FantasyDraftPointTotals
            {
                FantasyDraft = fd,
                DiscordName = discordUsers.FirstOrDefault(u => u.Id == (fd.DiscordAccountId.HasValue ? fd.DiscordAccountId.Value : 0))?.Username ?? "Unknown User",
                FantasyPlayerPoints = playerTotals.Where(pt => fd.DraftPickPlayers.Select(dpp => dpp.FantasyPlayer!.Id).Contains(pt.FantasyPlayer.Id)).ToList()
            })
            .ToList();

        return fantasyDraftPoints;
    }

    public async Task<List<FantasyPlayerPointTotals>> FantasyPlayerPointTotalsByFantasyLeagueAsync(FantasyLeague FantasyLeague)
    {
        _logger.LogInformation($"Fetching Fantasy Point Totals for Fantasy League Id: {FantasyLeague.Id}");

        var fantasyPlayerTotalsQuery = _dbContext.FantasyPlayerPointTotalsView
            .Include(fppv => fppv.FantasyPlayer)
            .Where(fppt => fppt.FantasyLeagueId == FantasyLeague.Id)
            .OrderByDescending(fppt => (double)fppt.TotalMatchFantasyPoints); // double casted needed for Sqlite: https://learn.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations

        _logger.LogDebug($"Match Details Query: {fantasyPlayerTotalsQuery.ToQueryString()}");

        return await fantasyPlayerTotalsQuery.ToListAsync();
    }

    public async Task<IEnumerable<FantasyPlayerPoints>> FantasyPlayerPointsAsync(FantasyDraft FantasyDraft, int limit)
    {
        _logger.LogInformation($"Fetching Fantasy Points for Fantasy League Id: {FantasyDraft.FantasyLeagueId}");

        List<FantasyPlayer> fantasyDraftPlayers = await _dbContext.FantasyDraftPlayers
            .Where(dpp => dpp.FantasyDraftId == FantasyDraft.Id)
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
            .Where(fpp => fpp.FantasyLeagueId == FantasyDraft.FantasyLeagueId)
            .Where(fpp => fantasyDraftPlayers.Contains(fpp.FantasyPlayer))
            .Where(fpp => fpp.FantasyMatchPlayer != null)
            .OrderByDescending(fpp => fpp.FantasyMatchPlayer!.FantasyMatchId)
            .ThenBy(fpp => fpp.FantasyMatchPlayer!.Team!.Name)
            .ThenBy(fpp => fpp.FantasyPlayer.TeamPosition)
            .Take(limit);

        _logger.LogDebug($"Match Details Query: {fantasyPlayerPointsByLeagueQuery.ToQueryString()}");

        return await fantasyPlayerPointsByLeagueQuery.ToListAsync();
    }
}