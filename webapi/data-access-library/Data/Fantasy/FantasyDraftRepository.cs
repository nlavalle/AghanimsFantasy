namespace DataAccessLibrary.Data;

using System.Threading.Tasks;
using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Discord;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Service to fetch and transform the Postgres data to read/write fantasy draft data for the webapi
/// example is to fetch all of the current scores, vs adding a new draft. Controllers should handle none of the business
/// logic 
/// </summary>
public class FantasyDraftRepository : IFantasyDraftRepository
{
    private readonly ILogger<FantasyDraftRepository> _logger;
    private AghanimsFantasyContext _dbContext;
    public FantasyDraftRepository(ILogger<FantasyDraftRepository> logger, AghanimsFantasyContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
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
        _dbContext.FantasyDrafts.Update(fantasyDraft);

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
        _dbContext.FantasyDrafts.Update(fantasyDraft);

        await _dbContext.SaveChangesAsync();

        return fantasyDraft;
    }

    public async Task ClearPicksAsync(FantasyDraft fantasyDraft)
    {
        fantasyDraft.DraftPickPlayers.Clear();
        _dbContext.FantasyDrafts.Update(fantasyDraft);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task<List<FantasyPlayerPointTotals>> FantasyPlayerPointTotalsByFantasyLeagueAsync(FantasyLeague FantasyLeague)
    {
        _logger.LogInformation($"Fetching Fantasy Point Totals for Fantasy League Id: {FantasyLeague.Id}");

        var fantasyPlayerTotalsQuery = _dbContext.FantasyPlayerPointTotalsView
            .Include(fppt => fppt.FantasyPlayer)
            .Where(fppt => fppt.FantasyLeagueId == FantasyLeague.Id)
            .OrderByDescending(fppt => (double)fppt.TotalMatchFantasyPoints); // double casted needed for Sqlite: https://learn.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations

        _logger.LogDebug($"Match Details Query: {fantasyPlayerTotalsQuery.ToQueryString()}");

        return await fantasyPlayerTotalsQuery.ToListAsync();
    }

    public async Task<IEnumerable<FantasyPlayerPoints>> FantasyPlayerPointsAsync(FantasyDraft FantasyDraft, int limit)
    {
        _logger.LogInformation($"Fetching Fantasy Points for Fantasy League Id: {FantasyDraft.FantasyLeagueId}");

        List<FantasyPlayer> fantasyDraftPlayers = FantasyDraft.DraftPickPlayers
            .Where(dpp => dpp.FantasyPlayer != null)
            .Select(dpp => dpp.FantasyPlayer!)
            .ToList();

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

    public async Task<FantasyDraftPointTotals?> DraftPointTotalAsync(FantasyDraft fantasyDraft)
    {
        _logger.LogInformation($"Fetching Fantasy Points for LeagueID: {fantasyDraft.FantasyLeagueId}");

        var fantasyLeague = fantasyDraft.FantasyLeague ?? await _dbContext.FantasyLeagues.FindAsync(fantasyDraft.FantasyLeagueId) ?? throw new ArgumentException("Fantasy Draft has invalid fantasy league ID");

        var playerTotals = await FantasyPlayerPointTotalsByFantasyLeagueAsync(fantasyLeague);

        var fantasyDraftPoints = new FantasyDraftPointTotals
        {
            FantasyDraft = fantasyDraft,
            IsTeam = _dbContext.Teams.Select(t => t.Id).Any(t => t == fantasyDraft.DiscordAccountId),
            TeamId = _dbContext.Teams.Select(t => t.Id).Any(t => t == fantasyDraft.DiscordAccountId) ?
                    _dbContext.Teams.FirstOrDefault(t => t.Id == fantasyDraft.DiscordAccountId)?.Id ?? -1 :
                    null,
            DiscordName = _dbContext.Teams.Select(t => t.Id).Any(t => t == fantasyDraft.DiscordAccountId) ?
                    _dbContext.Teams.FirstOrDefault(t => t.Id == fantasyDraft.DiscordAccountId)?.Name ?? "Unknown Team" :
                    _dbContext.DiscordUsers.FirstOrDefault(d => d.Id == fantasyDraft.DiscordAccountId)?.Username ?? "Unknown User",
            FantasyPlayerPoints = playerTotals.Where(pt => fantasyDraft.DraftPickPlayers.Select(dpp => dpp.FantasyPlayer!.Id).Contains(pt.FantasyPlayer.Id)).ToList()
        };

        return fantasyDraftPoints;
    }

    public async Task<List<FantasyDraftPointTotals>> AllDraftPointTotalsByFantasyLeagueAsync(FantasyLeague FantasyLeague)
    {
        _logger.LogInformation($"Fetching Fantasy Points for Fantasy League Id: {FantasyLeague.Id}");

        var playerTotals = await FantasyPlayerPointTotalsByFantasyLeagueAsync(FantasyLeague);

        var fantasyDraftPointsByLeagueQuery = await _dbContext.FantasyDrafts
            .Where(fl => fl.FantasyLeagueId == FantasyLeague.Id)
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
                FantasyPlayerPoints = playerTotals.Where(pt => fd.DraftPickPlayers.Select(dpp => dpp.FantasyPlayer!.Id).Contains(pt.FantasyPlayer.Id)).ToList()
            })
            .ToList()
            ;

        return fantasyDraftPoints;
    }

    public async Task<FantasyDraft?> GetByUserFantasyLeague(FantasyLeague fantasyLeague, DiscordUser user)
    {
        _logger.LogInformation($"Fetching Fantasy League {fantasyLeague.Name} Draft for User {user.Username}");

        var userFantasyDraftQuery = _dbContext.FantasyDrafts
                    .Include(fd => fd.DraftPickPlayers)
                    .Where(fd => fd.FantasyLeagueId == fantasyLeague.Id && fd.DiscordAccountId == user.Id);

        return await userFantasyDraftQuery.FirstOrDefaultAsync();
    }

    public async Task<FantasyDraft?> GetByIdAsync(long FantasyDraftId)
    {
        _logger.LogInformation($"Fetching Single Fantasy Draft {FantasyDraftId}");

        return await _dbContext.FantasyDrafts.FindAsync(FantasyDraftId);
    }

    public async Task<IEnumerable<FantasyDraft>> GetAllAsync()
    {
        _logger.LogInformation($"Get Fantasy Drafts");

        return await _dbContext.FantasyDrafts.ToListAsync();
    }

    public async Task AddAsync(FantasyDraft addFantasyDraft)
    {
        _logger.LogInformation($"Adding new Fantasy Draft for User {addFantasyDraft.DiscordAccountId}");

        await _dbContext.FantasyDrafts.AddAsync(addFantasyDraft);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task DeleteAsync(FantasyDraft deleteFantasyDraft)
    {
        _logger.LogInformation($"Removing Fantasy Draft {deleteFantasyDraft.Id}");

        _dbContext.FantasyDrafts.Remove(deleteFantasyDraft);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task UpdateAsync(FantasyDraft updateFantasyDraft)
    {
        _logger.LogInformation($"Updating Fantasy Draft {updateFantasyDraft.Id}");

        _dbContext.Entry(updateFantasyDraft).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        return;
    }
}