using csharp_ef_webapi.Models;

namespace csharp_ef_webapi.Data;
public interface IFantasyRepository
{
    // Fantasy Drafts
    Task<IEnumerable<FantasyPlayer>> GetFantasyPlayersAsync(int? LeagueId);
    Task<IEnumerable<object>> GetUserFantasyDraftsByLeagueAsync(long UserDiscordAccountId, int LeagueId);
    Task<IEnumerable<FantasyPlayerPoints>> GetFantasyPlayerPointsAsync(int LeagueId);
    Task<IEnumerable<object?>> GetPlayersTotalFantasyPointsByLeagueAsync(int LeagueId);
    Task<object?> GetUserTotalFantasyPointsByLeagueAsync(long UserDiscordAccountId, int LeagueId);
    Task<IEnumerable<object?>> GetTopNTotalFantasyPointsByLeagueAsync(int LeagueId, int Count);
    Task<object?> AddNewUserFantasyDraftAsync(long UserDiscordAccountId, FantasyDraft FantasyDraft);

    // Leagues
    Task<League?> GetLeagueAsync(int LeagueId);
    Task<IEnumerable<League>> GetLeaguesAsync(bool? IsActive);
    Task<DateTime> GetLeagueLockedDate(int LeagueId);

    // Matches
    Task<IEnumerable<MatchHistory>> GetMatchHistoryAsync(int LeagueId);
    Task<MatchDetail?> GetMatchDetailAsync(int LeagueId, long MatchId);
    Task<IEnumerable<MatchDetail>> GetMatchDetailsAsync(int LeagueId);
    Task<IEnumerable<MatchDetailsPlayer>> GetMatchDetailPlayersByLeagueAsync(int? LeagueId);

    // Players
    Task<IEnumerable<Account>> GetPlayerAccounts();

    // Teams
    Task<IEnumerable<Team>> GetTeamsAsync();

    // Heroes
    Task<IEnumerable<Hero>> GetHeroesAsync();
}