namespace DataAccessLibrary.Models.ProMetadata;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

[Table("dota_teams")]
public class Team
{
    [Key]
    [Column("id")]
    [JsonIgnore]
    public long Id { get; set; }

    [Column("name")]
    [JsonProperty("name")]
    public string? Name { get; set; }

    [Column("tag")]
    [JsonProperty("tag")]
    public string? Tag { get; set; }

    [Column("abbreviation")]
    [JsonProperty("abbreviation")]
    public string? Abbreviation { get; set; }

    [Column("time_created")]
    [JsonProperty("time_created")]
    public long TimeCreated { get; set; }

    [Column("logo")]
    [JsonProperty("logo")]
    public string? Logo { get; set; }

    [Column("logo_sponsor")]
    [JsonProperty("logo_sponsor")]
    public string? LogoSponsor { get; set; }

    [Column("country_code")]
    [JsonProperty("country_code")]
    public string? CountryCode { get; set; }

    [Column("url")]
    [JsonProperty("url")]
    public string? Url { get; set; }

    [Column("games_played")]
    [JsonProperty("games_played")]
    public int GamesPlayed { get; set; }

    [Column("player_0_account_id")]
    [JsonProperty("player_0_account_id")]
    public long Player0AccountId { get; set; }

    [Column("player_1_account_id")]
    [JsonProperty("player_1_account_id")]
    public long Player1AccountId { get; set; }

    [Column("player_2_account_id")]
    [JsonProperty("player_2_account_id")]
    public long Player2AccountId { get; set; }

    [Column("player_3_account_id")]
    [JsonProperty("player_3_account_id")]
    public long Player3AccountId { get; set; }

    [Column("player_4_account_id")]
    [JsonProperty("player_4_account_id")]
    public long Player4AccountId { get; set; }

    [Column("player_5_account_id")]
    [JsonProperty("player_5_account_id")]
    public long Player5AccountId { get; set; }

    [Column("admin_account_id")]
    [JsonProperty("admin_account_id")]
    public long AdminAccountId { get; set; }
}