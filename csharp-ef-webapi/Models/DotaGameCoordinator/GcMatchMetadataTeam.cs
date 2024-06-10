namespace csharp_ef_webapi.Models.GameCoordinator;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// EF mapping for CDOTAMatchMetadata
[Table("dota_gc_match_metadata_team")]
public class GcMatchMetadataTeam
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
    [Column("dota_team")]
    public uint DotaTeam { get; set; }
    [Column("cm_first_pick")]
    public bool CmFirstPick { get; set; }
    [Column("cm_captain_player_id")]
    public int CmCaptainPlayerId { get; set; }
    [Column("cm_penalty")]
    public uint CmPenalty { get; set; }
    public List<GcMatchMetadataPlayer> Players { get; set; } = [];
    public List<float> GraphExperience { get; set; } = [];
    public List<float> GraphGoldEarned { get; set; } = [];
    public List<float> GraphNetworth { get; set; } = [];
}
