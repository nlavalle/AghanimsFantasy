namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models.Fantasy;

public interface IPrivateFantasyPlayerRepository : IRepository<FantasyPrivateLeaguePlayer, int>
{
    Task<List<FantasyPrivateLeaguePlayer>> GetByFantasyLeagueAsync(int FantasyLeagueId);
    Task<FantasyPrivateLeaguePlayer?> GetByDiscordIdAsync(long id);
}