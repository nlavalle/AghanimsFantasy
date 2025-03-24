namespace DataAccessLibrary.Models;

// This is a view model and isn't saved in the db (in case we change the scoring)
public class FantasyPlayerTopHeroes
{
    public long FantasyPlayerId { get; set; }
    public TopHeroCount[] TopHeroes { get; set; } = [];

}