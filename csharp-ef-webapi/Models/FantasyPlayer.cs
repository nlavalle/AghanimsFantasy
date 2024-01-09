namespace csharp_ef_webapi.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

[Table("dota_fantasy_players")]
public class FantasyPlayer
{
    [Key]
    [Column("id")]
    [JsonIgnore]
    public long Id { get; set; }

    [Column("league_id")]
    public long LeagueId { get; set; }

    [Column("team_id")]
    public long TeamId { get; set; }
    public Team Team { get; set; } = new Team();

    [Column("dota_account_id")]
    public long DotaAccountId { get; set; }
    public Account DotaAccount { get; set; } = new Account();

}