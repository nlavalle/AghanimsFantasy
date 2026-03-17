namespace DataAccessLibrary.Models;

// This is a view model and isn't saved in the db (in case we change the scoring)
public class LeagueLeaderboard
{
    public required int LeagueId { get; set; }
    public List<LeagueLeaderboardRound> Rounds { get; set; } = [];
    public LeaderboardStats? AllRoundsStats { get; set; }
}

public class LeagueLeaderboardRound
{
    public required int FantasyLeagueId { get; set; }
    public List<FantasyDraftPointTotals> FantasyDrafts { get; set; } = [];
    public LeaderboardStats? AllRoundsStats { get; set; }
}

public class LeaderboardStats
{
    public int TotalDrafts { get; set; }
    public decimal DrafterPercentile { get; set; }
}