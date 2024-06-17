namespace csharp_ef_webapi.Data;

using csharp_ef_webapi.Models;
using csharp_ef_webapi.Models.Fantasy;

public interface IFantasyRepository
{
    // Fantasy Leagues
    Task<IEnumerable<FantasyLeague>> GetFantasyLeaguesAsync(bool? IsActive);
    Task<FantasyLeague?> GetFantasyLeagueAsync(int FantasyLeagueId);
    Task AddFantasyLeagueAsync(FantasyLeague newFantasyLeague);
    Task DeleteFantasyLeagueAsync(FantasyLeague deleteFantasyLeague);
    Task<DateTime> GetLeagueLockedDate(int LeagueId);

    //Fantasy Players
    Task<IEnumerable<FantasyPlayer>> GetFantasyPlayersAsync(int? FantasyLeagueId);
    Task<FantasyPlayer?> GetFantasyPlayerAsync(int FantasyPlayerId);
    Task AddFantasyPlayerAsync(FantasyPlayer newFantasyPlayer);
    Task DeleteFantasyPlayerAsync(FantasyPlayer deleteFantasyPlayer);
    Task<IEnumerable<FantasyPlayerPoints>> FantasyPlayerPointsByFantasyLeagueAsync(int LeagueId);
    Task<IEnumerable<FantasyPlayerPointTotals>> FantasyPlayerPointTotalsByFantasyLeagueAsync(int FantasyLeagueId);
    Task<FantasyPlayerTopHeroes> GetFantasyPlayerTopHeroesAsync(long FantasyPlayerId);
    Task<IEnumerable<FantasyNormalizedAveragesTable>> GetFantasyNormalizedAveragesAsync(long FantasyPlayerId);

    // Fantasy Drafts
    Task<IEnumerable<FantasyDraft>> FantasyDraftsByUserLeagueAsync(long UserDiscordAccountId, int LeagueId);
    Task<IEnumerable<FantasyDraftPointTotals>> FantasyDraftPointsByFantasyLeagueAsync(int LeagueId);
    Task<FantasyDraftPointTotals?> FantasyDraftPointsByUserLeagueAsync(long UserDiscordAccountId, int LeagueId);
    Task ClearUserFantasyPlayersAsync(long UserDiscordAccountId, int LeagueId);
    Task<FantasyDraft> AddNewUserFantasyPlayerAsync(long UserDiscordAccountId, int LeagueId, long? FantasyPlayerId, int DraftOrder);
    Task<IEnumerable<MetadataSummary>> MetadataSummariesByFantasyLeagueAsync(int FantasyLeagueId);

    // Matches
    Task<IEnumerable<MatchHighlights>> GetLastNMatchHighlights(int FantasyLeagueId, int MatchCount);
}