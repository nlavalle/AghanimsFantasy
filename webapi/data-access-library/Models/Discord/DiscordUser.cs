namespace DataAccessLibrary.Models.Discord;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("discord_users")]
public class DiscordUser
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("username")]
    public string Username { get; set; } = null!;

    [Column("is_admin")]
    public bool IsAdmin { get; set; } = false;
}