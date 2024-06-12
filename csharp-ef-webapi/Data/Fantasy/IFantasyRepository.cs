namespace csharp_ef_webapi.Data;

using csharp_ef_webapi.Models;
using csharp_ef_webapi.Models.Fantasy;

public interface IFantasyRepository
{
    // Fantasy Leagues
    Task<DateTime> GetLeagueLockedDate(int LeagueId);

    // Fantasy Drafts
    Task<IEnumerable<FantasyPlayer>> FantasyPlayersByFantasyLeagueAsync(int? FantasyLeagueId);
    Task<IEnumerable<FantasyDraft>> FantasyDraftsByUserLeagueAsync(long UserDiscordAccountId, int LeagueId);
    Task<IEnumerable<FantasyPlayerPoints>> FantasyPlayerPointsByFantasyLeagueAsync(int LeagueId);
    Task<IEnumerable<FantasyPlayerPointTotals>> FantasyPlayerPointTotalsByFantasyLeagueAsync(int FantasyLeagueId);
    Task<IEnumerable<FantasyDraftPointTotals>> FantasyDraftPointsByFantasyLeagueAsync(int LeagueId);
    Task<FantasyDraftPointTotals?> FantasyDraftPointsByUserLeagueAsync(long UserDiscordAccountId, int LeagueId);
    Task ClearUserFantasyPlayersAsync(long UserDiscordAccountId, int LeagueId);
    Task<FantasyDraft> AddNewUserFantasyPlayerAsync(long UserDiscordAccountId, int LeagueId, long? FantasyPlayerId, int DraftOrder);
    Task<IEnumerable<MetadataSummary>> MetadataSummariesByFantasyLeagueAsync(int FantasyLeagueId);

    // Matches
    Task<IEnumerable<MatchHighlights>> GetLastNMatchHighlights(int FantasyLeagueId, int MatchCount);
    Task<IEnumerable<FantasyNormalizedAveragesTable>> GetFantasyNormalizedAveragesAsync(long FantasyPlayerId);
    Task<FantasyPlayerTopHeroes> GetFantasyPlayerTopHeroesAsync(long FantasyPlayerId);
}