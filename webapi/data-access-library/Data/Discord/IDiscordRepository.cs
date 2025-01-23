namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models.Discord;
using DataAccessLibrary.Models.Fantasy;

public interface IDiscordRepository
{
    // Discord
    Task<DiscordUser?> GetDiscordUserAsync(long GetDiscordId);
    Task AddDiscordUserAsync(DiscordUser newUser);
    Task<bool> IsUserAdminAsync(long UserDiscordId);
    Task<bool> IsUserPrivateFantasyAdminAsync(long UserDiscordId);
    Task<bool> IsUserPrivateFantasyAdminAsync(long UserDiscordId, long FantasyLeagueId);

    // Outbox
    Task<List<FantasyMatch>> GetFantasyMatchesNotInDiscordOutboxAsync();
    Task<List<DiscordOutbox>> GetOutboxMessagesAsync(string getObjectKey, string getEventObject);
    Task AddDiscordOutboxAsync(DiscordOutbox newDiscordOutbox);
}