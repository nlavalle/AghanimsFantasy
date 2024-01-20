namespace csharp_ef_webapi.Models;

using System.ComponentModel.DataAnnotations.Schema;

[Table("player_match_details")]
public class PlayerMatchDetails
{
    [Column("match_id")]
    public long MatchId { get; set; }

    [Column("account_id")]
    public long AccountId { get; set; }

    [Column("player_slot")]
    public int PlayerSlot { get; set; }
    
    [Column("hero_id")]
    public int HeroId { get; set; }
    
    [Column("item_0")]
    public int? Item0 { get; set; }
    
    [Column("item_1")]
    public int? Item1 { get; set; }
    
    [Column("item_2")]
    public int? Item2 { get; set; }
    
    [Column("item_3")]
    public int? Item3 { get; set; }
    
    [Column("item_4")]
    public int? Item4 { get; set; }
    
    [Column("item_5")]
    public int? Item5 { get; set; }
    
    [Column("backpack_0")]
    public int? Backpack0 { get; set; }
    
    [Column("backpack_1")]
    public int? Backpack1 { get; set; }
    
    [Column("backpack_2")]
    public int? Backpack2 { get; set; }
    
    [Column("item_neutral")]
    public int? ItemNeutral { get; set; }
    
    [Column("kills")]
    public int? Kills { get; set; }
    
    [Column("deaths")]
    public int? Deaths { get; set; }
    
    [Column("assists")]
    public int? Assists { get; set; }
    
    [Column("leaver_status")]
    public int? LeaverStatus { get; set; }
    
    [Column("last_hits")]
    public int? LastHits { get; set; }
    
    [Column("denies")]
    public int? Denies { get; set; }
    
    [Column("gold_per_min")]
    public int? GoldPerMin { get; set; }
    
    [Column("xp_per_min")]
    public int? XpPerMin { get; set; }
    
    [Column("level")]
    public int? Level { get; set; }
    
    [Column("hero_damage")]
    public int? HeroDamage { get; set; }
    
    [Column("tower_damage")]
    public int? TowerDamage { get; set; }
    
    [Column("hero_healing")]
    public int? HeroHealing { get; set; }
    
    [Column("gold")]
    public int? Gold { get; set; }
    
    [Column("gold_spent")]
    public int? GoldSpent { get; set; }
    
    [Column("scaled_hero_damage")]
    public int? ScaledHeroDamage { get; set; }
    
    [Column("scaled_tower_damage")]
    public int? ScaledTowerDamage { get; set; }
    
    [Column("scaled_hero_healing")]
    public int? ScaledHeroHealing { get; set; }
    
    [Column("aghanims_scepter")]
    public int? AghanimsScepter { get; set; }
    
    [Column("aghanims_shard")]
    public int? AghanimsShard { get; set; }
    
    [Column("moonshard")]
    public int? Moonshard { get; set; }
    
    [Column("net_worth")]
    public long? Networth { get; set; }
    
    [Column("team_number")]
    public int? TeamNumber { get; set; }
    
    [Column("team_slot")]
    public int? TeamSlot { get; set; }

}