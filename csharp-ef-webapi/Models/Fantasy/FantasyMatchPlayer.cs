namespace csharp_ef_webapi.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using csharp_ef_webapi.Models.ProMetadata;
using Newtonsoft.Json;

[Table("fantasy_match_player")]
public class FantasyMatchPlayer
{
    [Key]
    [Column("id")]
    [JsonIgnore]
    public int Id { get; set; }

    [Column("match_id")]
    [JsonIgnore]
    public long MatchId { get; set; }

    [Column("account_id")]
    [JsonProperty("account_id")]
    public Account? Account { get; set; }
    public string AccountFormatted
    {
        get
        {
            return Account?.Name ?? string.Empty;
        }
    }

    [Column("team_id")]
    [JsonProperty("team_id")]
    public Team? Team { get; set; }
    public string TeamFormatted
    {
        get
        {
            return Team?.Name ?? string.Empty;
        }
    }

    [Column("dota_team_side")]
    [JsonProperty("dota_team_side")]
    public bool DotaTeamSide { get; set; }
    public string DotaTeamSideFormatted
    {
        get
        {
            return DotaTeamSide ? "dire" : "radiant";
        }
    }

    [Column("player_slot")]
    [JsonProperty("player_slot")]
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

    [Column("hero_id")]
    [JsonProperty("hero_id")]
    public Hero? Hero { get; set; }
    public string HeroFormatted
    {
        get
        {
            return Hero?.Name?.ToString() ?? string.Empty;
        }
    }

    [Column("kills")]
    [JsonProperty("kills")]
    public int? Kills { get; set; }

    [Column("deaths")]
    [JsonProperty("deaths")]
    public int? Deaths { get; set; }

    [Column("assists")]
    [JsonProperty("assists")]
    public int? Assists { get; set; }

    [Column("last_hits")]
    [JsonProperty("last_hits")]
    public int? LastHits { get; set; }

    [Column("denies")]
    [JsonProperty("denies")]
    public int? Denies { get; set; }

    [Column("gold_per_min")]
    [JsonProperty("gold_per_min")]
    public int? GoldPerMin { get; set; }

    [Column("xp_per_min")]
    [JsonProperty("xp_per_min")]
    public int? XpPerMin { get; set; }

    [Column("support_gold_spent")]
    [JsonProperty("support_gold_spent")]
    public int? SupportGoldSpent { get; set; }

    [Column("observer_wards_placed")]
    [JsonProperty("observer_wards_placed")]
    public int? ObserverWardsPlaced { get; set; }

    [Column("sentry_wards_placed")]
    [JsonProperty("sentry_wards_placed")]
    public int? SentyWardsPlaced { get; set; }

    [Column("dewards")]
    [JsonProperty("dewards")]
    public int? Dewards { get; set; }

    [Column("camps_stacked")]
    [JsonProperty("camps_stacked")]
    public int? CampsStacked { get; set; }

    [Column("stun_duration")]
    [JsonProperty("stun_duration")]
    public float? StunDuration { get; set; }

    [Column("level")]
    [JsonProperty("level")]
    public int? Level { get; set; }

    [Column("net_worth")]
    [JsonProperty("net_worth")]
    public long? Networth { get; set; }

    [Column("hero_damage")]
    [JsonProperty("hero_damage")]
    public int? HeroDamage { get; set; }

    [Column("tower_damage")]
    [JsonProperty("tower_damage")]
    public int? TowerDamage { get; set; }

    [Column("hero_healing")]
    [JsonProperty("hero_healing")]
    public int? HeroHealing { get; set; }

    [Column("gold")]
    [JsonProperty("gold")]
    public int? Gold { get; set; }
}