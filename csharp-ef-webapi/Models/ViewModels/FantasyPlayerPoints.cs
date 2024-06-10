namespace csharp_ef_webapi.Models;

using System.ComponentModel.DataAnnotations.Schema;
using csharp_ef_webapi.Models.Fantasy;
using csharp_ef_webapi.Models.WebApi;

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

    // Match Details
    [Column("networth")]
    public long? Networth { get; set; }

    [Column("networth_points")]
    public decimal? NetworthPoints { get; set; }

    [Column("hero_damage")]
    public int? HeroDamage { get; set; }

    [Column("hero_damage_points")]
    public decimal? HeroDamagePoints { get; set; }

    [Column("tower_damage")]
    public int? TowerDamage { get; set; }

    [Column("tower_damage_points")]
    public decimal? TowerDamagePoints { get; set; }

    [Column("hero_healing")]
    public int? HeroHealing { get; set; }

    [Column("hero_healing_points")]
    public decimal? HeroHealingPoints { get; set; }

    [Column("gold")]
    public int? Gold { get; set; }

    [Column("gold_points")]
    public decimal? GoldPoints { get; set; }

    // Match Metadata
    [Column("fight_score")]
    public float? FightScore { get; set; }

    [Column("fight_score_points")]
    public float? FightScorePoints { get; set; }

    [Column("farm_score")]
    public float? FarmScore { get; set; }

    [Column("farm_score_points")]
    public float? FarmScorePoints { get; set; }

    [Column("support_score")]
    public float? SupportScore { get; set; }

    [Column("support_score_points")]
    public float? SupportScorePoints { get; set; }

    [Column("push_score")]
    public float? PushScore { get; set; }

    [Column("push_score_points")]
    public float? PushScorePoints { get; set; }

    [Column("hero_xp")]
    public uint? HeroXp { get; set; }

    [Column("hero_xp_points")]
    public decimal? HeroXpPoints { get; set; }

    [Column("camps_stacked")]
    public uint? CampsStacked { get; set; }

    [Column("camps_stacked_points")]
    public decimal? CampsStackedPoints { get; set; }

    [Column("rampages")]
    public uint? Rampages { get; set; }

    [Column("rampages_points")]
    public decimal? RampagesPoints { get; set; }

    [Column("triple_kills")]
    public uint? TripleKills { get; set; }

    [Column("triple_kills_points")]
    public decimal? TripleKillsPoints { get; set; }

    [Column("aegis_snatched")]
    public uint? AegisSnatched { get; set; }

    [Column("aegis_snatched_points")]
    public decimal? AegisSnatchedPoints { get; set; }

    [Column("rapiers_purchased")]
    public uint? RapiersPurchased { get; set; }

    [Column("rapiers_purchased_points")]
    public decimal? RapiersPurchasedPoints { get; set; }

    [Column("couriers_killed")]
    public uint? CouriersKilled { get; set; }

    [Column("couriers_killed_points")]
    public decimal? CouriersKilledPoints { get; set; }

    [Column("support_gold_spent")]
    public uint? SupportGoldSpent { get; set; }

    [Column("support_gold_spent_points")]
    public decimal? SupportGoldSpentPoints { get; set; }

    [Column("observer_wards_placed")]
    public uint? ObserverWardsPlaced { get; set; }

    [Column("observer_wards_placed_points")]
    public decimal? ObserverWardsPlacedPoints { get; set; }

    [Column("sentry_wards_placed")]
    public uint? SentryWardsPlaced { get; set; }

    [Column("sentry_wards_placed_points")]
    public decimal? SentryWardsPlacedPoints { get; set; }

    [Column("wards_dewarded")]
    public uint? WardsDewarded { get; set; }

    [Column("wards_dewarded_points")]
    public decimal? WardsDewardedPoints { get; set; }

    [Column("stun_duration")]
    public float? StunDuration { get; set; }

    [Column("stun_duration_points")]
    public float? StunDurationPoints { get; set; }

    [Column("total_match_fantasy_points")]
    public decimal? TotalMatchFantasyPoints { get; set; }
}