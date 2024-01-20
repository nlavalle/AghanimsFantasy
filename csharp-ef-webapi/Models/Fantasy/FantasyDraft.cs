namespace csharp_ef_webapi.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("dota_fantasy_drafts")]
public class FantasyDraft
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("league_id")]
    public int LeagueId { get; set; }

    [Column("discord_account_id")]
    public long? DiscordAccountId { get; set; }

    [Column("draft_created")]
    public DateTime? DraftCreated { get; set; }

    [Column("draft_last_updated")]
    public DateTime? DraftLastUpdated { get; set; }

    [Column("draft_pick_one")]
    public long DraftPickOne { get; set; }

    [Column("draft_pick_two")]
    public long DraftPickTwo { get; set; }

    [Column("draft_pick_three")]
    public long DraftPickThree { get; set; }

    [Column("draft_pick_four")]
    public long DraftPickFour { get; set; }

    [Column("draft_pick_five")]
    public long DraftPickFive { get; set; }
    public List<FantasyDraftPlayer> DraftPickPlayers { get; set; } = new List<FantasyDraftPlayer>();
}