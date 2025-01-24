namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models.Discord;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Repository to handle all of the metadata operations (leagues, players, teams) and none of the specific
/// game coordinator, webapi, fantasy, or discord repositories
/// </summary>
public class DiscordRepository : IDiscordRepository
{
    private readonly ILogger<DiscordRepository> _logger;
    private AghanimsFantasyContext _dbContext;
    public DiscordRepository(ILogger<DiscordRepository> logger, AghanimsFantasyContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<List<FantasyMatch>> GetFantasyMatchesNotInDiscordOutboxAsync()
    {
        var outboxMessages = await _dbContext.DiscordOutbox.Where(ob => ob.EventObject == "FantasyMatch" && ob.EventType == "Scored").ToListAsync();
        var fantasyMatches = await _dbContext.FantasyPlayerPointsView.Where(fppv => fppv.FantasyMatchPlayerId != null && fppv.FantasyMatchPlayer!.Match != null).Select(fppv => fppv.FantasyMatchPlayer!.Match!).ToListAsync();

        return fantasyMatches.Where(fm => !outboxMessages.Any(obm => obm.ObjectKey == fm.MatchId.ToString())).Distinct().ToList();
    }

    #region DiscordOutbox
    public async Task<List<DiscordOutbox>> GetOutboxMessagesAsync(string getObjectKey, string getEventObject)
    {
        return await _dbContext.DiscordOutbox.Where(ob => ob.ObjectKey == getObjectKey && ob.EventObject == getEventObject).ToListAsync();
    }

    public async Task AddDiscordOutboxAsync(DiscordOutbox newDiscordOutbox)
    {
        _logger.LogInformation($"Adding Discord Outbox Message {newDiscordOutbox.EventObject} {newDiscordOutbox.EventType}");

        await _dbContext.DiscordOutbox.AddAsync(newDiscordOutbox);
        await _dbContext.SaveChangesAsync();
    }

    #endregion DiscordOutbox

    #region DiscordUser
    public async Task<DiscordUser?> GetDiscordUserAsync(long GetDiscordId)
    {
        _logger.LogInformation($"Getting Discord User {GetDiscordId}");

        return await _dbContext.DiscordUsers.Where(di => di.Id == GetDiscordId).FirstOrDefaultAsync();
    }

    public async Task AddDiscordUserAsync(DiscordUser newDiscordUser)
    {
        _logger.LogInformation($"Adding Discord User {newDiscordUser.Username}");

        await _dbContext.DiscordUsers.AddAsync(newDiscordUser);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task<bool> IsUserAdminAsync(long UserDiscordId)
    {
        _logger.LogDebug($"Checking if Discord ID {UserDiscordId} is admin.");

        var discordUser = await _dbContext.DiscordUsers.FirstOrDefaultAsync(di => di.Id == UserDiscordId);
        if (discordUser == null) return false;

        if (discordUser.IsAdmin) return true;

        return false;
    }

    public async Task<bool> IsUserPrivateFantasyAdminAsync(long UserDiscordId)
    {
        _logger.LogDebug($"Checking if Discord ID {UserDiscordId} is admin for any private fantasy league.");

        var discordUser = await _dbContext.fantasyPrivateLeaguePlayers.Where(fplp => fplp.DiscordUserId == UserDiscordId).FirstOrDefaultAsync();
        if (discordUser == null) return false;

        if (discordUser.IsAdmin) return true;

        return false;
    }

    public async Task<bool> IsUserPrivateFantasyAdminAsync(long UserDiscordId, long FantasyLeagueId)
    {
        _logger.LogDebug($"Checking if Discord ID {UserDiscordId} is admin for a private fantasy league {FantasyLeagueId}.");

        var discordUser = await _dbContext.fantasyPrivateLeaguePlayers.Where(fplp => fplp.FantasyLeagueId == FantasyLeagueId && fplp.DiscordUserId == UserDiscordId).FirstOrDefaultAsync();
        if (discordUser == null) return false;

        if (discordUser.IsAdmin) return true;

        return false;
    }
    #endregion DiscordUser
}