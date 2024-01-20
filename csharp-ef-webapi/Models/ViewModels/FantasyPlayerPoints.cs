namespace csharp_ef_webapi.Models;

// This is a view model and isn't saved in the db (in case we change the scoring)
public class FantasyPlayerPoints
{
    private const decimal KillsValue = 0.3M;
    private const decimal DeathsValue = -0.3M;
    private const decimal AssistsValue = 0.15M;
    private const decimal LastHitsValue = 0.003M;
    private const decimal GoldPerMinValue = 0.002M;
    private const decimal XpPerMinValue = 0.002M;
    public FantasyDraft FantasyDraft {get;set; } = new FantasyDraft();
    public FantasyPlayer FantasyPlayer { get; set; } = new FantasyPlayer();
    public MatchDetailsPlayer Match { get; set; } = new MatchDetailsPlayer();
    public int Kills
    {
        get
        {
            return Match.Kills ?? 0;
        }
    }
    public decimal KillsPoints
    {
        get
        {
            return Kills * KillsValue;
        }
    }

    public int Deaths
    {
        get
        {
            return Match.Deaths ?? 0;
        }
    }
    public decimal DeathsPoints
    {
        get
        {
            return Deaths * DeathsValue;
        }
    }

    public int Assists
    {
        get
        {
            return Match.Assists ?? 0;
        }
    }
    public decimal AssistsPoints
    {
        get
        {
            return Assists * AssistsValue;
        }
    }

    public int LastHits
    {
        get
        {
            return Match.LastHits ?? 0;
        }
    }
    public decimal LastHitsPoints
    {
        get
        {
            return LastHits * LastHitsValue;
        }
    }

    public int GoldPerMin
    {
        get
        {
            return Match.GoldPerMin ?? 0;
        }
    }
    public decimal GoldPerMinPoints
    {
        get
        {
            return GoldPerMin * GoldPerMinValue;
        }
    }

    public int XpPerMin
    {
        get
        {
            return Match.XpPerMin ?? 0;
        }
    }
    public decimal XpPerMinPoints
    {
        get
        {
            return XpPerMin * XpPerMinValue;
        }
    }

    public decimal TotalMatchFantasyPoints
    {
        get
        {
            return KillsPoints + DeathsPoints + AssistsPoints + LastHitsPoints + GoldPerMinPoints + XpPerMinPoints;
        }
    }
}