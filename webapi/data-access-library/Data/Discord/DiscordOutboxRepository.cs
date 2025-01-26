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
public class DiscordOutboxRepository : IDiscordOutboxRepository
{
    private readonly ILogger<DiscordOutboxRepository> _logger;
    private AghanimsFantasyContext _dbContext;
    public DiscordOutboxRepository(ILogger<DiscordOutboxRepository> logger, AghanimsFantasyContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public IQueryable<DiscordOutbox> GetQueryable()
    {
        return _dbContext.DiscordOutbox;
    }

    public async Task<List<DiscordOutbox>> GetAllAsync()
    {
        _logger.LogDebug($"Get Discord Outbox Messages");

        return await _dbContext.DiscordOutbox.ToListAsync();
    }

    public async Task<DiscordOutbox?> GetByIdAsync(long id)
    {
        _logger.LogDebug($"Fetching Single Discord Outbox {id}");

        return await _dbContext.DiscordOutbox.FindAsync(id);
    }

    public async Task AddAsync(DiscordOutbox entity)
    {
        _logger.LogInformation($"Adding new Discord Outbox {entity.Id}");

        await _dbContext.DiscordOutbox.AddAsync(entity);

        return;
    }

    public async Task DeleteAsync(DiscordOutbox entity)
    {
        _logger.LogInformation($"Removing Discord Outbox {entity.Id}");

        _dbContext.DiscordOutbox.Remove(entity);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task UpdateAsync(DiscordOutbox entity)
    {
        _logger.LogInformation($"Updating Discord Outbox {entity.Id}");

        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        return;
    }
}