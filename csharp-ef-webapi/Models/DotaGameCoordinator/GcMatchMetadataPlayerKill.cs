namespace csharp_ef_webapi.Models.GameCoordinator;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// EF mapping for CDOTAMatchMetadata
[Table("dota_gc_match_metadata_playerkill")]
public class GcMatchMetadataPlayerKill
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
    [Column("victim_slot")]
    public uint VictimSlot { get; set; }
    [Column("count")]
    public uint Count { get; set; }
}
