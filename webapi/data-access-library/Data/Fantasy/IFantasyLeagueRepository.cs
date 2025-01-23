using DataAccessLibrary.Models.Discord;
using DataAccessLibrary.Models.Fantasy;

namespace DataAccessLibrary.Data;

public interface IFantasyLeagueRepository : IRepository<FantasyLeague, int>
{
    Task<IEnumerable<FantasyLeague>> GetAccessibleFantasyLeaguesAsync(DiscordUser? user);
    Task<IEnumerable<FantasyLeague>> GetAccessibleFantasyLeaguesByLeagueIdAsync(DiscordUser? user, int LeagueId);
    Task<FantasyLeague?> GetAccessibleFantasyLeagueAsync(int FantasyLeagueId, DiscordUser? user);
    Task<DateTime> GetLeagueLockedDateAsync(int FantasyLeagueId);
    bool IsFantasyLeagueOpenAsync(FantasyLeague FantasyLeague);
}