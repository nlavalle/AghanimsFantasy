namespace DataAccessLibrary.Models.Fantasy;

using System.ComponentModel.DataAnnotations.Schema;

// This is a join table we need for the many to one relationship between drafts and players
[Table("dota_fantasy_draft_players")]
public class FantasyDraftPlayer
{
    [Column("fantasy_draft_id")]
    public long FantasyDraftId { get; set; }
    public FantasyPlayer? FantasyPlayer { get; set; }

    [Column("draft_order")]
    public int DraftOrder { get; set; }
}