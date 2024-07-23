namespace DataAccessLibrary.Models.ProMetadata;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

[Table("dota_heroes")]
public class Hero
{
    [Key]
    [Column("id")]
    [JsonProperty("id")]
    public long Id { get; set; }

    [Column("name")]
    [JsonProperty("name")]
    public string? Name { get; set; }

}