namespace csharp_ef_webapi.Models;

using System.ComponentModel.DataAnnotations.Schema;

// This is a join table we need for the many to one relationship between drafts and players
[Table("dota_fantasy_draft_players")]
public class FantasyDraftPlayer
{
    public long FantasyDraftId { get; set; }
    public FantasyDraft FantasyDraft { get; set; }
    public long FantasyPlayerId { get; set; }
    public FantasyPlayer FantasyPlayer { get; set; }
    public int DraftOrder { get; set; }
}