namespace DataAccessLibrary.Models.ProMetadata;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

[Table("dota_teams")]
public class Team
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [Column("name")]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [Column("tag")]
    [JsonPropertyName("tag")]
    public string? Tag { get; set; }

    [Column("abbreviation")]
    [JsonPropertyName("abbreviation")]
    public string? Abbreviation { get; set; }

    [Column("time_created")]
    [JsonPropertyName("time_created")]
    public long TimeCreated { get; set; }

    [Column("logo")]
    [JsonPropertyName("logo")]
    public decimal? Logo { get; set; }

    [Column("logo_sponsor")]
    [JsonPropertyName("logo_sponsor")]
    public decimal? LogoSponsor { get; set; }

    [Column("country_code")]
    [JsonPropertyName("country_code")]
    public string? CountryCode { get; set; }

    [Column("url")]
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [Column("games_played")]
    [JsonPropertyName("games_played")]
    public int GamesPlayed { get; set; }

    [Column("player_0_account_id")]
    [JsonPropertyName("player_0_account_id")]
    public long Player0AccountId { get; set; }

    [Column("player_1_account_id")]
    [JsonPropertyName("player_1_account_id")]
    public long Player1AccountId { get; set; }

    [Column("player_2_account_id")]
    [JsonPropertyName("player_2_account_id")]
    public long Player2AccountId { get; set; }

    [Column("player_3_account_id")]
    [JsonPropertyName("player_3_account_id")]
    public long Player3AccountId { get; set; }

    [Column("player_4_account_id")]
    [JsonPropertyName("player_4_account_id")]
    public long Player4AccountId { get; set; }

    [Column("player_5_account_id")]
    [JsonPropertyName("player_5_account_id")]
    public long Player5AccountId { get; set; }

    [Column("admin_account_id")]
    [JsonPropertyName("admin_account_id")]
    public long AdminAccountId { get; set; }
}