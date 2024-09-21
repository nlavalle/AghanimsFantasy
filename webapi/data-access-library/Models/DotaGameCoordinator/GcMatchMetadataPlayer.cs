namespace DataAccessLibrary.Models.GameCoordinator;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// EF mapping for CDOTAMatchMetadata
[Table("dota_gc_match_metadata_player")]
public class GcMatchMetadataPlayer
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("player_slot")]
    public uint PlayerSlot { get; set; }

    [Column("avg_kills_x16")]
    public uint AvgKillsX16 { get; set; }

    [Column("avg_deaths_x16")]
    public uint AvgDeathsX16 { get; set; }

    [Column("avg_assists_x16")]
    public uint AvgAssistsX16 { get; set; }

    [Column("avg_gpm_x16")]
    public uint AvgGpmX16 { get; set; }

    [Column("avg_xpm_x16")]
    public uint AvgXpmX16 { get; set; }

    [Column("best_kills_x16")]
    public uint BestKillsX16 { get; set; }

    [Column("best_assists_x16")]
    public uint BestAssistsX16 { get; set; }

    [Column("best_gpm_x16")]
    public uint BestGpmX16 { get; set; }

    [Column("best_xpm_x16")]
    public uint BestXpmX16 { get; set; }

    [Column("win_streak")]
    public uint WinStreak { get; set; }

    [Column("best_win_streak")]
    public uint BestWinStreak { get; set; }

    [Column("fight_score")]
    public float FightScore { get; set; }

    [Column("farm_score")]
    public float FarmScore { get; set; }

    [Column("support_score")]
    public float SupportScore { get; set; }

    [Column("push_score")]
    public float PushScore { get; set; }

    [Column("avg_stats_calibrated")]
    public bool AvgStatsCalibrated { get; set; }

    [Column("hero_xp")]
    public uint HeroXp { get; set; }

    [Column("camps_stacked")]
    public uint CampsStacked { get; set; }

    [Column("lane_selection_flags")]
    public uint LaneSelectionFlags { get; set; }

    [Column("rampages")]
    public uint Rampages { get; set; }

    [Column("triple_kills")]
    public uint TripleKills { get; set; }

    [Column("aegis_snatched")]
    public uint AegisSnatched { get; set; }

    [Column("rapiers_purchased")]
    public uint RapiersPurchased { get; set; }

    [Column("couriers_killed")]
    public uint CouriersKilled { get; set; }

    [Column("net_worth_rank")]
    public uint NetworthRank { get; set; }

    [Column("support_gold_spent")]
    public uint SupportGoldSpent { get; set; }

    [Column("observer_wards_placed")]
    public uint ObserverWardsPlaced { get; set; }

    [Column("sentry_wards_placed")]
    public uint SentryWardsPlaced { get; set; }

    [Column("wards_dewarded")]
    public uint WardsDewarded { get; set; }

    [Column("stun_duration")]
    public float StunDuration { get; set; }

    [Column("team_slot")]
    public uint TeamSlot { get; set; }

    [Column("featured_hero_sticker_index")]
    public uint FeaturedHeroStickerIndex { get; set; }

    [Column("featured_hero_sticker_quality")]
    public uint FeaturedHeroStickerQuality { get; set; }

    [Column("game_player_id")]
    public int GamePlayerId { get; set; }
    public List<int> AbilityUpgrades { get; set; } = [];
    public List<GcMatchMetadataPlayerKill> Kills { get; set; } = [];
    public List<GcMatchMetadataItemPurchase> Items { get; set; } = [];
}
