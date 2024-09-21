using DataAccessLibrary.Models.Discord;
using DataAccessLibrary.Models.Fantasy;

namespace DataAccessLibrary.Data;

public interface IFantasyLeagueRepository : IRepository<FantasyLeague, int>
{
    Task<IEnumerable<FantasyLeague>> GetAccessibleFantasyLeaguesAsync(DiscordUser? user);
    Task<FantasyLeague?> GetAccessibleFantasyLeagueAsync(int FantasyLeagueId, DiscordUser? user);
    Task<DateTime> GetLeagueLockedDateAsync(int FantasyLeagueId);
}