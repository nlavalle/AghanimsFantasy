namespace DataAccessLibrary.Models.Fantasy;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DataAccessLibrary.Models.Discord;

[Table("dota_fantasy_private_league_players")]
public class FantasyPrivateLeaguePlayer
{
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

    [JsonIgnore]
    public required DiscordUser DiscordUser { get; set; }

    [Column("fantasy_league_join_date")]
    public long FantasyLeagueJoinDate { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
}