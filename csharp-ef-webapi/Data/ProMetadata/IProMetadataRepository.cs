using csharp_ef_webapi.Models.ProMetadata;

namespace csharp_ef_webapi.Data;
public interface IProMetadataRepository
{
    // Leagues
    Task<League?> GetLeagueAsync(int LeagueId);
    Task<IEnumerable<League>> GetLeaguesAsync(bool? IsActive);
    Task AddLeagueAsync(League newLeague);
    Task DeleteLeagueAsync(League deleteLeague);

    // Teams
    Task<IEnumerable<Team>> GetTeamsAsync();

    // Players
    Task<IEnumerable<Account>> GetPlayerAccounts();

    // Heroes
    Task<IEnumerable<Hero>> GetHeroesAsync();
}