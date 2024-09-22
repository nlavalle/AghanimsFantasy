namespace DataAccessLibrary.Models.WebApi;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

// https://api.steampowered.com/IDOTA2Match_570/GetMatchDetails/v1?
[Table("dota_match_details_picks_bans")]
public class MatchDetailsPicksBans
{
    [Key]
    [Column("id")]
    [System.Text.Json.Serialization.JsonIgnore]
    public int Id { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public MatchDetail Match { get; set; } = null!;

    [Column("is_pick")]
    [JsonProperty("is_pick")]
    public bool IsPick { get; set; }

    [Column("hero_id")]
    [JsonProperty("hero_id")]
    public int HeroId { get; set; }

    [Column("team")]
    [JsonProperty("team")]
    public int Team { get; set; }

    [Column("order")]
    [JsonProperty("order")]
    public int Order { get; set; }

}