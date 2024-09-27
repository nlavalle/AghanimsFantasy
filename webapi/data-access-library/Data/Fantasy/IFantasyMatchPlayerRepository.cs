namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models.Fantasy;

public interface IFantasyMatchPlayerRepository : IRepository<FantasyMatchPlayer, int>
{
    Task<List<FantasyMatchPlayer>> GetNotGcDetailParsed(int takeAmount);
    Task UpdateRangeAsync(IEnumerable<FantasyMatchPlayer> updateFantasyMatchPlayers);
}