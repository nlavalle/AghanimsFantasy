using csharp_ef_webapi.Models.Discord;

namespace csharp_ef_webapi.Data;
public interface IDiscordRepository
{
    // Discord
    Task<DiscordUser?> GetDiscordUserAsync(long GetDiscordId);
    Task AddDiscordUserAsync(DiscordUser newUser);
    Task<bool> IsUserAdminAsync(long UserDiscordId);
}