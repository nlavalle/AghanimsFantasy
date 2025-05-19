using Microsoft.AspNetCore.Identity;

namespace DataAccessLibrary.Data.Identity;

// Add profile data for application users by adding properties to the AghanimsFantasyUser class
public class AghanimsFantasyUser : IdentityUser
{
    [PersonalData]
    public string DisplayName { get; set; } = string.Empty;

    // Discord fields
    [PersonalData]
    public long DiscordId { get; set; }

    [PersonalData]
    public string DiscordHandle { get; set; } = string.Empty;
}

