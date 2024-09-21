namespace DataAccessLibrary.Models.Fantasy;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("dota_fantasy_league_weights")]
public class FantasyLeagueWeight
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("fantasy_league_id")]
    public required FantasyLeague FantasyLeague { get; set; }

    [Column("kills_weight")]
    public decimal KillsWeight { get; set; } = 0.3M;
    [Column("deaths_weight")]
    public decimal DeathsWeight { get; set; } = -0.3M;
    [Column("assists_weight")]
    public decimal AssistsWeight { get; set; } = 0.15M;
    [Column("last_hits_weight")]
    public decimal LastHitsWeight { get; set; } = 0.003M;
    [Column("gold_per_min_weight")]
    public decimal GoldPerMinWeight { get; set; } = 0.002M;
    [Column("xp_per_min_weight")]
    public decimal XpPerMinWeight { get; set; } = 0.002M;

    // Match Details
    [Column("networth_weight")]
    public decimal NetworthWeight { get; set; }
    [Column("hero_damage_weight")]
    public decimal HeroDamageWeight { get; set; }
    [Column("tower_damage_weight")]
    public decimal TowerDamageWeight { get; set; }
    [Column("hero_healing_weight")]
    public decimal HeroHealingWeight { get; set; }
    [Column("gold_weight")]
    public decimal GoldWeight { get; set; }
    [Column("scaled_hero_damage_weight")]
    public decimal ScaledHeroDamageWeight { get; set; }
    [Column("scaled_tower_damage_weight")]
    public decimal ScaledTowerDamageWeight { get; set; }
    [Column("scaled_hero_healing_weight")]
    public decimal ScaledHeroHealingWeight { get; set; }

    // Match Metadata
    [Column("fight_score_weight")]
    public decimal FightScoreWeight { get; set; }
    [Column("farm_score_weight")]
    public decimal FarmScoreWeight { get; set; }
    [Column("support_score_weight")]
    public decimal SupportScoreWeight { get; set; }
    [Column("push_score_weight")]
    public decimal PushScoreWeight { get; set; }
    [Column("hero_xp_weight")]
    public decimal HeroXpWeight { get; set; }
    [Column("camps_stacked_weight")]
    public decimal CampsStackedWeight { get; set; }
    [Column("rampages_weight")]
    public decimal RampagesWeight { get; set; }
    [Column("triple_kills_weight")]
    public decimal TripleKillsWeight { get; set; }
    [Column("aegis_snatched_weight")]
    public decimal AegisSnatchedWeight { get; set; }
    [Column("rapiers_purchased_weight")]
    public decimal RapiersPurchasedWeight { get; set; }
    [Column("couriers_killed_weight")]
    public decimal CouriersKilledWeight { get; set; }
    [Column("networth_rank_weight")]
    public decimal NetworthRankWeight { get; set; }
    [Column("support_gold_spent_weight")]
    public decimal SupportGoldSpentWeight { get; set; }
    [Column("observer_wards_placed_weight")]
    public decimal ObserverWardsPlacedWeight { get; set; }
    [Column("sentry_wards_placed_weight")]
    public decimal SentryWardsPlacedWeight { get; set; }
    [Column("wards_dewarded_weight")]
    public decimal WardsDewardedWeight { get; set; }
    [Column("stun_duration_weight")]
    public decimal StunDurationWeight { get; set; }
}