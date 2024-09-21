namespace DataAccessLibrary.Models.Fantasy;

using System.ComponentModel.DataAnnotations.Schema;
using DataAccessLibrary.Models.ProMetadata;

[Table("dota_fantasy_leagues")]
public class FantasyLeague
{
    [Column("id")]
    public int Id { get; set; }
    public required League League { get; set; }

    [Column("league_name")]
    public string? Name { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("is_private")]
    public bool IsPrivate { get; set; } = false;

    [Column("fantasy_draft_locked_date")]
    public long FantasyDraftLocked { get; set; }

    [Column("league_start_time")]
    public long LeagueStartTime { get; set; }

    [Column("league_end_time")]
    public long LeagueEndTime { get; set; }
    public FantasyLeagueWeight? FantasyLeagueWeight { get; set; }
    public List<FantasyDraft> FantasyDrafts { get; set; } = new List<FantasyDraft>();
    public List<FantasyPlayer> FantasyPlayers { get; set; } = new List<FantasyPlayer>();
    public List<FantasyPrivateLeaguePlayer> FantasyPrivateLeaguePlayers { get; set; } = new List<FantasyPrivateLeaguePlayer>();
}