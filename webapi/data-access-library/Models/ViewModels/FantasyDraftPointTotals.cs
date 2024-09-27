namespace DataAccessLibrary.Models;

using DataAccessLibrary.Models.Fantasy;

// This is a view model and isn't saved in the db (in case we change the scoring)
public class FantasyDraftPointTotals
{
    public required FantasyDraft FantasyDraft { get; set; }
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
                        .FirstOrDefault()?.FantasyPlayer!.Id
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
                        .FirstOrDefault()?.FantasyPlayer!.Id
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
                        .FirstOrDefault()?.FantasyPlayer!.Id
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
                        .FirstOrDefault()?.FantasyPlayer!.Id
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
                        .FirstOrDefault()?.FantasyPlayer!.Id
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