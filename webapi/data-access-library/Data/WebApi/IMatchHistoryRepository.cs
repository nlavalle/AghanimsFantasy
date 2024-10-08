namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.ProMetadata;
using DataAccessLibrary.Models.WebApi;

public interface IMatchHistoryRepository : IRepository<MatchHistory, long>
{
    Task<List<MatchHistory>> GetByLeagueAsync(League League);
    Task<List<MatchHistory>> GetByFantasyLeagueAsync(FantasyLeague FantasyLeague);
    Task<List<MatchHistory>> GetNotInFantasyMatches(int takeAmount);
    Task<List<MatchHistory>> GetNotInGcMatches(int takeAmount);
    Task<List<MatchHistory>> GetNotInMatchDetails(int takeAmount);
}