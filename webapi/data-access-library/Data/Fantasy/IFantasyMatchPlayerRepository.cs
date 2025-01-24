namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Fantasy;

public interface IFantasyMatchPlayerRepository : IRepository<FantasyMatchPlayer, int>
{
    Task<List<FantasyMatchPlayer>> GetNotGcDetailParsed(int takeAmount);
    Task UpdateRangeAsync(IEnumerable<FantasyMatchPlayer> updateFantasyMatchPlayers);
    Task<List<FantasyPlayerPoints>> GetFantasyPlayerPointsByMatchAsync(long MatchId);
    Task<List<FantasyPlayerPoints>> GetFantasyPlayerPointsByMatchesAsync(IEnumerable<FantasyMatch> FantasyMatches);
}