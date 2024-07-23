namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models.ProMetadata;

public interface IProMetadataRepository
{
    // Leagues
    Task<League?> GetLeagueAsync(int LeagueId);
    Task<List<League>> GetLeaguesAsync(bool? IsActive);
    Task AddLeagueAsync(League newLeague);
    Task DeleteLeagueAsync(League deleteLeague);

    // Teams
    Task<List<Team>> GetTeamsAsync();

    // Players
    Task<List<Account>> GetPlayerAccounts();

    // Heroes
    Task<List<Hero>> GetHeroesAsync();
}