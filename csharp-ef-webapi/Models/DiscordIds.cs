namespace csharp_ef_webapi.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("discord_ids")]
public class DiscordIds
{
    [Key]
    [Column("discord_id")]
    public long DiscordId { get; set; }

    [Column("steam_id")]
    public long SteamId { get; set; }

    [Column("account_id")]
    public long AccountId { get; set; }

    [Column("discord_name")]
    public string DiscordName { get; set; } = "";

}