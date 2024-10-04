namespace DataAccessLibrary.Models;

// This is a view model and isn't saved in the db
public class LeaderboardStats
{
    public int TotalDrafts { get; set; }
    public decimal DrafterPercentile { get; set; }
}