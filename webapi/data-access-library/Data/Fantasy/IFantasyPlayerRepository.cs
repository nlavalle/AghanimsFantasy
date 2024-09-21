namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models.Fantasy;

public interface IFantasyPlayerRepository : IRepository<FantasyPlayer, long>
{
    Task<IEnumerable<FantasyPlayer>> GetFantasyLeaguePlayersAsync(FantasyLeague FantasyLeague);
}