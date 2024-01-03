namespace csharp_ef_webapi.Models;

using System.ComponentModel.DataAnnotations.Schema;

[Table("dota_leagues")]
public class League
{
    [Column("league_id")]
    public int id { get; set; }
    [Column("league_name")]
    public string? name { get; set; }
    [Column("is_active")]
    public bool isActive { get; set; }
}