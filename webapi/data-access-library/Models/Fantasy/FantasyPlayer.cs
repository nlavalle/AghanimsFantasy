namespace DataAccessLibrary.Models.Fantasy;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccessLibrary.Models.ProMetadata;
using Newtonsoft.Json;

[Table("dota_fantasy_players")]
public class FantasyPlayer
{
    [Key]
    [Column("id")]
    [JsonIgnore]
    public long Id { get; set; }
    public required FantasyLeague FantasyLeague { get; set; }
    public required Team Team { get; set; }
    public required Account DotaAccount { get; set; }

    [Column("team_position")]
    public int TeamPosition { get; set; }
}