namespace csharp_ef_webapi.Models;

// This is a view model and isn't saved in the db (in case we change the scoring)
public class MetadataSummary
{
    public FantasyPlayer Player { get; set; } = null!;
    public MatchDetailsPlayer MatchDetailsPlayers { get; set; } = null!;
    public GcMatchMetadataPlayer MetadataPlayer { get; set; } = null!;
    // Match Details
    public int? Kills { get { return MatchDetailsPlayers.Kills; } }
    public int? Deaths { get { return MatchDetailsPlayers.Deaths; } }
    public int? Assists { get { return MatchDetailsPlayers.Assists; } }
    public int? LastHits { get { return MatchDetailsPlayers.LastHits; } }
    public int? Denies { get { return MatchDetailsPlayers.Denies; } }
    public int? GoldPerMin { get { return MatchDetailsPlayers.GoldPerMin; } }
    public int? XpPerMin { get { return MatchDetailsPlayers.XpPerMin; } }
    public long? Networth { get { return MatchDetailsPlayers.Networth; } }
    public int? HeroDamage { get { return MatchDetailsPlayers.HeroDamage; } }
    public int? TowerDamage { get { return MatchDetailsPlayers.TowerDamage; } }
    public int? HeroHealing { get { return MatchDetailsPlayers.HeroHealing; } }
    public int? Gold { get { return MatchDetailsPlayers.Gold; } }
    public int? ScaledHeroDamage { get { return MatchDetailsPlayers.ScaledHeroDamage; } }
    public int? ScaledTowerDamage { get { return MatchDetailsPlayers.ScaledTowerDamage; } }
    public int? ScaledHeroHealing { get { return MatchDetailsPlayers.ScaledHeroHealing; } }

    // Match Metadata
    public uint WinStreak { get { return MetadataPlayer.WinStreak; } }
    public uint BestWinStreak { get { return MetadataPlayer.BestWinStreak; } }
    public float FightScore { get { return MetadataPlayer.FightScore; } }
    public float FarmScore { get { return MetadataPlayer.FarmScore; } }
    public float SupportScore { get { return MetadataPlayer.SupportScore; } }
    public float PushScore { get { return MetadataPlayer.PushScore; } }
    public uint HeroXp { get { return MetadataPlayer.HeroXp; } }
    public uint CampsStacked { get { return MetadataPlayer.CampsStacked; } }
    public uint Rampages { get { return MetadataPlayer.Rampages; } }
    public uint TripleKills { get { return MetadataPlayer.TripleKills; } }
    public uint AegisSnatched { get { return MetadataPlayer.AegisSnatched; } }
    public uint RapiersPurchased { get { return MetadataPlayer.RapiersPurchased; } }
    public uint CouriersKilled { get { return MetadataPlayer.CouriersKilled; } }
    public uint NetworthRank { get { return MetadataPlayer.NetworthRank; } }
    public uint SupportGoldSpent { get { return MetadataPlayer.SupportGoldSpent; } }
    public uint ObserverWardsPlaced { get { return MetadataPlayer.ObserverWardsPlaced; } }
    public uint SentryWardsPlaced { get { return MetadataPlayer.SentryWardsPlaced; } }
    public uint WardsDewarded { get { return MetadataPlayer.WardsDewarded; } }
    public float StunDuration { get { return MetadataPlayer.StunDuration; } }
}