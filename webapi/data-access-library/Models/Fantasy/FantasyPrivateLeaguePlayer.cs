namespace DataAccessLibrary.Models.Fantasy;

using System.ComponentModel.DataAnnotations.Schema;
using DataAccessLibrary.Models.Discord;

[Table("dota_fantasy_private_league_players")]
public class FantasyPrivateLeaguePlayer
{
    [Column("id")]
    public int Id { get; set; }

    [Column("fantasy_league_id")]
    public required FantasyLeague FantasyLeague { get; set; }

    [Column("discord_user_id")]
    public required DiscordUser DiscordUser { get; set; }

    [Column("fantasy_league_join_date")]
    public long FantasyLeagueJoinDate { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
}