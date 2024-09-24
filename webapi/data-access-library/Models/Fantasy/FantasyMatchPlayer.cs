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
    [JsonPropertyName("match_detail_player_parsed")]
    public bool MatchDetailPlayerParsed { get; set; }

    [Column("gc_metadata_player_parsed")]
    [JsonPropertyName("gc_metadata_player_parsed")]
    public bool GcMetadataPlayerParsed { get; set; }

    [Column("dota_team_side")]
    [JsonPropertyName("dota_team_side")]
    public bool DotaTeamSide { get; set; }
    public string DotaTeamSideFormatted
    {
        get
        {
            return DotaTeamSide ? "dire" : "radiant";
        }
    }

    [Column("player_slot")]
    [JsonPropertyName("player_slot")]
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

    [JsonPropertyName("hero_id")]
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
    [JsonPropertyName("last_hits")]
    public int? LastHits { get; set; }

    [Column("denies")]
    [JsonPropertyName("denies")]
    public int? Denies { get; set; }

    [Column("gold_per_min")]
    [JsonPropertyName("gold_per_min")]
    public int? GoldPerMin { get; set; }

    [Column("xp_per_min")]
    [JsonPropertyName("xp_per_min")]
    public int? XpPerMin { get; set; }

    [Column("support_gold_spent")]
    [JsonPropertyName("support_gold_spent")]
    public int? SupportGoldSpent { get; set; }

    [Column("observer_wards_placed")]
    [JsonPropertyName("observer_wards_placed")]
    public int? ObserverWardsPlaced { get; set; }

    [Column("sentry_wards_placed")]
    [JsonPropertyName("sentry_wards_placed")]
    public int? SentyWardsPlaced { get; set; }

    [Column("dewards")]
    [JsonPropertyName("dewards")]
    public int? Dewards { get; set; }

    [Column("camps_stacked")]
    [JsonPropertyName("camps_stacked")]
    public int? CampsStacked { get; set; }

    [Column("stun_duration")]
    [JsonPropertyName("stun_duration")]
    public float? StunDuration { get; set; }

    [Column("level")]
    [JsonPropertyName("level")]
    public int? Level { get; set; }

    [Column("net_worth")]
    [JsonPropertyName("net_worth")]
    public long? Networth { get; set; }

    [Column("hero_damage")]
    [JsonPropertyName("hero_damage")]
    public int? HeroDamage { get; set; }

    [Column("tower_damage")]
    [JsonPropertyName("tower_damage")]
    public int? TowerDamage { get; set; }

    [Column("hero_healing")]
    [JsonPropertyName("hero_healing")]
    public int? HeroHealing { get; set; }

    [Column("gold")]
    [JsonPropertyName("gold")]
    public int? Gold { get; set; }

    // Match Metadata

    [Column("fight_score")]
    public float? FightScore { get; set; }

    [Column("farm_score")]
    public float? FarmScore { get; set; }

    [Column("support_score")]
    public float? SupportScore { get; set; }

    [Column("push_score")]
    public float? PushScore { get; set; }

    [Column("hero_xp")]
    public uint? HeroXp { get; set; }

    [Column("rampages")]
    public uint? Rampages { get; set; }

    [Column("triple_kills")]
    public uint? TripleKills { get; set; }

    [Column("aegis_snatched")]
    public uint? AegisSnatched { get; set; }

    [Column("rapiers_purchased")]
    public uint? RapiersPurchased { get; set; }

    [Column("couriers_killed")]
    public uint? CouriersKilled { get; set; }
}