namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models.Fantasy;

public interface IFantasyMatchPlayerRepository : IRepository<FantasyMatchPlayer, int>
{
    Task UpdateRangeAsync(IEnumerable<FantasyMatchPlayer> updateFantasyMatchPlayers);
}