namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models.Fantasy;

public interface IPrivateFantasyPlayerRepository
{
    Task<List<FantasyLeague>> GetPrivateFantasyLeaguesAsync(long DiscordAccountId);
    Task<FantasyPrivateLeaguePlayer?> GetFantasyPrivateLeaguePlayerAsync(int FantasyPrivateLeaguePlayerId);
    Task<List<FantasyPrivateLeaguePlayer>> GetFantasyPrivateLeaguePlayersAsync(int FantasyLeagueId);
    Task AddPrivateFantasyPlayerAsync(FantasyPrivateLeaguePlayer newPrivateFantasyLeaguePlayer);
    Task UpdatePrivateFantasyPlayerAsync(FantasyPrivateLeaguePlayer updatePrivateFantasyLeaguePlayer);
    Task DeletePrivateFantasyPlayerAsync(FantasyPrivateLeaguePlayer deletePrivateFantasyLeaguePlayer);
}