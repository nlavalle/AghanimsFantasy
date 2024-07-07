namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Fantasy;

public interface IFantasyRepository
{
    // Fantasy Leagues
    Task<List<FantasyLeague>> GetFantasyLeaguesAsync(bool? IsActive);
    Task<FantasyLeague?> GetFantasyLeagueAsync(int FantasyLeagueId);
    Task AddFantasyLeagueAsync(FantasyLeague newFantasyLeague);
    Task DeleteFantasyLeagueAsync(FantasyLeague deleteFantasyLeague);
    Task<DateTime> GetLeagueLockedDate(int LeagueId);

    //Fantasy Players
    Task<List<FantasyPlayer>> GetFantasyPlayersAsync(int? FantasyLeagueId);
    Task<FantasyPlayer?> GetFantasyPlayerAsync(int FantasyPlayerId);
    Task AddFantasyPlayerAsync(FantasyPlayer newFantasyPlayer);
    Task DeleteFantasyPlayerAsync(FantasyPlayer deleteFantasyPlayer);
    Task<List<FantasyPlayerPoints>> FantasyPlayerPointsByFantasyLeagueAsync(int LeagueId, int limit);
    Task<List<FantasyPlayerPointTotals>> FantasyPlayerPointTotalsByFantasyLeagueAsync(int FantasyLeagueId);
    Task<FantasyPlayerTopHeroes> GetFantasyPlayerTopHeroesAsync(long FantasyPlayerId);
    Task<List<FantasyNormalizedAveragesTable>> GetFantasyNormalizedAveragesAsync(long FantasyPlayerId);

    // Fantasy Drafts
    Task<List<FantasyDraft>> FantasyDraftsByUserLeagueAsync(long UserDiscordAccountId, int LeagueId);
    Task<List<FantasyDraftPointTotals>> FantasyDraftPointsByFantasyLeagueAsync(int LeagueId);
    Task<FantasyDraftPointTotals?> FantasyDraftPointsByUserLeagueAsync(long UserDiscordAccountId, int LeagueId);
    Task ClearUserFantasyPlayersAsync(long UserDiscordAccountId, int LeagueId);
    Task<FantasyDraft> AddNewUserFantasyPlayerAsync(long UserDiscordAccountId, int LeagueId, long? FantasyPlayerId, int DraftOrder);
    Task<List<MetadataSummary>> MetadataSummariesByFantasyLeagueAsync(int FantasyLeagueId);

    // Matches
    Task<List<MatchHighlights>> GetLastNMatchHighlights(int FantasyLeagueId, int MatchCount);
}