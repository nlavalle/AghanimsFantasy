namespace DataAccessLibrary.Models.ProMetadata;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

[Table("dota_accounts")]
public class Account
{
    [Key]
    [Column("id")]
    [JsonIgnore]
    public long Id { get; set; }

    [Column("name")]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [Column("steam_profile_picture")]
    [JsonPropertyName("steamProfilePicture")]
    public string? SteamProfilePicture { get; set; }
}