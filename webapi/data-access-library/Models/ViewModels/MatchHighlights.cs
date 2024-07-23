namespace DataAccessLibrary.Models;

using System.ComponentModel.DataAnnotations.Schema;
using DataAccessLibrary.Models.Fantasy;

// This is a view model and isn't saved in the db
public class MatchHighlights
{
    [Column("fantasy_league_id")]
    public int FantasyLeagueId { get; set; }
    [Column("team_id")]
    public int TeamId { get; set; }
    [Column("match_id")]
    public long MatchId { get; set; }
    [Column("start_time")]
    public long StartTime { get; set; }
    [Column("fantasy_player_id")]
    public long FantasyPlayerId { get; set; }
    public FantasyPlayer FantasyPlayer { get; set; } = null!;
    [Column("kills")]
    public int? Kills { get; set; }
    [Column("kills_points")]
    public decimal? KillsPoints { get; set; }
    [Column("kills_diff")]
    public decimal? KillsDiff { get; set; }
    [Column("kills_points_deviation")]
    public bool KillsPointsDeviation { get; set; }
    [Column("deaths")]
    public int? Deaths { get; set; }
    [Column("deaths_points")]
    public decimal? DeathsPoints { get; set; }
    [Column("deaths_diff")]
    public decimal? DeathsDiff { get; set; }
    [Column("deaths_points_deviation")]
    public bool DeathsPointsDeviation { get; set; }
    [Column("assists")]
    public int? Assists { get; set; }
    [Column("assists_points")]
    public decimal? AssistsPoints { get; set; }
    [Column("assists_diff")]
    public decimal? AssistsDiff { get; set; }
    [Column("assists_points_deviation")]
    public bool AssistsPointsDeviation { get; set; }
    [Column("last_hits")]
    public int? LastHits { get; set; }
    [Column("last_hits_points")]
    public decimal? LastHitsPoints { get; set; }
    [Column("last_hits_diff")]
    public decimal? LastHitsDiff { get; set; }
    [Column("last_hits_points_deviation")]
    public bool LastHitsPointsDeviation { get; set; }
    [Column("gold_per_min")]
    public int? GoldPerMin { get; set; }
    [Column("gold_per_min_points")]
    public decimal? GoldPerMinPoints { get; set; }
    [Column("gold_per_min_diff")]
    public decimal? GoldPerMinDiff { get; set; }
    [Column("gold_per_min_deviation")]
    public bool GoldPerMinPointsDeviation { get; set; }
    [Column("xp_per_min")]
    public int? XpPerMin { get; set; }
    [Column("xp_per_min_points")]
    public decimal? XpPerMinPoints { get; set; }
    [Column("xp_per_min_diff")]
    public decimal? XpPerMinDiff { get; set; }
    [Column("xp_per_min_deviation")]
    public bool XpPerMinPointsDeviation { get; set; }
    [Column("total_match_fantasy_points")]
    public decimal? TotalMatchFantasyPoints { get; set; }
}