namespace DataAccessLibrary.Models.Fantasy;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DataAccessLibrary.Models.ProMetadata;

[Table("dota_fantasy_players")]
public class FantasyPlayer
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [ForeignKey("FantasyLeague")]
    [Column("fantasy_league_id")]
    public required int FantasyLeagueId { get; set; }

    [JsonIgnore]
    public FantasyLeague? FantasyLeague { get; set; }

    [ForeignKey("Team")]
    [Column("team_id")]
    public required long TeamId { get; set; }
    public Team? Team { get; set; }

    [ForeignKey("DotaAccount")]
    [Column("dota_account_id")]
    public required long DotaAccountId { get; set; }
    public Account? DotaAccount { get; set; }

    [Column("team_position")]
    public int TeamPosition { get; set; }
}