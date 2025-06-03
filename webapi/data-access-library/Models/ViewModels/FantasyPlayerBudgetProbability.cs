namespace DataAccessLibrary.Models;

using System.ComponentModel.DataAnnotations.Schema;
using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.ProMetadata;

// This is a view model and isn't saved in the db (in case we change the scoring)
public class FantasyPlayerBudgetProbability
{
    [Column("fantasy_league_id")]
    public int FantasyLeagueId { get; set; }
    public FantasyLeague FantasyLeague { get; set; } = null!;

    [Column("account_id")]
    public long AccountId { get; set; }
    public Account Account { get; set; } = null!;

    [Column("cost")]
    public decimal Cost { get; set; }
}
