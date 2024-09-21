namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.ProMetadata;
using DataAccessLibrary.Models.WebApi;

public interface IMatchDetailRepository : IRepository<MatchDetail, long>
{
    Task<List<MatchDetailsPlayer>> GetByLeagueAsync(League League);
    Task<List<MatchDetail>> GetByFantasyLeagueAsync(FantasyLeague FantasyLeague);
}