namespace DataAccessLibrary.Models.WebApi;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

[Table("dota_match_history_players")]
public class MatchHistoryPlayer
{
    [Key]
    [Column("id")]
    [System.Text.Json.Serialization.JsonIgnore]
    public long Id { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public MatchHistory Match { get; set; } = null!;

    [Column("account_id")]
    [JsonProperty("account_id")]
    public long AccountId { get; set; }

    [Column("player_slot")]
    [JsonProperty("player_slot")]
    public int PlayerSlot { get; set; }

    [Column("team_number")]
    [JsonProperty("team_number")]
    public int TeamNumber { get; set; }

    [Column("team_slot")]
    [JsonProperty("team_slot")]
    public int TeamSlot { get; set; }

    [Column("hero_id")]
    [JsonProperty("hero_id")]
    public int HeroId { get; set; }
}