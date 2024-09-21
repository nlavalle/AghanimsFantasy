using DataAccessLibrary.Models.Discord;
using DataAccessLibrary.Models.Fantasy;

namespace DataAccessLibrary.Data;

public interface IFantasyLeagueRepository : IRepository<FantasyLeague, int>
{
    Task<IEnumerable<FantasyLeague>> GetAccessibleFantasyLeagues(DiscordUser? user);
    Task<FantasyLeague?> GetAccessibleFantasyLeague(int FantasyLeagueId, DiscordUser? user);
    Task<DateTime> GetLeagueLockedDate(int FantasyLeagueId);
}