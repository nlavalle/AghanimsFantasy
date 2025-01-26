using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Discord;
using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.ProMetadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccessLibrary.Facades;

public class FantasyDraftFacade
{
    private readonly ILogger<FantasyDraftFacade> _logger;
    private readonly IProMetadataRepository _proMetadataRepository;
    private readonly IDiscordUserRepository _discordUserRepository;
    private readonly IFantasyLeagueRepository _fantasyLeagueRepository;
    private readonly IFantasyDraftRepository _fantasyDraftRepository;
    private readonly IFantasyViewsRepository _fantasyViewsRepository;

    public FantasyDraftFacade(
        ILogger<FantasyDraftFacade> logger,
        IProMetadataRepository proMetadataRepository,
        IDiscordUserRepository discordUserRepository,
        IFantasyLeagueRepository fantasyLeagueRepository,
        IFantasyDraftRepository fantasyDraftRepository,
        IFantasyViewsRepository fantasyViewsRepository
    )
    {
        _logger = logger;
        _proMetadataRepository = proMetadataRepository;
        _discordUserRepository = discordUserRepository;
        _fantasyLeagueRepository = fantasyLeagueRepository;
        _fantasyDraftRepository = fantasyDraftRepository;
        _fantasyViewsRepository = fantasyViewsRepository;
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
        await _fantasyDraftRepository.UpdateAsync(fantasyDraft);

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
        await _fantasyDraftRepository.UpdateAsync(fantasyDraft);

        return fantasyDraft;
    }

    public async Task ClearPicksAsync(FantasyDraft fantasyDraft)
    {
        fantasyDraft.DraftPickPlayers.Clear();
        await _fantasyDraftRepository.UpdateAsync(fantasyDraft);

        return;
    }

    public async Task<FantasyDraftPointTotals?> DraftPointTotalAsync(FantasyDraft fantasyDraft)
    {
        _logger.LogInformation($"Fetching Fantasy Points for LeagueID: {fantasyDraft.FantasyLeagueId}");

        var fantasyLeague = fantasyDraft.FantasyLeague ?? await _fantasyLeagueRepository.GetByIdAsync(fantasyDraft.FantasyLeagueId) ?? throw new ArgumentException("Fantasy Draft has invalid fantasy league ID");

        var playerTotals = await FantasyPlayerPointTotalsByFantasyLeagueAsync(fantasyLeague);
        Team? teamDiscordIdLookup = null;
        DiscordUser? discordUserLookup = null;

        if (fantasyDraft.DiscordAccountId.HasValue)
        {
            teamDiscordIdLookup = await _proMetadataRepository.GetTeamAsync(fantasyDraft.DiscordAccountId.Value);
            discordUserLookup = await _discordUserRepository.GetByIdAsync(fantasyDraft.DiscordAccountId.Value);
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

        var allTeams = await _proMetadataRepository.GetTeamsAsync();
        var discordUsers = await _discordUserRepository.GetAllAsync();

        var fantasyDraftPointsByLeague = await _fantasyDraftRepository.GetQueryable()
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

        var fantasyPlayerTotalsQuery = _fantasyViewsRepository.GetPlayerPointTotalsQueryable()
            .Where(fppt => fppt.FantasyLeagueId == FantasyLeague.Id)
            .OrderByDescending(fppt => (double)fppt.TotalMatchFantasyPoints); // double casted needed for Sqlite: https://learn.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations

        _logger.LogDebug($"Match Details Query: {fantasyPlayerTotalsQuery.ToQueryString()}");

        return await fantasyPlayerTotalsQuery.ToListAsync();
    }

    public async Task<IEnumerable<FantasyPlayerPoints>> FantasyPlayerPointsAsync(FantasyDraft FantasyDraft, int limit)
    {
        _logger.LogInformation($"Fetching Fantasy Points for Fantasy League Id: {FantasyDraft.FantasyLeagueId}");

        var fantasyPlayerPointsByLeagueQuery = _fantasyViewsRepository.GetPlayerPointsQueryable()
            .Where(fpp => fpp.FantasyLeagueId == FantasyDraft.FantasyLeagueId)
            .Where(fpp => FantasyDraft.DraftPickPlayers.Any(dpp => dpp.FantasyPlayer == fpp.FantasyPlayer))
            .Where(fpp => fpp.FantasyMatchPlayer != null)
            .OrderByDescending(fpp => fpp.FantasyMatchPlayer!.FantasyMatchId)
            .ThenBy(fpp => fpp.FantasyMatchPlayer!.Team!.Name)
            .ThenBy(fpp => fpp.FantasyPlayer.TeamPosition)
            .Take(limit);

        _logger.LogDebug($"Match Details Query: {fantasyPlayerPointsByLeagueQuery.ToQueryString()}");

        return await fantasyPlayerPointsByLeagueQuery.ToListAsync();
    }
}