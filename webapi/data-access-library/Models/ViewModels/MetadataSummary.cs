namespace DataAccessLibrary.Models;

using System.ComponentModel.DataAnnotations.Schema;
using DataAccessLibrary.Models.Fantasy;

// This is a view model and isn't saved in the db (in case we change the scoring)
public class MetadataSummary
{
    [Column("fantasy_league_id")]
    public long FantasyLeagueId { get; set; }

    [Column("fantasy_player_id")]
    public long FantasyPlayerId { get; set; }
    public FantasyPlayer? FantasyPlayer { get; set; }

    [Column("matches_played")]
    public int? MatchesPlayed { get; set; }

    [Column("kills_sum")]
    public int? Kills { get; set; }

    [Column("kills_avg")]
    public double? KillsAverage { get; set; }

    [Column("deaths_sum")]
    public int? Deaths { get; set; }

    [Column("deaths_avg")]
    public double? DeathsAverage { get; set; }

    [Column("assists_sum")]
    public int? Assists { get; set; }

    [Column("assists_avg")]
    public double? AssistsAverage { get; set; }

    [Column("last_hits_sum")]
    public int? LastHits { get; set; }

    [Column("last_hits_avg")]
    public double? LastHitsAverage { get; set; }

    [Column("denies_sum")]
    public int? Denies { get; set; }

    [Column("denies_avg")]
    public double? DeniesAverage { get; set; }

    [Column("gold_per_min_sum")]
    public int? GoldPerMin { get; set; }

    [Column("gold_per_min_avg")]
    public double? GoldPerMinAverage { get; set; }

    [Column("xp_per_min_sum")]
    public int? XpPerMin { get; set; }

    [Column("xp_per_min_avg")]
    public double? XpPerMinAverage { get; set; }

    [Column("support_gold_spent_sum")]
    public uint SupportGoldSpent { get; set; }

    [Column("support_gold_spent_avg")]
    public double? SupportGoldSpentAverage { get; set; }

    [Column("observer_wards_placed_sum")]
    public uint ObserverWardsPlaced { get; set; }

    [Column("observer_wards_placed_avg")]
    public double? ObserverWardsPlacedAverage { get; set; }

    [Column("sentry_wards_placed_sum")]
    public uint SentryWardsPlaced { get; set; }

    [Column("sentry_wards_placed_avg")]
    public double? SentryWardsPlacedAverage { get; set; }

    [Column("dewards_sum")]
    public uint WardsDewarded { get; set; }

    [Column("dewards_avg")]
    public double? WardsDewardedAverage { get; set; }

    [Column("camps_stacked_sum")]
    public uint CampsStacked { get; set; }

    [Column("camps_stacked_avg")]
    public double? CampsStackedAverage { get; set; }

    [Column("stun_duration_sum")]
    public float StunDuration { get; set; }

    [Column("stun_duration_avg")]
    public double? StunDurationAverage { get; set; }

    [Column("net_worth_sum")]
    public long? Networth { get; set; }

    [Column("net_worth_avg")]
    public double? NetworthAverage { get; set; }

    [Column("hero_damage_sum")]
    public int? HeroDamage { get; set; }

    [Column("hero_damage_avg")]
    public double? HeroDamageAverage { get; set; }

    [Column("tower_damage_sum")]
    public int? TowerDamage { get; set; }

    [Column("tower_damage_avg")]
    public double? TowerDamageAverage { get; set; }

    [Column("hero_healing_sum")]
    public int? HeroHealing { get; set; }

    [Column("hero_healing_avg")]
    public double? HeroHealingAverage { get; set; }

    [Column("gold_sum")]
    public int? Gold { get; set; }

    [Column("gold_avg")]
    public double? GoldAverage { get; set; }
}