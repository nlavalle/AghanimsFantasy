namespace DataAccessLibrary.Models.Fantasy;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DataAccessLibrary.Models.Discord;

[Table("fantasy_ledger")]
public class FantasyLedger
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [ForeignKey("DiscordUser")]
    [Column("discord_id")]
    [JsonPropertyName("discord_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public required long DiscordId { get; set; }

    [JsonIgnore]
    public DiscordUser? DiscordUser { get; set; }

    [Column("source_type")]
    public string SourceType { get; set; } = "unknown";

    [Column("source_id")]
    public int SourceId { get; set; }

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("ledger_recorded_time")]
    public long LedgerRecordedTime { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
}