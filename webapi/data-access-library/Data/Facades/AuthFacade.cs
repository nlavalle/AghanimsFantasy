namespace DataAccessLibrary.Data.Facades;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class AuthFacade
{
    private readonly ILogger<AuthFacade> _logger;
    private readonly AghanimsFantasyContext _dbContext;
    public AuthFacade(
        ILogger<AuthFacade> logger,
        AghanimsFantasyContext dbContext
    )
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<bool> IsUserAdminAsync(long UserDiscordId)
    {
        _logger.LogDebug($"Checking if Discord ID {UserDiscordId} is admin.");

        var discordUser = await _dbContext.DiscordUsers.FindAsync(UserDiscordId);
        if (discordUser == null) return false;

        if (discordUser.IsAdmin) return true;

        return false;
    }

    public async Task<bool> IsUserPrivateFantasyAdminAsync(long UserDiscordId)
    {
        _logger.LogDebug($"Checking if Discord ID {UserDiscordId} is admin for any private fantasy league.");

        var discordUser = await _dbContext.FantasyPrivateLeaguePlayers.Where(fplp => fplp.DiscordUserId == UserDiscordId).ToListAsync();
        if (discordUser == null) return false;

        if (discordUser.Where(du => du.IsAdmin).Count() > 0) return true;

        return false;
    }

    public async Task<bool> IsUserPrivateFantasyAdminAsync(long UserDiscordId, long FantasyLeagueId)
    {
        _logger.LogDebug($"Checking if Discord ID {UserDiscordId} is admin for a private fantasy league {FantasyLeagueId}.");

        var discordUser = await _dbContext.FantasyPrivateLeaguePlayers.Where(fplp => fplp.FantasyLeagueId == FantasyLeagueId && fplp.DiscordUserId == UserDiscordId).FirstOrDefaultAsync();
        if (discordUser == null) return false;

        if (discordUser.IsAdmin) return true;

        return false;
    }
}