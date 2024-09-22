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
    [JsonIgnore]
    public long Id { get; set; }

    [JsonIgnore]
    public FantasyLeague FantasyLeague { get; set; } = null!;
    public required Team Team { get; set; }
    public required Account DotaAccount { get; set; }

    [Column("team_position")]
    public int TeamPosition { get; set; }
}