namespace DataAccessLibrary.Models.Fantasy;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DataAccessLibrary.Models.ProMetadata;

[Table("fantasy_match_player")]
public class FantasyMatchPlayer
{
    [Key]
    [Column("id")]
    [JsonIgnore]
    public int Id { get; set; }

    [ForeignKey("FantasyMatch")]
    [Column("match_id")]
    public required long FantasyMatchId { get; set; }

    [JsonIgnore]
    public FantasyMatch? Match { get; set; }

    [ForeignKey("Account")]
    [Column("account_id")]
    public required long AccountId { get; set; }

    [JsonIgnore]
    public Account? Account { get; set; }
    public string AccountFormatted
    {
        get
        {
            return Account?.Name ?? string.Empty;
        }
    }

    [ForeignKey("Team")]
    [Column("team_id")]
    public required long TeamId { get; set; }

    [JsonIgnore]
    public Team? Team { get; set; }
    public string TeamFormatted
    {
        get
        {
            return Team?.Name ?? string.Empty;
        }
    }

    [Column("match_detail_player_parsed")]
    [JsonIgnore]
    public bool MatchDetailPlayerParsed { get; set; }

    [Column("gc_metadata_player_parsed")]
    [JsonIgnore]
    public bool GcMetadataPlayerParsed { get; set; }

    [Column("dota_team_side")]
    [JsonPropertyName("dotaTeamSide")]
    public bool DotaTeamSide { get; set; }
    public string DotaTeamSideFormatted
    {
        get
        {
            return DotaTeamSide ? "dire" : "radiant";
        }
    }

    [Column("player_slot")]
    [JsonPropertyName("playerSlot")]
    public int PlayerSlot { get; set; }
    public string PlayerSlotFormatted
    {
        get
        {
            switch (PlayerSlot)
            {
                case 0:
                    return "blue";
                case 1:
                    return "teal";
                case 2:
                    return "purple";
                case 3:
                    return "yellow";
                case 4:
                    return "orange";
                case 128:
                    return "pink";
                case 129:
                    return "grey";
                case 130:
                    return "light blue";
                case 131:
                    return "dark green";
                case 132:
                    return "brown";
                default:
                    return "unknown";
            }
        }
    }

    [JsonPropertyName("hero")]
    public Hero? Hero { get; set; }
    public string HeroFormatted
    {
        get
        {
            return Hero?.Name?.ToString() ?? string.Empty;
        }
    }

    [Column("kills")]
    [JsonPropertyName("kills")]
    public int? Kills { get; set; }

    [Column("deaths")]
    [JsonPropertyName("deaths")]
    public int? Deaths { get; set; }

    [Column("assists")]
    [JsonPropertyName("assists")]
    public int? Assists { get; set; }

    [Column("last_hits")]
    [JsonPropertyName("lastHits")]
    public int? LastHits { get; set; }

    [Column("denies")]
    [JsonPropertyName("denies")]
    public int? Denies { get; set; }

    [Column("gold_per_min")]
    [JsonPropertyName("goldPerMin")]
    public int? GoldPerMin { get; set; }

    [Column("xp_per_min")]
    [JsonPropertyName("xpPerMin")]
    public int? XpPerMin { get; set; }

    [Column("support_gold_spent")]
    [JsonPropertyName("supportGoldSpent")]
    public int? SupportGoldSpent { get; set; }

    [Column("observer_wards_placed")]
    [JsonPropertyName("observerWardsPlaced")]
    public int? ObserverWardsPlaced { get; set; }

    [Column("sentry_wards_placed")]
    [JsonPropertyName("sentryWardsPlaced")]
    public int? SentyWardsPlaced { get; set; }

    [Column("dewards")]
    [JsonPropertyName("dewards")]
    public int? Dewards { get; set; }

    [Column("camps_stacked")]
    [JsonPropertyName("campsStacked")]
    public int? CampsStacked { get; set; }

    [Column("stun_duration")]
    [JsonPropertyName("stunDuration")]
    public float? StunDuration { get; set; }

    [Column("level")]
    [JsonPropertyName("level")]
    public int? Level { get; set; }

    [Column("net_worth")]
    [JsonPropertyName("networth")]
    public long? Networth { get; set; }

    [Column("hero_damage")]
    [JsonPropertyName("heroDamage")]
    public int? HeroDamage { get; set; }

    [Column("tower_damage")]
    [JsonPropertyName("towerDamage")]
    public int? TowerDamage { get; set; }

    [Column("hero_healing")]
    [JsonPropertyName("heroHealing")]
    public int? HeroHealing { get; set; }

    [Column("gold")]
    [JsonPropertyName("gold")]
    public int? Gold { get; set; }

    // Match Metadata

    [Column("fight_score")]
    [JsonPropertyName("fightScore")]
    public float? FightScore { get; set; }

    [Column("farm_score")]
    [JsonPropertyName("farmScore")]
    public float? FarmScore { get; set; }

    [Column("support_score")]
    [JsonPropertyName("supportScore")]
    public float? SupportScore { get; set; }

    [Column("push_score")]
    [JsonPropertyName("pushScore")]
    public float? PushScore { get; set; }

    [Column("hero_xp")]
    [JsonPropertyName("heroXp")]
    public uint? HeroXp { get; set; }

    [Column("rampages")]
    [JsonPropertyName("rampages")]
    public uint? Rampages { get; set; }

    [Column("triple_kills")]
    [JsonPropertyName("tripleKills")]
    public uint? TripleKills { get; set; }

    [Column("aegis_snatched")]
    [JsonPropertyName("aegisSnatched")]
    public uint? AegisSnatched { get; set; }

    [Column("rapiers_purchased")]
    [JsonPropertyName("rapiersPurchased")]
    public uint? RapiersPurchased { get; set; }

    [Column("couriers_killed")]
    [JsonPropertyName("couriersKilled")]
    public uint? CouriersKilled { get; set; }
}