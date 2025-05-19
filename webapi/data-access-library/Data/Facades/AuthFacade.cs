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

    public async Task<bool> IsUserPrivateFantasyAdminAsync(string UserId, long FantasyLeagueId)
    {
        _logger.LogDebug($"Checking if User ID {UserId} is admin for a private fantasy league {FantasyLeagueId}.");

        var user = await _dbContext.FantasyPrivateLeaguePlayers.Where(fplp => fplp.FantasyLeagueId == FantasyLeagueId && fplp.UserId == UserId).FirstOrDefaultAsync();
        if (user == null) return false;

        if (user.IsAdmin) return true;

        return false;
    }
}