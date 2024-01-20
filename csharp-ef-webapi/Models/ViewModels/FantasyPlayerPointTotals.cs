namespace csharp_ef_webapi.Models;

// This is a view model and isn't saved in the db (in case we change the scoring)
public class FantasyPlayerPointTotals
{
    public FantasyPlayer FantasyPlayer { get; set; } = new FantasyPlayer();
    public decimal TotalMatchFantasyPoints { get; set; }
}