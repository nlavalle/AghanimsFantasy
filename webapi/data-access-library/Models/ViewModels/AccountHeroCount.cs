using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLibrary.Models.ViewModels;

// This is a view model and isn't saved in the db (in case we change the scoring)
public class AccountHeroCount
{
    [Column("account_id")]
    public long AccountId { get; set; }

    [Column("hero_id")]
    public int HeroId { get; set; }

    [Column("count")]
    public int Count { get; set; }

    [Column("wins")]
    public int Wins { get; set; }

    [Column("losses")]
    public int Losses { get; set; }
}