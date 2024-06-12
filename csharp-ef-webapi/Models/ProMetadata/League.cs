namespace csharp_ef_webapi.Models.ProMetadata;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using csharp_ef_webapi.Models.Fantasy;
using csharp_ef_webapi.Models.GameCoordinator;
using csharp_ef_webapi.Models.WebApi;

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
    public List<GcMatchMetadata> MatchMetadatas { get; set; } = new List<GcMatchMetadata>();
    public List<MatchHistory> MatchHistories { get; set; } = new List<MatchHistory>();
    public List<MatchDetail> MatchDetails { get; set; } = new List<MatchDetail>();
    public List<FantasyLeague> FantasyLeagues { get; set; } = new List<FantasyLeague>();
}