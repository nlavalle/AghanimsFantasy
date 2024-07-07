namespace DataAccessLibrary.Models.WebApi;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

// https://api.steampowered.com/IDOTA2Match_570/GetMatchDetails/v1?
[Table("dota_match_details_players")]
public class MatchDetailsPlayer
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
    public long AccountId { get; set; }

    [Column("player_slot")]
    [JsonProperty("player_slot")]
    public int PlayerSlot { get; set; }

    [Column("team_number")]
    [JsonProperty("team_number")]
    public int? TeamNumber { get; set; }

    [Column("team_slot")]
    [JsonProperty("team_slot")]
    public int? TeamSlot { get; set; }

    [Column("hero_id")]
    [JsonProperty("hero_id")]
    public int HeroId { get; set; }

    [Column("item_0")]
    [JsonProperty("item_0")]
    public int? Item0 { get; set; }

    [Column("item_1")]
    [JsonProperty("item_1")]
    public int? Item1 { get; set; }

    [Column("item_2")]
    [JsonProperty("item_2")]
    public int? Item2 { get; set; }

    [Column("item_3")]
    [JsonProperty("item_3")]
    public int? Item3 { get; set; }

    [Column("item_4")]
    [JsonProperty("item_4")]
    public int? Item4 { get; set; }

    [Column("item_5")]
    [JsonProperty("item_5")]
    public int? Item5 { get; set; }

    [Column("backpack_0")]
    [JsonProperty("backpack_0")]
    public int? Backpack0 { get; set; }

    [Column("backpack_1")]
    [JsonProperty("backpack_1")]
    public int? Backpack1 { get; set; }

    [Column("backpack_2")]
    [JsonProperty("backpack_2")]
    public int? Backpack2 { get; set; }

    [Column("item_neutral")]
    [JsonProperty("item_neutral")]
    public int? ItemNeutral { get; set; }

    [Column("kills")]
    [JsonProperty("kills")]
    public int? Kills { get; set; }

    [Column("deaths")]
    [JsonProperty("deaths")]
    public int? Deaths { get; set; }

    [Column("assists")]
    [JsonProperty("assists")]
    public int? Assists { get; set; }

    [Column("leaver_status")]
    [JsonProperty("leaver_status")]
    public int? LeaverStatus { get; set; }

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

    [Column("level")]
    [JsonProperty("level")]
    public int? Level { get; set; }

    [Column("net_worth")]
    [JsonProperty("net_worth")]
    public long? Networth { get; set; }

    [Column("aghanims_scepter")]
    [JsonProperty("aghanims_scepter")]
    public int? AghanimsScepter { get; set; }

    [Column("aghanims_shard")]
    [JsonProperty("aghanims_shard")]
    public int? AghanimsShard { get; set; }

    [Column("moonshard")]
    [JsonProperty("moonshard")]
    public int? Moonshard { get; set; }

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

    [Column("gold_spent")]
    [JsonProperty("gold_spent")]
    public int? GoldSpent { get; set; }

    [Column("scaled_hero_damage")]
    [JsonProperty("scaled_hero_damage")]
    public int? ScaledHeroDamage { get; set; }

    [Column("scaled_tower_damage")]
    [JsonProperty("scaled_tower_damage")]
    public int? ScaledTowerDamage { get; set; }

    [Column("scaled_hero_healing")]
    [JsonProperty("scaled_hero_healing")]
    public int? ScaledHeroHealing { get; set; }

    [JsonProperty("ability_upgrades")]
    public List<MatchDetailsPlayersAbilityUpgrade> AbilityUpgrades { get; set; } = new List<MatchDetailsPlayersAbilityUpgrade>();
}