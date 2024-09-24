namespace DataAccessLibrary.Models.WebApi;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

[Table("dota_match_history_players")]
public class MatchHistoryPlayer
{
    [Key]
    [Column("id")]
    [JsonIgnore]
    public long Id { get; set; }

    [ForeignKey("Match")]
    [Column("match_id")]
    public required long MatchId { get; set; }

    [JsonIgnore]
    public MatchHistory? Match { get; set; }

    [Column("account_id")]
    [JsonPropertyName("account_id")]
    public long AccountId { get; set; }

    [Column("player_slot")]
    [JsonPropertyName("player_slot")]
    public int PlayerSlot { get; set; }

    [Column("team_number")]
    [JsonPropertyName("team_number")]
    public int TeamNumber { get; set; }

    [Column("team_slot")]
    [JsonPropertyName("team_slot")]
    public int TeamSlot { get; set; }

    [Column("hero_id")]
    [JsonPropertyName("hero_id")]
    public int HeroId { get; set; }
}