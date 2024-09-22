namespace DataAccessLibrary.Models.GameCoordinator;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccessLibrary.Models.ProMetadata;

// EF mapping for CDOTAMatchMetadata
[Table("dota_gc_match_metadata")]
public class GcMatchMetadata
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public League League { get; set; } = null!;

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