namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Fantasy;

public interface IFantasyRepository
{
    Task<List<FantasyPlayerPointTotals>> FantasyPlayerPointTotalsByFantasyLeagueAsync(FantasyLeague FantasyLeague);
    Task<List<FantasyPlayerPoints>> FantasyPlayerPointsByFantasyLeagueAsync(int FantasyLeagueId, int limit);
    Task<FantasyPlayerTopHeroes> GetFantasyPlayerTopHeroesAsync(long FantasyPlayerId);
    Task<List<FantasyNormalizedAveragesTable>> GetFantasyNormalizedAveragesAsync(long FantasyPlayerId);
    Task<List<MetadataSummary>> MetadataSummariesByFantasyLeagueAsync(FantasyLeague FantasyLeague);
    Task<List<MatchHighlights>> GetLastNMatchHighlights(int FantasyLeagueId, int MatchCount);
}