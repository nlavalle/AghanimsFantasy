namespace DataAccessLibrary.Models.Fantasy;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DataAccessLibrary.Models.ProMetadata;

[Table("dota_fantasy_leagues")]
public class FantasyLeague
{
    [Column("id")]
    public int Id { get; set; }

    [ForeignKey("League")]
    [Column("league_id")]
    public required int LeagueId { get; set; }

    [JsonIgnore]
    public League? League { get; set; }

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

    [JsonIgnore]
    public FantasyLeagueWeight? FantasyLeagueWeight { get; set; }

    [JsonIgnore]
    public List<FantasyDraft> FantasyDrafts { get; set; } = new List<FantasyDraft>();

    [JsonIgnore]
    public List<FantasyPlayer> FantasyPlayers { get; set; } = new List<FantasyPlayer>();

    [JsonIgnore]
    public List<FantasyPrivateLeaguePlayer> FantasyPrivateLeaguePlayers { get; set; } = new List<FantasyPrivateLeaguePlayer>();
}