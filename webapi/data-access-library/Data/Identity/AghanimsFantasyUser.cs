using Microsoft.AspNetCore.Identity;

namespace DataAccessLibrary.Data.Identity;

// Add profile data for application users by adding properties to the AghanimsFantasyUser class
public class AghanimsFantasyUser : IdentityUser
{
    // Discord fields
    public long DiscordId { get; set; }
    public string DiscordHandle { get; set; } = string.Empty;
}

