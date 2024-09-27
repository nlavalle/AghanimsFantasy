namespace DataAccessLibrary.Models.ProMetadata;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

[Table("dota_heroes")]
public class Hero
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [Column("name")]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

}