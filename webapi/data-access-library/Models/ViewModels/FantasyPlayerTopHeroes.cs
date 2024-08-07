namespace DataAccessLibrary.Models;

using DataAccessLibrary.Models.Fantasy;

// This is a view model and isn't saved in the db (in case we change the scoring)
public class FantasyPlayerTopHeroes
{
    public long FantasyPlayerId { get; set; }
    public FantasyPlayer FantasyPlayer { get; set; } = null!;
    public TopHeroCount[] TopHeroes { get; set; } = [];

}