namespace csharp_ef_webapi.Models;

using System.ComponentModel.DataAnnotations.Schema;

[Table("dota_fantasy_leagues")]
public class FantasyLeague
{
    [Column("id")]
    public int Id { get; set; }

    [Column("league_id")]
    public int LeagueId { get; set; }

    [Column("league_name")]
    public string? Name { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("fantasy_draft_locked_date")]
    public long FantasyDraftLocked { get; set; }

    [Column("league_start_time")]
    public long LeagueStartTime { get; set; }

    [Column("league_end_time")]
    public long LeagueEndTime { get; set; }
    public List<FantasyDraft> FantasyDrafts { get; set; } = new List<FantasyDraft>();
    public List<FantasyPlayer> FantasyPlayers { get; set; } = new List<FantasyPlayer>();
}