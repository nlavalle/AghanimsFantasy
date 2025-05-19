namespace DataAccessLibrary.Models.Fantasy;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

[Table("dota_fantasy_drafts")]
public class FantasyDraft
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [ForeignKey("FantasyLeague")]
    [Column("fantasy_league_id")]
    public required int FantasyLeagueId { get; set; }

    [JsonIgnore]
    public FantasyLeague? FantasyLeague { get; set; }

    [Column("discord_account_id")]
    public long? DiscordAccountId { get; set; }

    [Column("user_id")]
    public string? UserId { get; set; }

    [Column("draft_created")]
    public DateTime? DraftCreated { get; set; }

    [Column("draft_last_updated")]
    public DateTime? DraftLastUpdated { get; set; }
    public List<FantasyDraftPlayer> DraftPickPlayers { get; set; } = new List<FantasyDraftPlayer>();
}