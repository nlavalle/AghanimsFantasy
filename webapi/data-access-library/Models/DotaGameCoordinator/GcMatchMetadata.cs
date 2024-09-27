namespace DataAccessLibrary.Models.GameCoordinator;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccessLibrary.Models.ProMetadata;
using System.Text.Json.Serialization;

// EF mapping for CDOTAMatchMetadata
[Table("dota_gc_match_metadata")]
public class GcMatchMetadata
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [ForeignKey("League")]
    [Column("league_id")]
    public required int LeagueId { get; set; }

    [JsonIgnore]
    public League? League { get; set; }

    [Column("match_id")]
    public long MatchId { get; set; }

    [Column("lobby_id")]
    public ulong LobbyId { get; set; }

    [Column("report_until_time")]
    public ulong ReportUntilTime { get; set; }

    [Column("primary_event_id")]
    public uint PrimaryEventId { get; set; }
    public List<GcMatchMetadataTeam> Teams { get; set; } = [];
    public List<GcMatchMetadataTip> MatchTips { get; set; } = [];
}