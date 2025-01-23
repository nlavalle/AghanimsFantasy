namespace DataAccessLibrary.Models.Fantasy;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DataAccessLibrary.Models.Discord;

[Table("dota_fantasy_private_league_players")]
public class FantasyPrivateLeaguePlayer
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [ForeignKey("FantasyLeague")]
    [Column("fantasy_league_id")]
    public required int FantasyLeagueId { get; set; }

    [JsonIgnore]
    public FantasyLeague? FantasyLeague { get; set; }

    [ForeignKey("DiscordUser")]
    [Column("discord_user_id")]
    public required long DiscordUserId { get; set; }

    public DiscordUser? DiscordUser { get; set; }

    [Column("is_admin")]
    public required bool IsAdmin { get; set; } = false;

    [Column("fantasy_league_join_date")]
    public long FantasyLeagueJoinDate { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
}