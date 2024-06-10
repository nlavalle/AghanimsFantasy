namespace csharp_ef_webapi.Models.GameCoordinator;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// EF mapping for CDOTAMatchMetadata
[Table("dota_gc_match_metadata")]
public class GcMatchMetadata
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
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