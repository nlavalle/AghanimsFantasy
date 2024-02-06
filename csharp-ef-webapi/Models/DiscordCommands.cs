// These entities don't belong in the database, purely for responses
using System.ComponentModel.DataAnnotations.Schema;

namespace csharp_ef_webapi.Models;

[NotMapped]
public class Feederboard
{
    public int Rank { get; set; }
    public int Deaths { get; set; }
    public string DiscordName { get; set; } = "";

}