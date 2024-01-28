using csharp_ef_webapi.Models;

namespace csharp_ef_webapi.Data;
public interface IFantasyRepository
{
    // Fantasy Drafts
    Task<IEnumerable<FantasyPlayer>> FantasyPlayersByLeagueAsync(int? LeagueId);
    Task<IEnumerable<FantasyDraft>> FantasyDraftsByUserLeagueAsync(long UserDiscordAccountId, int LeagueId);
    Task<IEnumerable<FantasyPlayerPoints>> FantasyPlayerPointsByLeagueAsync(int LeagueId);
    Task<IEnumerable<FantasyPlayerPoints>> FantasyDraftPointsByLeagueAsync(int LeagueId);
    Task<IEnumerable<FantasyPlayerPoints>> FantasyDraftPointsByUserLeagueAsync(long UserDiscordAccountId, int LeagueId);
    IEnumerable<FantasyPlayerPointTotals> AggregateFantasyPlayerPoints(IEnumerable<FantasyPlayerPoints> fantasyPlayerPoints);
    IEnumerable<FantasyDraftPointTotals> AggregateFantasyDraftPoints(IEnumerable<FantasyPlayerPoints> fantasyPlayerPoints);
    Task ClearUserFantasyPlayersAsync(long UserDiscordAccountId, int LeagueId);
    Task<FantasyDraft> AddNewUserFantasyPlayerAsync(long UserDiscordAccountId, int LeagueId, long? FantasyPlayerId, int DraftOrder);

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