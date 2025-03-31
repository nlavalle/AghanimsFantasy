namespace DataAccessLibrary.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.ProMetadata;

// This is a view model and isn't saved in the db (in case we change the scoring)
public class FantasyPlayerBudgetProbabilityTable
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    public required FantasyLeague FantasyLeague { get; set; }

    public required Account Account { get; set; }

    [Column("quintile")]
    public int Quintile { get; set; }

    [Column("probability")]
    public decimal Probability { get; set; }

    [Column("cumulative_probability")]
    public decimal CumulativeProbability { get; set; }

    public decimal Winnings
    {
        get
        {
            return 300 - Quintile * 60;
        }
    }

    public decimal EstimatedCost
    {
        get
        {
            return Winnings * Probability;
        }
    }
}
