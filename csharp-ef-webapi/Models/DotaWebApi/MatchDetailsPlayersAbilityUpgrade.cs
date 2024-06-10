namespace csharp_ef_webapi.Models.WebApi;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

// https://api.steampowered.com/IDOTA2Match_570/GetMatchDetails/v1?
[Table("dota_match_details_players_ability_upgrades")]
public class MatchDetailsPlayersAbilityUpgrade
{
    [Key]
    [Column("id")]
    [JsonIgnore]
    public int Id { get; set; }

    [Column("player_id")]
    [JsonIgnore]
    public int PlayerId { get; set; }

    [Column("ability")]
    [JsonProperty("ability")]
    public int Ability { get; set; }

    [Column("time")]
    [JsonProperty("time")]
    public int Time { get; set; }

    [Column("level")]
    [JsonProperty("level")]
    public int Level { get; set; }
}