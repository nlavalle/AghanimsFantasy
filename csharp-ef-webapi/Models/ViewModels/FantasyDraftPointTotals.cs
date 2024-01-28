namespace csharp_ef_webapi.Models;

// This is a view model and isn't saved in the db (in case we change the scoring)
public class FantasyDraftPointTotals
{
    public FantasyDraft FantasyDraft { get; set; } = new FantasyDraft();
    public bool IsTeam { get; set; }
    public long? TeamId { get; set; }
    public string DiscordName { get; set; } = "";
    public decimal DraftPickOnePoints { get; set; }
    public decimal DraftPickTwoPoints { get; set; }
    public decimal DraftPickThreePoints { get; set; }
    public decimal DraftPickFourPoints { get; set; }
    public decimal DraftPickFivePoints { get; set; }
    public decimal DraftTotalFantasyPoints
    {
        get
        {
            return DraftPickOnePoints + DraftPickTwoPoints + DraftPickThreePoints + DraftPickFourPoints + DraftPickFivePoints;
        }
    }
}