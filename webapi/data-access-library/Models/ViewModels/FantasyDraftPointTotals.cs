namespace DataAccessLibrary.Models;

using DataAccessLibrary.Models.Fantasy;

// This is a view model and isn't saved in the db (in case we change the scoring)
public class FantasyDraftPointTotals
{
    public required FantasyDraft FantasyDraft { get; set; }
    public string UserName { get; set; } = "unknown";
    public List<FantasyPlayerPointTotals> FantasyPlayerPoints { get; set; } = new List<FantasyPlayerPointTotals>();
    public decimal DraftPickOnePoints
    {
        get
        {
            return FantasyPlayerPoints
                .FirstOrDefault(fpp =>
                    fpp.FantasyPlayer.Id == FantasyDraft.DraftPickPlayers
                        .FirstOrDefault(fdp => fdp.DraftOrder == 1)?.FantasyPlayer!.Id
                )?.TotalMatchFantasyPoints ?? 0;
        }
    }
    public decimal DraftPickTwoPoints
    {
        get
        {
            return FantasyPlayerPoints
                .FirstOrDefault(fpp =>
                    fpp.FantasyPlayer.Id == FantasyDraft.DraftPickPlayers
                        .FirstOrDefault(fdp => fdp.DraftOrder == 2)?.FantasyPlayer!.Id
                )?.TotalMatchFantasyPoints ?? 0;
        }
    }
    public decimal DraftPickThreePoints
    {
        get
        {
            return FantasyPlayerPoints
                .FirstOrDefault(fpp =>
                    fpp.FantasyPlayer.Id == FantasyDraft.DraftPickPlayers
                        .FirstOrDefault(fdp => fdp.DraftOrder == 3)?.FantasyPlayer!.Id
                )?.TotalMatchFantasyPoints ?? 0;
        }
    }
    public decimal DraftPickFourPoints
    {
        get
        {
            return FantasyPlayerPoints
                .FirstOrDefault(fpp =>
                    fpp.FantasyPlayer.Id == FantasyDraft.DraftPickPlayers
                        .FirstOrDefault(fdp => fdp.DraftOrder == 4)?.FantasyPlayer!.Id
                )?.TotalMatchFantasyPoints ?? 0;
        }
    }
    public decimal DraftPickFivePoints
    {
        get
        {
            return FantasyPlayerPoints
                .FirstOrDefault(fpp =>
                    fpp.FantasyPlayer.Id == FantasyDraft.DraftPickPlayers
                        .FirstOrDefault(fdp => fdp.DraftOrder == 5)?.FantasyPlayer!.Id
                )?.TotalMatchFantasyPoints ?? 0;
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