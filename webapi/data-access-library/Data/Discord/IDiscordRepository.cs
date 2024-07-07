namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models.Discord;

public interface IDiscordRepository
{
    // Discord
    Task<DiscordUser?> GetDiscordUserAsync(long GetDiscordId);
    Task AddDiscordUserAsync(DiscordUser newUser);
    Task<bool> IsUserAdminAsync(long UserDiscordId);
}