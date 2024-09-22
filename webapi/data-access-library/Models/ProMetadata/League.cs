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
    public int Id { get; set; }

    [Column("league_name")]
    public string? Name { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [JsonIgnore]
    public List<GcMatchMetadata> MatchMetadatas { get; set; } = new List<GcMatchMetadata>();

    [JsonIgnore]
    public List<MatchHistory> MatchHistories { get; set; } = new List<MatchHistory>();

    [JsonIgnore]
    public List<MatchDetail> MatchDetails { get; set; } = new List<MatchDetail>();

    [JsonIgnore]
    public List<FantasyLeague> FantasyLeagues { get; set; } = new List<FantasyLeague>();
}