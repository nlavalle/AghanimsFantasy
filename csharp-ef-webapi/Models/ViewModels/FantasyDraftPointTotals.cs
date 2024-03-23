namespace csharp_ef_webapi.Models;

// This is a view model and isn't saved in the db (in case we change the scoring)
public class FantasyDraftPointTotals
{
    public FantasyDraft FantasyDraft { get; set; } = new FantasyDraft();
    public bool IsTeam { get; set; }
    public long? TeamId { get; set; }
    public string DiscordName { get; set; } = "";
    public List<FantasyPlayerPointTotals> FantasyPlayerPoints { get; set; } = new List<FantasyPlayerPointTotals>();
    public decimal DraftPickOnePoints
    {
        get
        {
            return FantasyPlayerPoints
                .Where(fpp => 
                    fpp.FantasyPlayer.Id == FantasyDraft.DraftPickPlayers
                        .Where(fdp => fdp.DraftOrder == 1)
                        .FirstOrDefault()?.FantasyPlayerId
                )
                .FirstOrDefault()?.TotalMatchFantasyPoints ?? 0;
        }
    }
    public decimal DraftPickTwoPoints
    {
        get
        {
            return FantasyPlayerPoints
                .Where(fpp => 
                    fpp.FantasyPlayer.Id == FantasyDraft.DraftPickPlayers
                        .Where(fdp => fdp.DraftOrder == 2)
                        .FirstOrDefault()?.FantasyPlayerId
                )
                .FirstOrDefault()?.TotalMatchFantasyPoints ?? 0;
        }
    }
    public decimal DraftPickThreePoints
    {
        get
        {
            return FantasyPlayerPoints
                .Where(fpp => 
                    fpp.FantasyPlayer.Id == FantasyDraft.DraftPickPlayers
                        .Where(fdp => fdp.DraftOrder == 3)
                        .FirstOrDefault()?.FantasyPlayerId
                )
                .FirstOrDefault()?.TotalMatchFantasyPoints ?? 0;
        }
    }
    public decimal DraftPickFourPoints
    {
        get
        {
            return FantasyPlayerPoints
                .Where(fpp => 
                    fpp.FantasyPlayer.Id == FantasyDraft.DraftPickPlayers
                        .Where(fdp => fdp.DraftOrder == 4)
                        .FirstOrDefault()?.FantasyPlayerId
                )
                .FirstOrDefault()?.TotalMatchFantasyPoints ?? 0;
        }
    }
    public decimal DraftPickFivePoints
    {
        get
        {
            return FantasyPlayerPoints
                .Where(fpp => 
                    fpp.FantasyPlayer.Id == FantasyDraft.DraftPickPlayers
                        .Where(fdp => fdp.DraftOrder == 5)
                        .FirstOrDefault()?.FantasyPlayerId
                )
                .FirstOrDefault()?.TotalMatchFantasyPoints ?? 0;
        }
    }
    public decimal DraftTotalFantasyPoints
    {
        get
        {
            return DraftPickOnePoints + DraftPickTwoPoints + DraftPickThreePoints + DraftPickFourPoints + DraftPickFivePoints;
        }
    }
}