namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models.Fantasy;

public interface IFantasyMatchRepository : IRepository<FantasyMatch, long>
{
    Task<List<FantasyMatch>> GetNotGcDetailParsed(int takeAmount);
    Task<List<FantasyMatch>> GetNotDetailParsed(int takeAmount);
    Task AddRangeAsync(IEnumerable<FantasyMatch> newFantasyMatches);
    Task UpdateRangeAsync(IEnumerable<FantasyMatch> updateFantasyMatches);
}