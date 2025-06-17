namespace DataAccessLibrary.Models.Fantasy;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DataAccessLibrary.Data.Identity;

[Table("fantasy_prizes")]
public class FantasyPrize
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [ForeignKey("User")]
    [Column("user_id")]
    [JsonPropertyName("user_id")]
    public required string UserId { get; set; }

    [JsonIgnore]
    public required AghanimsFantasyUser User { get; set; }

    [Column("prize_type")]
    [JsonPropertyName("prize_type")]
    public FantasyPrizeOption FantasyPrizeOption { get; set; }

    [Column("prize_timestamp")]
    [JsonPropertyName("prize_timestamp")]
    public long PrizeTimestamp { get; set; }
}

public enum FantasyPrizeOption
{
    KILL_STREAK_FLAMES,
    STICKER,
    LEADERBOARD_BADGE,

}