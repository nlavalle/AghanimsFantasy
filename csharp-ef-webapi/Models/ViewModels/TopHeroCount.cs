namespace csharp_ef_webapi.Models;

// This is a view model and isn't saved in the db (in case we change the scoring)
public class TopHeroCount
{
    public required Hero Hero { get; set; }
    public int Count { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
}