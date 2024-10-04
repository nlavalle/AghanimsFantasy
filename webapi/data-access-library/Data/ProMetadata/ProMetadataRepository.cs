namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models.ProMetadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
    public async Task<List<League>> GetLeaguesAsync(bool? IncludeInactive)
    {
        _logger.LogInformation($"Fetching All Leagues");

        return await _dbContext.Leagues
                .Where(l => IncludeInactive == true || l.IsActive)
                .ToListAsync();
    }

    public async Task<League?> GetLeagueAsync(int LeagueId)
    {
        _logger.LogInformation($"Fetching Single League {LeagueId}");

        return await _dbContext.Leagues.FindAsync(LeagueId);
    }

    public async Task AddLeagueAsync(League newLeague)
    {
        _logger.LogInformation($"Adding new League {newLeague.Name}");

        await _dbContext.Leagues.AddAsync(newLeague);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task UpdateLeagueAsync(League updateLeague)
    {
        _logger.LogInformation($"Updating League {updateLeague.Name}");

        _dbContext.Entry(updateLeague).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task DeleteLeagueAsync(League deleteLeague)
    {
        _logger.LogInformation($"Removing League {deleteLeague.Name}");

        _dbContext.Leagues.Remove(deleteLeague);
        await _dbContext.SaveChangesAsync();

        return;
    }
    #endregion League

    #region Team
    public async Task<List<Team>> GetTeamsAsync()
    {
        _logger.LogInformation($"Getting Teams loaded into DB");

        return await _dbContext.Teams.ToListAsync();
    }

    public async Task<Team?> GetTeamAsync(long teamId)
    {
        _logger.LogInformation($"Getting Team {teamId} loaded into DB");

        return await _dbContext.Teams.FindAsync(teamId);
    }

    public async Task AddTeamAsync(Team newTeam)
    {
        _logger.LogInformation($"Adding Team {newTeam.Name} into DB");

        if (_dbContext.Teams.FirstOrDefault(t => t.Id == newTeam.Id) == null)
        {
            _dbContext.Teams.Add(newTeam);
            await _dbContext.SaveChangesAsync();
        }

        return;
    }

    public async Task UpdateTeamAsync(Team updateTeam)
    {
        _logger.LogInformation($"Updating Team {updateTeam.Name}");

        _dbContext.Entry(updateTeam).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task DeleteTeamAsync(Team deleteTeam)
    {
        _logger.LogInformation($"Removing Team {deleteTeam.Name}");

        _dbContext.Teams.Remove(deleteTeam);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task<List<long>> GetUnknownTeamIds()
    {
        _logger.LogInformation($"Getting Unknown Team Ids in match history");

        var matchesNotInGcQuery = _dbContext.MatchHistory
            .Select(mh => mh.RadiantTeamId)
            .Union(_dbContext.MatchHistory.Select(mh => mh.DireTeamId))
            .Distinct()
            .Where(t => t != 0)
            .Where(
                mh => !_dbContext.Teams
                    .Select(t => t.Id)
                    .Contains(mh));

        return await matchesNotInGcQuery.ToListAsync();
    }
    #endregion Team

    #region Hero
    public async Task<Hero?> GetHeroAsync(long id)
    {
        _logger.LogInformation($"Getting Hero {id} loaded into DB");

        return await _dbContext.Heroes.FindAsync(id);
    }

    public async Task<List<Hero>> GetHeroesAsync()
    {
        _logger.LogInformation($"Getting Heroes loaded into DB");

        return await _dbContext.Heroes.ToListAsync();
    }

    public async Task AddHeroAsync(Hero newHero)
    {
        _logger.LogInformation($"Adding Hero {newHero.Name} into DB");

        await _dbContext.Heroes.AddAsync(newHero);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task UpsertHeroAsync(Hero upsertHero)
    {
        _logger.LogInformation($"Upserting Dota Hero {upsertHero.Name}");

        if (_dbContext.Heroes.Find(upsertHero.Id) != null)
        {
            Hero updateHero = _dbContext.Heroes.First(h => h.Id == upsertHero.Id);
            updateHero.Name = upsertHero.Name;
            _dbContext.Heroes.Update(updateHero);
        }
        else
        {
            _dbContext.Heroes.Add(upsertHero);
        }

        await _dbContext.SaveChangesAsync();

        return;
    }
    #endregion Hero

    #region Player
    public async Task<Account?> GetPlayerAccount(long id)
    {
        _logger.LogInformation($"Getting Account {id} loaded into DB");

        return await _dbContext.Accounts.FindAsync(id);
    }

    public async Task<List<Account>> GetPlayerAccounts()
    {
        _logger.LogInformation($"Getting Player Accounts");

        return await _dbContext.Accounts
                .ToListAsync();
    }

    public async Task AddAccountAsync(Account newAccount)
    {
        _logger.LogInformation($"Adding new Account {newAccount.Name}");

        await _dbContext.Accounts.AddAsync(newAccount);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task UpdateAccountAsync(Account updateAccount)
    {
        _logger.LogInformation($"Updating Account {updateAccount.Name}");

        _dbContext.Entry(updateAccount).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task DeleteAccountAsync(Account deleteAccount)
    {
        _logger.LogInformation($"Removing Account {deleteAccount.Name}");

        _dbContext.Accounts.Remove(deleteAccount);
        await _dbContext.SaveChangesAsync();

        return;
    }
    #endregion Player
}