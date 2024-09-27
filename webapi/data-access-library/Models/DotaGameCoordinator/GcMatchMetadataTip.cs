namespace DataAccessLibrary.Models.GameCoordinator;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// EF mapping for CDOTAMatchMetadata
[Table("dota_gc_match_metadata_tip")]
public class GcMatchMetadataTip
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("source_player_slot")]
    public uint SourcePlayerSlot { get; set; }

    [Column("target_player_slot")]
    public uint TargetPlayerSlot { get; set; }

    [Column("tip_amount")]
    public uint TipAmount { get; set; }
}
