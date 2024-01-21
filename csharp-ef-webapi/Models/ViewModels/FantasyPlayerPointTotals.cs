namespace csharp_ef_webapi.Models;

// This is a view model and isn't saved in the db (in case we change the scoring)
public class FantasyPlayerPointTotals
{
    public FantasyPlayer FantasyPlayer { get; set; } = new FantasyPlayer();
    public int TotalMatches { get; set; }
    public int TotalKills { get; set; }
    public decimal TotalKillsPoints { get; set; }
    public int TotalDeaths { get; set; }
    public decimal TotalDeathsPoints { get; set; }
    public int TotalAssists { get; set; }
    public decimal TotalAssistsPoints { get; set; }
    public int TotalLastHits { get; set; }
    public decimal TotalLastHitsPoints { get; set; }
    public double AvgGoldPerMin { get; set; }
    public decimal TotalGoldPerMinPoints { get; set; }
    public double AvgXpPerMin { get; set; }
    public decimal TotalXpPerMinPoints { get; set; }
    public decimal TotalMatchFantasyPoints { get; set; }
}