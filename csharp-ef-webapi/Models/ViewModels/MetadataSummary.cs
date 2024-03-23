namespace csharp_ef_webapi.Models;

// This is a view model and isn't saved in the db (in case we change the scoring)
public class MetadataSummary
{
    public FantasyPlayer Player { get; set; } = null!;
    public int? MatchesPlayed {get;set;}
    // Match Details
    public int? Kills { get; set; }
    public double? KillsAverage { get; set; }
    public int? Deaths { get; set; }
    public double? DeathsAverage { get; set; }
    public int? Assists { get; set; }
    public double? AssistsAverage { get; set; }
    public int? LastHits { get; set; }
    public double? LastHitsAverage { get; set; }
    public int? Denies { get; set; }
    public double? DeniesAverage { get; set; }
    public int? GoldPerMin { get; set; }
    public double? GoldPerMinAverage { get; set; }
    public int? XpPerMin { get; set; }
    public double? XpPerMinAverage { get; set; }
    public long? Networth { get; set; }
    public double? NetworthAverage { get; set; }
    public int? HeroDamage { get; set; }
    public double? HeroDamageAverage { get; set; }
    public int? TowerDamage { get; set; }
    public double? TowerDamageAverage { get; set; }
    public int? HeroHealing { get; set; }
    public double? HeroHealingAverage { get; set; }
    public int? Gold { get; set; }
    public double? GoldAverage { get; set; }
    public int? ScaledHeroDamage { get; set; }
    public double? ScaledHeroDamageAverage { get; set; }
    public int? ScaledTowerDamage { get; set; }
    public double? ScaledTowerDamageAverage { get; set; }
    public int? ScaledHeroHealing { get; set; }
    public double? ScaledHeroHealingAverage { get; set; }

    // Match Metadata
    public uint WinStreak { get; set; }
    public uint BestWinStreak { get; set; }
    public float FightScore { get; set; }
    public double? FightScoreAverage { get; set; }
    public float FarmScore { get; set; }
    public double? FarmScoreAverage { get; set; }
    public float SupportScore { get; set; }
    public double? SupportScoreAverage { get; set; }
    public float PushScore { get; set; }
    public double? PushScoreAverage { get; set; }
    public uint HeroXp { get; set; }
    public double? HeroXpAverage { get; set; }
    public uint CampsStacked { get; set; }
    public double? CampsStackedAverage { get; set; }
    public uint Rampages { get; set; }
    public double? RampagesAverage { get; set; }
    public uint TripleKills { get; set; }
    public double? TripleKillsAverage { get; set; }
    public uint AegisSnatched { get; set; }
    public double? AegisSnatchedAverage { get; set; }
    public uint RapiersPurchased { get; set; }
    public double? RapiersPurchasedAverage { get; set; }
    public uint CouriersKilled { get; set; }
    public double? CouriersKilledAverage { get; set; }
    public uint NetworthRank { get; set; }
    public double? NetworthRankAverage { get; set; }
    public uint SupportGoldSpent { get; set; }
    public double? SupportGoldSpentAverage { get; set; }
    public uint ObserverWardsPlaced { get; set; }
    public double? ObserverWardsPlacedAverage { get; set; }
    public uint SentryWardsPlaced { get; set; }
    public double? SentryWardsPlacedAverage { get; set; }
    public uint WardsDewarded { get; set; }
    public double? WardsDewardedAverage { get; set; }
    public float StunDuration { get; set; }
    public double? StunDurationAverage { get; set; }
}