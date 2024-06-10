using csharp_ef_webapi.Models.WebApi;

namespace csharp_ef_webapi.Data;
public interface IWebApiRepository
{
    // Matches
    Task<IEnumerable<MatchHistory>> GetMatchHistoryByFantasyLeagueAsync(int LeagueId);
    Task<MatchDetail?> GetMatchDetailAsync(long MatchId);
    Task<IEnumerable<MatchDetailsPlayer>> GetMatchDetailPlayersByLeagueAsync(int? LeagueId);
    Task<IEnumerable<MatchDetail>> GetMatchDetailsByFantasyLeagueAsync(int FantasyLeagueId);
}