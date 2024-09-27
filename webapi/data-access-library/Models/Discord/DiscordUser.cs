namespace DataAccessLibrary.Models.Discord;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

[Table("discord_users")]
public class DiscordUser
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [Column("username")]
    [JsonPropertyName("username")]
    public string Username { get; set; } = null!;

    [Column("is_admin")]
    [JsonIgnore]
    public bool IsAdmin { get; set; } = false;
}