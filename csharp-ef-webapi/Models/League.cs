namespace csharp_ef_webapi.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("dota_leagues")]
public class League
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("league_name")]
    public string? Name { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }
    public List<MatchHistory> MatchHistories { get; set; } = new List<MatchHistory>();
    public List<MatchDetail> MatchDetails { get; set; } = new List<MatchDetail>();
    public List<FantasyLeague> FantasyLeagues { get; set; } = new List<FantasyLeague>();
}