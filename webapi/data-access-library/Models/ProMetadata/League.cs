namespace DataAccessLibrary.Models.ProMetadata;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.GameCoordinator;
using DataAccessLibrary.Models.WebApi;

[Table("dota_leagues")]
public class League
{
    [Key]
    [Column("id")]
    [JsonPropertyName("league_id")]
    public int Id { get; set; }

    [Column("league_name")]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [Column("is_active")]
    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; } = false;

    [Column("is_scheduled")]
    [JsonPropertyName("is_scheduled")]
    public bool IsScheduled { get; set; } = false;

    [Column("tier")]
    [JsonPropertyName("tier")]
    public int Tier { get; set; }

    [Column("region")]
    [JsonPropertyName("region")]
    public int Region { get; set; }

    [Column("most_recent_activity")]
    [JsonPropertyName("most_recent_activity")]
    public long MostRecentActivity { get; set; }

    [Column("total_prize_pool")]
    [JsonPropertyName("total_prize_pool")]
    public long PrizePool { get; set; }

    [Column("start_timestamp")]
    [JsonPropertyName("start_timestamp")]
    public long StartTimestamp { get; set; }

    [Column("end_timestamp")]
    [JsonPropertyName("end_timestamp")]
    public long EndTimeStamp { get; set; }

    [Column("status")]
    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonIgnore]
    public List<GcMatchMetadata> MatchMetadatas { get; set; } = new List<GcMatchMetadata>();

    [JsonIgnore]
    public List<MatchHistory> MatchHistories { get; set; } = new List<MatchHistory>();

    [JsonIgnore]
    public List<MatchDetail> MatchDetails { get; set; } = new List<MatchDetail>();

    [JsonIgnore]
    public List<FantasyLeague> FantasyLeagues { get; set; } = new List<FantasyLeague>();
}