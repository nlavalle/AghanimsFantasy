namespace DataAccessLibrary.Models.Fantasy;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// This is a view model and isn't saved in the db (in case we change the scoring)
public class FantasyNormalizedAveragesTable
{
    [Key]
    [Column("fantasy_normalized_averages_table_id")]
    public long FantasyNormalizedAveragesTableId { get; set; }

    [Column("fantasy_player_id")]
    public FantasyPlayer FantasyPlayer { get; set; } = null!;

    [Column("matches_played")]
    public decimal? TotalMatches { get; set; }

    [Column("kills_points")]
    public decimal? AvgKillsPoints { get; set; }

    [Column("deaths_points")]
    public decimal? AvgDeathsPoints { get; set; }

    [Column("assists_points")]
    public decimal? AvgAssistsPoints { get; set; }

    [Column("last_hits_points")]
    public decimal? AvgLastHitsPoints { get; set; }

    [Column("gold_per_min_points")]
    public decimal? AvgGoldPerMinPoints { get; set; }

    [Column("xp_per_min_points")]
    public decimal? AvgXpPerMinPoints { get; set; }

    // Match Details
    [Column("networth_points")]
    public decimal? AvgNetworthPoints { get; set; }

    [Column("hero_damage_points")]
    public decimal? AvgHeroDamagePoints { get; set; }

    [Column("tower_damage_points")]
    public decimal? AvgTowerDamagePoints { get; set; }

    [Column("hero_healing_points")]
    public decimal? AvgHeroHealingPoints { get; set; }

    [Column("gold_points")]
    public decimal? AvgGoldPoints { get; set; }

    // Match Metadata
    [Column("fight_score")]
    public float? AvgFightScore { get; set; }

    [Column("farm_score")]
    public float? AvgFarmScore { get; set; }

    [Column("support_score")]
    public float? AvgSupportScore { get; set; }

    [Column("push_score")]
    public float? AvgPushScore { get; set; }

    [Column("hero_xp_points")]
    public decimal? AvgHeroXpPoints { get; set; }

    [Column("camps_stacked_points")]
    public decimal? AvgCampsStackedPoints { get; set; }

    [Column("rampages_points")]
    public decimal? AvgRampagesPoints { get; set; }

    [Column("triple_kills_points")]
    public decimal? AvgTripleKillsPoints { get; set; }

    [Column("aegis_snatched_points")]
    public decimal? AvgAegisSnatchedPoints { get; set; }

    [Column("rapiers_purchased_points")]
    public decimal? AvgRapiersPurchasedPoints { get; set; }

    [Column("couriers_killed_points")]
    public decimal? AvgCouriersKilledPoints { get; set; }

    [Column("support_gold_spent_points")]
    public decimal? AvgSupportGoldSpentPoints { get; set; }

    [Column("observer_wards_placed_points")]
    public decimal? AvgObserverWardsPlacedPoints { get; set; }

    [Column("sentry_wards_placed_points")]
    public decimal? AvgSentryWardsPlacedPoints { get; set; }

    [Column("wards_dewarded_points")]
    public decimal? AvgWardsDewardedPoints { get; set; }

    [Column("stun_duration_points")]
    public float? AvgStunDurationPoints { get; set; }

    [Column("total_match_fantasy_points")]
    public decimal? AvgMatchFantasyPoints { get; set; }
}