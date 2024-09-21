namespace DataAccessLibrary.Models.GameCoordinator;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// EF mapping for CDOTAMatchMetadata
[Table("dota_gc_match_metadata_itempurchase")]
public class GcMatchMetadataItemPurchase
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("item_id")]
    public uint ItemId { get; set; }

    [Column("purchase_time")]
    public uint PurchaseTime { get; set; }
}