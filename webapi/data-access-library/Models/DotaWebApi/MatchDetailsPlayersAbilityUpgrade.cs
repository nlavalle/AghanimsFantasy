namespace DataAccessLibrary.Models.WebApi;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

// https://api.steampowered.com/IDOTA2Match_570/GetMatchDetails/v1?
[Table("dota_match_details_players_ability_upgrades")]
public class MatchDetailsPlayersAbilityUpgrade
{
    [Key]
    [Column("id")]
    [JsonIgnore]
    public int Id { get; set; }

    [ForeignKey("Player")]
    [Column("player_id")]
    public required int PlayerId { get; set; }

    [JsonIgnore]
    public MatchDetailsPlayer? Player { get; set; }

    [Column("ability")]
    [JsonPropertyName("ability")]
    public int Ability { get; set; }

    [Column("time")]
    [JsonPropertyName("time")]
    public int Time { get; set; }

    [Column("level")]
    [JsonPropertyName("level")]
    public int Level { get; set; }
}