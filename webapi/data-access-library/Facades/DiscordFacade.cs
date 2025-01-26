using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Discord;
using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.ProMetadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccessLibrary.Facades;

public class DiscordFacade
{
    private readonly ILogger<DiscordFacade> _logger;
    private readonly IDiscordUserRepository _discordUserRepository;
    private readonly IDiscordOutboxRepository _discordOutboxRepository;
    private readonly IFantasyLeagueRepository _fantasyLeagueRepository;
    private readonly IFantasyDraftRepository _fantasyDraftRepository;
    private readonly IFantasyViewsRepository _fantasyViewsRepository;
    private readonly IPrivateFantasyPlayerRepository _privateFantasyPlayerRepository;

    public DiscordFacade(
        ILogger<DiscordFacade> logger,
        IDiscordUserRepository discordUserRepository,
        IDiscordOutboxRepository discordOutboxRepository,
        IFantasyLeagueRepository fantasyLeagueRepository,
        IFantasyDraftRepository fantasyDraftRepository,
        IFantasyViewsRepository fantasyViewsRepository,
        IPrivateFantasyPlayerRepository privateFantasyPlayerRepository
    )
    {
        _logger = logger;
        _discordUserRepository = discordUserRepository;
        _discordOutboxRepository = discordOutboxRepository;
        _fantasyLeagueRepository = fantasyLeagueRepository;
        _fantasyDraftRepository = fantasyDraftRepository;
        _fantasyViewsRepository = fantasyViewsRepository;
        _privateFantasyPlayerRepository = privateFantasyPlayerRepository;
    }

    public async Task<List<FantasyMatch>> GetFantasyMatchesNotInDiscordOutboxAsync()
    {
        var outboxMessages = await _discordOutboxRepository.GetQueryable().Where(ob => ob.EventObject == "FantasyMatch" && ob.EventType == "Scored").ToListAsync();
        var fantasyMatches = await _fantasyViewsRepository.GetPlayerPointsQueryable().Where(fppv => fppv.FantasyMatchPlayerId != null && fppv.FantasyMatchPlayer!.Match != null).Select(fppv => fppv.FantasyMatchPlayer!.Match!).ToListAsync();

        return fantasyMatches.Where(fm => !outboxMessages.Any(obm => obm.ObjectKey == fm.MatchId.ToString())).Distinct().ToList();
    }

    public async Task<bool> IsUserAdminAsync(long UserDiscordId)
    {
        _logger.LogDebug($"Checking if Discord ID {UserDiscordId} is admin.");

        var discordUser = await _discordUserRepository.GetByIdAsync(UserDiscordId);
        if (discordUser == null) return false;

        if (discordUser.IsAdmin) return true;

        return false;
    }

    public async Task<bool> IsUserPrivateFantasyAdminAsync(long UserDiscordId)
    {
        _logger.LogDebug($"Checking if Discord ID {UserDiscordId} is admin for any private fantasy league.");

        var discordUser = await _privateFantasyPlayerRepository.GetByDiscordIdAsync(UserDiscordId);
        if (discordUser == null) return false;

        if (discordUser.IsAdmin) return true;

        return false;
    }

    public async Task<bool> IsUserPrivateFantasyAdminAsync(long UserDiscordId, long FantasyLeagueId)
    {
        _logger.LogDebug($"Checking if Discord ID {UserDiscordId} is admin for a private fantasy league {FantasyLeagueId}.");

        var discordUser = await _privateFantasyPlayerRepository.GetQueryable().Where(fplp => fplp.FantasyLeagueId == FantasyLeagueId && fplp.DiscordUserId == UserDiscordId).FirstOrDefaultAsync();
        if (discordUser == null) return false;

        if (discordUser.IsAdmin) return true;

        return false;
    }
}