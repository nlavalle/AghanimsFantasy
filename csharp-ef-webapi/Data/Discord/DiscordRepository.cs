using csharp_ef_webapi.Models.Discord;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace csharp_ef_webapi.Data;

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
    #endregion Discord
}