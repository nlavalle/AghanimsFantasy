using csharp_ef_webapi.Models.ProMetadata;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace csharp_ef_webapi.Data;

/// <summary>
/// Repository to handle all of the metadata operations (leagues, players, teams) and none of the specific
/// game coordinator, webapi, fantasy, or discord repositories
/// </summary>
public class ProMetadataRepository : IProMetadataRepository
{
    private readonly ILogger<ProMetadataRepository> _logger;
    private AghanimsFantasyContext _dbContext;
    public ProMetadataRepository(ILogger<ProMetadataRepository> logger, AghanimsFantasyContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    #region League
    public async Task<IEnumerable<League>> GetLeaguesAsync(bool? IsActive)
    {
        _logger.LogInformation($"Fetching All Leagues");

        return await _dbContext.Leagues
                .Where(l => IsActive == null || l.IsActive == IsActive)
                .ToListAsync();
    }

    public async Task<League?> GetLeagueAsync(int LeagueId)
    {
        _logger.LogInformation($"Fetching Single League {LeagueId}");

        return await _dbContext.Leagues.FindAsync(LeagueId);
    }
    #endregion League

    #region Team
    public async Task<IEnumerable<Team>> GetTeamsAsync()
    {
        _logger.LogInformation($"Getting Teams loaded into DB");

        return await _dbContext.Teams.ToListAsync();
    }
    #endregion Team

    #region Hero
    public async Task<IEnumerable<Hero>> GetHeroesAsync()
    {
        _logger.LogInformation($"Getting Heroes loaded into DB");

        return await _dbContext.Heroes.ToListAsync();
    }

    #endregion Hero

    #region Player
    public async Task<IEnumerable<Account>> GetPlayerAccounts()
    {
        _logger.LogInformation($"Getting Player Accounts");

        return await _dbContext.Accounts
                .ToListAsync();
    }
    #endregion Player
}