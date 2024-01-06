namespace csharp_ef_webapi.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

[Table("dota_accounts")]
public class Account
{
    [Key]
    [Column("id")]
    [JsonIgnore]
    public long Id { get; set; }

    [Column("name")]
    [JsonProperty("name")]
    public string? Name { get; set; }
}