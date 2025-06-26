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
    public decimal Cost { get; set; }
}
