namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models.Fantasy;

public interface IFantasyPlayerRepository : IRepository<FantasyPlayer, long>
{
    Task<List<FantasyPlayer>> GetByFantasyLeagueAsync(FantasyLeague FantasyLeague);
}