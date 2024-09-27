namespace DataAccessLibrary.Models.WebApi;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

// https://api.steampowered.com/IDOTA2Match_570/GetMatchDetails/v1?
[Table("dota_match_details_players")]
public class MatchDetailsPlayer
{
    [Key]
    [Column("id")]
    [JsonIgnore]
    public int Id { get; set; }

    [ForeignKey("Match")]
    [Column("match_id")]
    public required long MatchId { get; set; }

    [JsonIgnore]
    public MatchDetail? Match { get; set; }

    [Column("account_id")]
    [JsonPropertyName("account_id")]
    public long AccountId { get; set; }

    [Column("player_slot")]
    [JsonPropertyName("player_slot")]
    public int PlayerSlot { get; set; }

    [Column("team_number")]
    [JsonPropertyName("team_number")]
    public int? TeamNumber { get; set; }

    [Column("team_slot")]
    [JsonPropertyName("team_slot")]
    public int? TeamSlot { get; set; }

    [Column("hero_id")]
    [JsonPropertyName("hero_id")]
    public int HeroId { get; set; }

    [Column("item_0")]
    [JsonPropertyName("item_0")]
    public int? Item0 { get; set; }

    [Column("item_1")]
    [JsonPropertyName("item_1")]
    public int? Item1 { get; set; }

    [Column("item_2")]
    [JsonPropertyName("item_2")]
    public int? Item2 { get; set; }

    [Column("item_3")]
    [JsonPropertyName("item_3")]
    public int? Item3 { get; set; }

    [Column("item_4")]
    [JsonPropertyName("item_4")]
    public int? Item4 { get; set; }

    [Column("item_5")]
    [JsonPropertyName("item_5")]
    public int? Item5 { get; set; }

    [Column("backpack_0")]
    [JsonPropertyName("backpack_0")]
    public int? Backpack0 { get; set; }

    [Column("backpack_1")]
    [JsonPropertyName("backpack_1")]
    public int? Backpack1 { get; set; }

    [Column("backpack_2")]
    [JsonPropertyName("backpack_2")]
    public int? Backpack2 { get; set; }

    [Column("item_neutral")]
    [JsonPropertyName("item_neutral")]
    public int? ItemNeutral { get; set; }

    [Column("kills")]
    [JsonPropertyName("kills")]
    public int? Kills { get; set; }

    [Column("deaths")]
    [JsonPropertyName("deaths")]
    public int? Deaths { get; set; }

    [Column("assists")]
    [JsonPropertyName("assists")]
    public int? Assists { get; set; }

    [Column("leaver_status")]
    [JsonPropertyName("leaver_status")]
    public int? LeaverStatus { get; set; }

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

    [Column("level")]
    [JsonPropertyName("level")]
    public int? Level { get; set; }

    [Column("net_worth")]
    [JsonPropertyName("net_worth")]
    public long? Networth { get; set; }

    [Column("aghanims_scepter")]
    [JsonPropertyName("aghanims_scepter")]
    public int? AghanimsScepter { get; set; }

    [Column("aghanims_shard")]
    [JsonPropertyName("aghanims_shard")]
    public int? AghanimsShard { get; set; }

    [Column("moonshard")]
    [JsonPropertyName("moonshard")]
    public int? Moonshard { get; set; }

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

    [Column("gold_spent")]
    [JsonPropertyName("gold_spent")]
    public int? GoldSpent { get; set; }

    [Column("scaled_hero_damage")]
    [JsonPropertyName("scaled_hero_damage")]
    public int? ScaledHeroDamage { get; set; }

    [Column("scaled_tower_damage")]
    [JsonPropertyName("scaled_tower_damage")]
    public int? ScaledTowerDamage { get; set; }

    [Column("scaled_hero_healing")]
    [JsonPropertyName("scaled_hero_healing")]
    public int? ScaledHeroHealing { get; set; }

    [JsonPropertyName("ability_upgrades")]
    public List<MatchDetailsPlayersAbilityUpgrade> AbilityUpgrades { get; set; } = new List<MatchDetailsPlayersAbilityUpgrade>();
}