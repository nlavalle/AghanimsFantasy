using csharp_ef_webapi.Models;

namespace csharp_ef_webapi.Data;
public interface IFantasyRepository
{
    // Leagues
    Task<League?> GetLeagueAsync(int LeagueId);
    Task<IEnumerable<League>> GetLeaguesAsync(bool? IsActive);

    // Fantasy Leagues
    Task<DateTime> GetLeagueLockedDate(int LeagueId);

    // Fantasy Drafts
    Task<IEnumerable<FantasyPlayer>> FantasyPlayersByFantasyLeagueAsync(int? FantasyLeagueId);
    Task<IEnumerable<FantasyDraft>> FantasyDraftsByUserLeagueAsync(long UserDiscordAccountId, int LeagueId);
    Task<IEnumerable<FantasyPlayerPoints>> FantasyPlayerPointsByFantasyLeagueAsync(int LeagueId);
    Task<IEnumerable<FantasyDraftPointTotals>> FantasyDraftPointsByFantasyLeagueAsync(int LeagueId);
    Task<FantasyDraftPointTotals?> FantasyDraftPointsByUserLeagueAsync(long UserDiscordAccountId, int LeagueId);
    IEnumerable<FantasyPlayerPointTotals> AggregateFantasyPlayerPoints(IEnumerable<FantasyPlayerPoints> fantasyPlayerPoints);
    Task ClearUserFantasyPlayersAsync(long UserDiscordAccountId, int LeagueId);
    Task<FantasyDraft> AddNewUserFantasyPlayerAsync(long UserDiscordAccountId, int LeagueId, long? FantasyPlayerId, int DraftOrder);

    // Matches
    Task<IEnumerable<MatchHistory>> GetMatchHistoryByFantasyLeagueAsync(int LeagueId);
    Task<MatchDetail?> GetMatchDetailAsync(long MatchId);
    Task<IEnumerable<MatchDetailsPlayer>> GetMatchDetailPlayersByLeagueAsync(int? LeagueId);
    Task<IEnumerable<GcMatchMetadata>> GetLeagueMetadataAsync(int LeagueId);
    Task<IEnumerable<GcMatchMetadata>> GetLeagueMetadataAsync(int LeagueId, int Skip, int Take);
    Task<IEnumerable<GcMatchMetadata>> GetFantasyLeagueMetadataAsync(int FantasyLeagueId);
    Task<IEnumerable<GcMatchMetadata>> GetFantasyLeagueMetadataAsync(int FantasyLeagueId, int Skip, int Take);
    Task<GcMatchMetadata?> GetMatchMetadataAsync(long MatchId);
    Task<IEnumerable<MatchHighlights>> GetLastNMatchHighlights(int FantasyLeagueId, int MatchCount);

    // Players
    Task<IEnumerable<Account>> GetPlayerAccounts();

    // Teams
    Task<IEnumerable<Team>> GetTeamsAsync();

    // Heroes
    Task<IEnumerable<Hero>> GetHeroesAsync();
    
    // Discord
    Task<DiscordIds?> GetDiscordIdAsync(long GetDiscordId);
}