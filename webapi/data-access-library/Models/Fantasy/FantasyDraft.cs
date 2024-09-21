namespace DataAccessLibrary.Models.Fantasy;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("dota_fantasy_drafts")]
public class FantasyDraft
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("fantasy_league_id")]
    public required FantasyLeague FantasyLeague { get; set; }

    [Column("discord_account_id")]
    public long? DiscordAccountId { get; set; }

    [Column("draft_created")]
    public DateTime? DraftCreated { get; set; }

    [Column("draft_last_updated")]
    public DateTime? DraftLastUpdated { get; set; }
    public List<FantasyDraftPlayer> DraftPickPlayers { get; set; } = new List<FantasyDraftPlayer>();
}