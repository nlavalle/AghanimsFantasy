using System.ComponentModel.DataAnnotations.Schema;

namespace csharp_ef_webapi.Models;

// This is a view model and read-only
public class FantasyPlayerPoints
{
    [Column("fantasy_league_id")]
    public int FantasyLeagueId { get; set; }
    [Column("fantasy_player_id")]
    public long FantasyPlayerId { get; set; }
    public FantasyPlayer FantasyPlayer { get; set; } = null!;
    [Column("match_details_player_id")]
    public int? MatchDetailsPlayerId { get; set; }
    public MatchDetailsPlayer? Match { get; set; }
    [Column("kills")]
    public int? Kills { get; set; }
    [Column("kills_points")]
    public decimal? KillsPoints { get; set; }
    [Column("deaths")]
    public int? Deaths { get; set; }
    [Column("deaths_points")]
    public decimal? DeathsPoints { get; set; }
    [Column("assists")]
    public int? Assists { get; set; }
    [Column("assists_points")]
    public decimal? AssistsPoints { get; set; }
    [Column("last_hits")]
    public int? LastHits { get; set; }
    [Column("last_hits_points")]
    public decimal? LastHitsPoints { get; set; }
    [Column("gold_per_min")]
    public int? GoldPerMin { get; set; }
    [Column("gold_per_min_points")]
    public decimal? GoldPerMinPoints { get; set; }
    [Column("xp_per_min")]
    public int? XpPerMin { get; set; }
    [Column("xp_per_min_points")]
    public decimal? XpPerMinPoints { get; set; }
    [Column("total_match_fantasy_points")]
    public decimal? TotalMatchFantasyPoints { get; set; }
}