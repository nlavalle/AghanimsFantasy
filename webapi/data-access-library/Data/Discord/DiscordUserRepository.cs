namespace DataAccessLibrary.Data;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLibrary.Models.Discord;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Repository to handle all of the metadata operations (leagues, players, teams) and none of the specific
/// game coordinator, webapi, fantasy, or discord repositories
/// </summary>
public class DiscordUserRepository : IDiscordUserRepository
{
    private readonly ILogger<DiscordUserRepository> _logger;
    private AghanimsFantasyContext _dbContext;
    public DiscordUserRepository(ILogger<DiscordUserRepository> logger, AghanimsFantasyContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public IQueryable<DiscordUser> GetQueryable()
    {
        return _dbContext.DiscordUsers;
    }

    public async Task<List<DiscordUser>> GetAllAsync()
    {
        _logger.LogDebug($"Get Discord Users");

        return await _dbContext.DiscordUsers.ToListAsync();
    }

    public async Task<DiscordUser?> GetByIdAsync(long DiscordAccountId)
    {
        _logger.LogDebug($"Fetching Single Discord User {DiscordAccountId}");

        return await _dbContext.DiscordUsers.FindAsync(DiscordAccountId);
    }

    public async Task AddAsync(DiscordUser entity)
    {
        _logger.LogInformation($"Adding new Discord User {entity.Id}");

        await _dbContext.DiscordUsers.AddAsync(entity);

        return;
    }

    public async Task DeleteAsync(DiscordUser entity)
    {
        _logger.LogInformation($"Removing Discord User {entity.Id}");

        _dbContext.DiscordUsers.Remove(entity);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task UpdateAsync(DiscordUser entity)
    {
        _logger.LogInformation($"Updating Discord User {entity.Id}");

        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        return;
    }
}