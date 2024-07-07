namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models.WebApi;

public interface IWebApiRepository
{
    // Matches
    Task<List<MatchHistory>> GetMatchHistoryByFantasyLeagueAsync(int LeagueId);
    Task<MatchDetail?> GetMatchDetailAsync(long MatchId);
    Task<List<MatchDetailsPlayer>> GetMatchDetailPlayersByLeagueAsync(int? LeagueId);
    Task<List<MatchDetail>> GetMatchDetailsByFantasyLeagueAsync(int FantasyLeagueId);
}