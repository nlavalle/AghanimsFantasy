namespace DataAccessLibrary.Models.Discord;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("discord_outbox")]
public class DiscordOutbox
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("event_object")]
    public required string EventObject { get; set; }

    [Column("event_type")]
    public required string EventType { get; set; }

    [Column("object_key")]
    public required string ObjectKey { get; set; }

    [Column("message_sent_timestamp")]
    public long MessageSentTimestamp { get; set; }
}