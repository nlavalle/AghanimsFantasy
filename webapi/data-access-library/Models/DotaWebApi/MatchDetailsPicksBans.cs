namespace DataAccessLibrary.Models.WebApi;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

// https://api.steampowered.com/IDOTA2Match_570/GetMatchDetails/v1?
[Table("dota_match_details_picks_bans")]
public class MatchDetailsPicksBans
{
    [Key]
    [Column("id")]
    [JsonIgnore]
    public int Id { get; set; }

    [ForeignKey("Match")]
    [Column("match_id")]
    public required long MatchId { get; set; }

    [JsonIgnore]
    public MatchDetail? Match { get; set; }

    [Column("is_pick")]
    [JsonPropertyName("is_pick")]
    public bool IsPick { get; set; }

    [Column("hero_id")]
    [JsonPropertyName("hero_id")]
    public int HeroId { get; set; }

    [Column("team")]
    [JsonPropertyName("team")]
    public int Team { get; set; }

    [Column("order")]
    [JsonPropertyName("order")]
    public int Order { get; set; }

}