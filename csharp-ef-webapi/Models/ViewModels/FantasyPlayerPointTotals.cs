using System.ComponentModel.DataAnnotations.Schema;
using csharp_ef_webapi.Models.Fantasy;

namespace csharp_ef_webapi.Models;

// This is a view model and isn't saved in the db (in case we change the scoring)
public class FantasyPlayerPointTotals
{
    [Column("fantasy_league_id")]
    public int FantasyLeagueId { get; set; }

    [Column("fantasy_player_id")]
    public long FantasyPlayerId { get; set; }
    public FantasyPlayer FantasyPlayer { get; set; } = null!;

    [Column("matches")]
    public int TotalMatches { get; set; }

    [Column("kills")]
    public int TotalKills { get; set; }

    [Column("kills_points")]
    public decimal TotalKillsPoints { get; set; }

    [Column("deaths")]
    public int TotalDeaths { get; set; }

    [Column("deaths_points")]
    public decimal TotalDeathsPoints { get; set; }

    [Column("assists")]
    public int TotalAssists { get; set; }

    [Column("assists_points")]
    public decimal TotalAssistsPoints { get; set; }

    [Column("last_hits")]
    public int TotalLastHits { get; set; }

    [Column("last_hits_points")]
    public decimal TotalLastHitsPoints { get; set; }

    [Column("gold_per_min")]
    public double AvgGoldPerMin { get; set; }

    [Column("gold_per_min_points")]
    public decimal TotalGoldPerMinPoints { get; set; }

    [Column("xp_per_min")]
    public double AvgXpPerMin { get; set; }

    [Column("xp_per_min_points")]
    public decimal TotalXpPerMinPoints { get; set; }

    // Match Details
    [Column("networth")]
    public long TotalNetworth { get; set; }

    [Column("networth_points")]
    public decimal TotalNetworthPoints { get; set; }

    [Column("hero_damage")]
    public int TotalHeroDamage { get; set; }

    [Column("hero_damage_points")]
    public decimal TotalHeroDamagePoints { get; set; }

    [Column("tower_damage")]
    public int TotalTowerDamage { get; set; }

    [Column("tower_damage_points")]
    public decimal TotalTowerDamagePoints { get; set; }

    [Column("hero_healing")]
    public int TotalHeroHealing { get; set; }

    [Column("hero_healing_points")]
    public decimal TotalHeroHealingPoints { get; set; }

    [Column("gold")]
    public int TotalGold { get; set; }

    [Column("gold_points")]
    public decimal TotalGoldPoints { get; set; }

    // Match Metadata
    [Column("fight_score")]
    public float TotalFightScore { get; set; }

    [Column("fight_score_points")]
    public float TotalFightScorePoints { get; set; }

    [Column("farm_score")]
    public float TotalFarmScore { get; set; }

    [Column("farm_score_points")]
    public float TotalFarmScorePoints { get; set; }

    [Column("support_score")]
    public float TotalSupportScore { get; set; }

    [Column("support_score_points")]
    public float TotalSupportScorePoints { get; set; }

    [Column("push_score")]
    public float TotalPushScore { get; set; }

    [Column("push_score_points")]
    public float TotalPushScorePoints { get; set; }

    [Column("hero_xp")]
    public uint TotalHeroXp { get; set; }

    [Column("hero_xp_points")]
    public decimal TotalHeroXpPoints { get; set; }

    [Column("camps_stacked")]
    public uint TotalCampsStacked { get; set; }

    [Column("camps_stacked_points")]
    public decimal TotalCampsStackedPoints { get; set; }

    [Column("rampages")]
    public uint TotalRampages { get; set; }

    [Column("rampages_points")]
    public decimal TotalRampagesPoints { get; set; }

    [Column("triple_kills")]
    public uint TotalTripleKills { get; set; }

    [Column("triple_kills_points")]
    public decimal TotalTripleKillsPoints { get; set; }

    [Column("aegis_snatched")]
    public uint TotalAegisSnatched { get; set; }

    [Column("aegis_snatched_points")]
    public decimal TotalAegisSnatchedPoints { get; set; }

    [Column("rapiers_purchased")]
    public uint TotalRapiersPurchased { get; set; }

    [Column("rapiers_purchased_points")]
    public decimal TotalRapiersPurchasedPoints { get; set; }

    [Column("couriers_killed")]
    public uint TotalCouriersKilled { get; set; }

    [Column("couriers_killed_points")]
    public decimal TotalCouriersKilledPoints { get; set; }

    [Column("support_gold_spent")]
    public uint TotalSupportGoldSpent { get; set; }

    [Column("support_gold_spent_points")]
    public decimal TotalSupportGoldSpentPoints { get; set; }

    [Column("observer_wards_placed")]
    public uint TotalObserverWardsPlaced { get; set; }

    [Column("observer_wards_placed_points")]
    public decimal TotalObserverWardsPlacedPoints { get; set; }

    [Column("sentry_wards_placed")]
    public uint TotalSentryWardsPlaced { get; set; }

    [Column("sentry_wards_placed_points")]
    public decimal TotalSentryWardsPlacedPoints { get; set; }

    [Column("wards_dewarded")]
    public uint TotalWardsDewarded { get; set; }

    [Column("wards_dewarded_points")]
    public decimal TotalWardsDewardedPoints { get; set; }

    [Column("stun_duration")]
    public float TotalStunDuration { get; set; }

    [Column("stun_duration_points")]
    public float TotalStunDurationPoints { get; set; }

    [Column("total_match_fantasy_points")]
    public decimal TotalMatchFantasyPoints { get; set; }
}