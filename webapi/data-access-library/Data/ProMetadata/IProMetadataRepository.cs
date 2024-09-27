namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models.ProMetadata;

public interface IProMetadataRepository
{
    // Leagues
    Task<League?> GetLeagueAsync(int LeagueId);
    Task<List<League>> GetLeaguesAsync(bool? IsActive);
    Task AddLeagueAsync(League newLeague);
    Task UpdateLeagueAsync(League updateLeague);
    Task DeleteLeagueAsync(League deleteLeague);

    // Teams
    Task<List<Team>> GetTeamsAsync();
    Task<Team?> GetTeamAsync(long teamId);
    Task AddTeamAsync(Team newTeam);
    Task<List<long>> GetUnknownTeamIds();

    // Players
    Task<List<Account>> GetPlayerAccounts();
    Task<Account?> GetPlayerAccount(long id);

    // Heroes
    Task<List<Hero>> GetHeroesAsync();
    Task<Hero?> GetHeroAsync(long id);
    Task UpsertHeroAsync(Hero upsertHero);
}