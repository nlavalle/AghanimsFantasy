namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models.Discord;
using Microsoft.CodeAnalysis;
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

    #region Discord
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
    #endregion Discord
}