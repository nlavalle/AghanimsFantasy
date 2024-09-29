using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Discord;
using DataAccessLibrary.Models.Fantasy;

namespace csharp_ef_webapi.Services;
public class FantasyService
{
    private readonly ILogger<FantasyService> _logger;
    private readonly IFantasyLeagueRepository _fantasyLeagueRepository;
    private readonly IFantasyLeagueWeightRepository _fantasyLeagueWeightRepository;
    private readonly IFantasyDraftRepository _fantasyDraftRepository;
    private readonly IFantasyPlayerRepository _fantasyPlayerRepository;
    private readonly IFantasyRepository _fantasyRepository;

    public FantasyService(
        ILogger<FantasyService> logger,
        IFantasyLeagueRepository fantasyLeagueRepository,
        IFantasyLeagueWeightRepository fantasyLeagueWeightRepository,
        IFantasyDraftRepository fantasyDraftRepository,
        IFantasyPlayerRepository fantasyPlayerRepository,
        IFantasyRepository fantasyRepository
    )
    {
        _logger = logger;
        _fantasyLeagueRepository = fantasyLeagueRepository;
        _fantasyLeagueWeightRepository = fantasyLeagueWeightRepository;
        _fantasyDraftRepository = fantasyDraftRepository;
        _fantasyPlayerRepository = fantasyPlayerRepository;
        _fantasyRepository = fantasyRepository;
    }

    public async Task<FantasyPlayer?> GetFantasyPlayerAsync(DiscordUser? siteUser, int fantasyPlayerId)
    {
        IEnumerable<FantasyLeague> fantasyLeagues = await _fantasyLeagueRepository.GetAccessibleFantasyLeaguesAsync(siteUser);
        FantasyPlayer? fantasyPlayer = await _fantasyPlayerRepository.GetByIdAsync(fantasyPlayerId);

        if (fantasyPlayer != null && !fantasyLeagues.Contains(fantasyPlayer.FantasyLeague))
        {
            // User doesn't have access to this player
            return null;
        }

        return fantasyPlayer;
    }

    public async Task<FantasyLeagueWeight?> GetFantasyLeagueWeightAsync(DiscordUser? siteUser, int fantasyLeagueWeightId)
    {
        IEnumerable<FantasyLeague> fantasyLeagues = await _fantasyLeagueRepository.GetAccessibleFantasyLeaguesAsync(siteUser);
        FantasyLeagueWeight? fantasyLeagueWeight = await _fantasyLeagueWeightRepository.GetByIdAsync(fantasyLeagueWeightId);

        if (fantasyLeagueWeight != null && !fantasyLeagues.Contains(fantasyLeagueWeight.FantasyLeague))
        {
            // User doesn't have access to this player
            return null;
        }

        return fantasyLeagueWeight;
    }

    public async Task<List<FantasyLeagueWeight>> GetFantasyLeagueWeightsAsync(DiscordUser? siteUser)
    {
        IEnumerable<FantasyLeague> fantasyLeagues = await _fantasyLeagueRepository.GetAccessibleFantasyLeaguesAsync(siteUser);
        IEnumerable<FantasyLeagueWeight> fantasyLeagueWeights = await _fantasyLeagueWeightRepository.GetAllAsync();

        return fantasyLeagueWeights.Where(flw => fantasyLeagues.Contains(flw.FantasyLeague)).ToList();
    }

    public async Task<FantasyLeague> GetAccessibleFantasyLeagueAsync(DiscordUser? siteUser, int fantasyLeagueId)
    {
        FantasyLeague? fantasyLeague = await _fantasyLeagueRepository.GetAccessibleFantasyLeagueAsync(fantasyLeagueId, siteUser);
        if (fantasyLeague == null)
        {
            throw new ArgumentException("Fantasy League Id not found");
        }

        return fantasyLeague;
    }

    public async Task<IEnumerable<FantasyLeague>> GetAccessibleFantasyLeaguesAsync(DiscordUser? siteUser)
    {
        return await _fantasyLeagueRepository.GetAccessibleFantasyLeaguesAsync(siteUser);
    }

    public async Task<IEnumerable<FantasyPlayer>> GetFantasyLeaguePlayersAsync(DiscordUser? siteUser, int fantasyLeagueId)
    {
        FantasyLeague? fantasyLeague = await _fantasyLeagueRepository.GetAccessibleFantasyLeagueAsync(fantasyLeagueId, siteUser);
        if (fantasyLeague == null)
        {
            throw new ArgumentException("Fantasy League Id not found");
        }

        return await _fantasyPlayerRepository.GetFantasyLeaguePlayersAsync(fantasyLeague);
    }

    public async Task<FantasyDraft?> GetFantasyDraft(DiscordUser siteUser, int fantasyLeagueId)
    {
        FantasyLeague? fantasyLeague = await _fantasyLeagueRepository.GetAccessibleFantasyLeagueAsync(fantasyLeagueId, siteUser);

        if (fantasyLeague == null)
        {
            throw new ArgumentException("Fantasy League Id not found.");
        }

        return await _fantasyDraftRepository.GetByUserFantasyLeague(fantasyLeague, siteUser);
    }

    public async Task<FantasyDraft> UpdateFantasyDraft(DiscordUser siteUser, FantasyDraft fantasyDraft)
    {
        FantasyLeague? fantasyLeague = await _fantasyLeagueRepository.GetByIdAsync(fantasyDraft.FantasyLeagueId);
        if (fantasyLeague == null)
        {
            throw new ArgumentException("Draft created for invalid Fantasy League Id");
        }

        FantasyDraft? existingUserDraft = await _fantasyDraftRepository.GetByUserFantasyLeague(fantasyLeague, siteUser);
        var draftLockedDate = await _fantasyLeagueRepository.GetLeagueLockedDateAsync(fantasyLeague.Id);
        if (DateTime.UtcNow > draftLockedDate)
        {
            // TODO: Set this up so that a user can draft late, but then the points only count starting from that time
            // If a user hasn't drafted yet let them add it in late, if they already have a draft though return a bad request cannot update
            throw new ArgumentException("Draft is locked for this league");
        }

        // Ensure player has posted a draft that is one of each team position, if there's 2 of the same position then reject it as a bad request
        var fantasyPlayers = await _fantasyPlayerRepository.GetFantasyLeaguePlayersAsync(fantasyLeague);

        // Populate fantasy players by ID for draft picks
        foreach (FantasyDraftPlayer pick in fantasyDraft.DraftPickPlayers)
        {
            pick.FantasyPlayer = pick.FantasyPlayer ?? fantasyPlayers.FirstOrDefault(fp => fp.Id == pick.FantasyPlayerId);
        }

        if (fantasyPlayers.Where(fp => fantasyDraft.DraftPickPlayers.Where(dpp => dpp.FantasyPlayer != null).Any(dpp => dpp.FantasyPlayer!.Id == fp.Id)).GroupBy(fp => fp.TeamPosition).Where(grp => grp.Count() > 1).Count() > 0)
        {
            throw new ArgumentException("Can only draft one of each team position");
        };

        if (existingUserDraft != null)
        {
            // To handle partial drafts we're going to always clear the current draft then add the picks
            await _fantasyDraftRepository.ClearPicksAsync(existingUserDraft);
        }
        else
        {
            existingUserDraft = new FantasyDraft
            {
                FantasyLeagueId = fantasyDraft.FantasyLeagueId,
                DiscordAccountId = siteUser.Id,
                DraftCreated = DateTime.UtcNow,
                DraftPickPlayers = []
            };
            await _fantasyDraftRepository.AddAsync(existingUserDraft);
        }

        // Fantasy Draft may be incomplete, so go through and add the IDs passed
        for (int i = 0; i < fantasyDraft.DraftPickPlayers.Count(); i++)
        {
            if (fantasyDraft.DraftPickPlayers[i].FantasyPlayer != null)
            {
                existingUserDraft = await _fantasyDraftRepository.AddPlayerPickAsync(existingUserDraft, fantasyDraft.DraftPickPlayers[i].FantasyPlayer!);
            }
        }


        return existingUserDraft;
    }

    public async Task<FantasyDraftPointTotals?> GetFantasyDraftPointTotal(DiscordUser siteUser, int fantasyLeagueId)
    {
        FantasyLeague? fantasyLeague = await _fantasyLeagueRepository.GetAccessibleFantasyLeagueAsync(fantasyLeagueId, siteUser);

        if (fantasyLeague == null)
        {
            throw new ArgumentException("Fantasy League Id not found.");
        }

        var fantasyPoints = await _fantasyPlayerRepository.GetFantasyLeaguePlayersAsync(fantasyLeague);
        if (fantasyPoints.Count() == 0)
        {
            // League doesn't have fantasy players/points yet
            return null;
        }

        FantasyDraft? fantasyDraft = await _fantasyDraftRepository.GetByUserFantasyLeague(fantasyLeague, siteUser);
        if (fantasyDraft == null)
        {
            return null;
        }

        return await _fantasyDraftRepository.DraftPointTotalAsync(fantasyDraft);
    }

    public async Task<IEnumerable<FantasyPlayerPoints>> GetFantasyPlayerPoints(DiscordUser siteUser, int fantasyLeagueId, int limit)
    {
        FantasyLeague? fantasyLeague = await _fantasyLeagueRepository.GetAccessibleFantasyLeagueAsync(fantasyLeagueId, siteUser);

        if (fantasyLeague == null)
        {
            throw new ArgumentException("Fantasy League Id not found.");
        }

        FantasyDraft? fantasyDraft = await _fantasyDraftRepository.GetByUserFantasyLeague(fantasyLeague, siteUser);

        if (fantasyDraft == null)
        {
            throw new ArgumentException("Fantasy Draft not found.");
        }

        var fantasyPlayerPointTotals = await _fantasyDraftRepository.FantasyPlayerPointsAsync(fantasyDraft, limit);

        return fantasyPlayerPointTotals;
    }

    public async Task<IEnumerable<FantasyPlayerPointTotals>> GetFantasyLeaguePlayerPointTotalsAsync(DiscordUser? siteUser, int fantasyLeagueId)
    {
        FantasyLeague? fantasyLeague = await _fantasyLeagueRepository.GetAccessibleFantasyLeagueAsync(fantasyLeagueId, siteUser);

        if (fantasyLeague == null)
        {
            throw new ArgumentException("Fantasy League Id not found");
        }

        return await _fantasyDraftRepository.FantasyPlayerPointTotalsByFantasyLeagueAsync(fantasyLeague);
    }

    public async Task<IEnumerable<FantasyPlayerPoints>> GetFantasyLeaguePlayerPointsByMatchAsync(DiscordUser? siteUser, int fantasyLeagueId, int limit)
    {
        FantasyLeague? fantasyLeague = await _fantasyLeagueRepository.GetAccessibleFantasyLeagueAsync(fantasyLeagueId, siteUser);

        if (fantasyLeague == null)
        {
            throw new ArgumentException("Fantasy League Id not found");
        }

        return await _fantasyRepository.FantasyPlayerPointsByFantasyLeagueAsync(fantasyLeague.Id, limit);
    }

    public async Task<IEnumerable<MetadataSummary>> GetMetadataSummariesByFantasyLeagueAsync(DiscordUser? siteUser, int fantasyLeagueId)
    {
        FantasyLeague? fantasyLeague = await _fantasyLeagueRepository.GetAccessibleFantasyLeagueAsync(fantasyLeagueId, siteUser);

        if (fantasyLeague == null)
        {
            throw new ArgumentException("Fantasy League Id not found");
        }

        List<MetadataSummary>? matchSummary = await _fantasyRepository.MetadataSummariesByFantasyLeagueAsync(fantasyLeague);

        if (matchSummary == null)
        {
            throw new ArgumentException("Metadata not found for Fantasy League Id");
        }

        return matchSummary = matchSummary.OrderBy(m => m.FantasyPlayer?.DotaAccount!.Name).ToList();
    }

    public async Task<IEnumerable<FantasyDraftPointTotals>?> GetTop10FantasyDraftsAsync(DiscordUser siteUser, int fantasyLeagueId)
    {
        FantasyLeague? fantasyLeague = await _fantasyLeagueRepository.GetAccessibleFantasyLeagueAsync(fantasyLeagueId, siteUser);

        if (fantasyLeague == null)
        {
            throw new ArgumentException("Fantasy League Id not found");
        }

        List<FantasyDraftPointTotals> fantasyPoints = await _fantasyDraftRepository.AllDraftPointTotalsByFantasyLeagueAsync(fantasyLeague);
        if (fantasyPoints.Count() == 0)
        {
            // League doesn't have fantasy players/points yet
            return null;
        }

        List<FantasyDraftPointTotals> top10Players = fantasyPoints.Where(fp => !fp.IsTeam).OrderByDescending(fp => fp.DraftTotalFantasyPoints).Take(10).ToList();

        // We want the user included even if they're not top 10
        if (!top10Players.Any(tp => tp.FantasyDraft.DiscordAccountId == siteUser.Id))
        {
            var currentPlayer = fantasyPoints.Where(fp => fp.FantasyDraft.DiscordAccountId == siteUser.Id).FirstOrDefault();
            if (currentPlayer != null)
            {
                top10Players.Add(currentPlayer);
            }
        }

        List<FantasyDraftPointTotals> teamsScores = fantasyPoints.Where(fp => fp.IsTeam).OrderByDescending(fp => fp.DraftTotalFantasyPoints).ToList();

        IEnumerable<FantasyDraftPointTotals> unionedLeaderboard = top10Players.Union(teamsScores);

        unionedLeaderboard = unionedLeaderboard.Select(
            lb => new FantasyDraftPointTotals
            {
                //We're doing this to mask the DiscordAccountId
                FantasyDraft = new FantasyDraft
                {
                    Id = lb.FantasyDraft.Id,
                    DraftCreated = lb.FantasyDraft.DraftCreated,
                    DraftLastUpdated = lb.FantasyDraft.DraftLastUpdated,
                    FantasyLeagueId = lb.FantasyDraft.FantasyLeagueId,
                    FantasyLeague = lb.FantasyDraft.FantasyLeague,
                    DraftPickPlayers = lb.FantasyDraft.DraftPickPlayers
                },
                DiscordName = lb.DiscordName,
                IsTeam = lb.IsTeam,
                TeamId = lb.TeamId,
                FantasyPlayerPoints = lb.FantasyPlayerPoints
            }
        )
        .OrderByDescending(fp => fp.DraftTotalFantasyPoints)
        .ToList();

        return unionedLeaderboard;
    }

    public async Task<IEnumerable<MatchHighlights>> GetMatchHighlightsAsync(DiscordUser? siteUser, int fantasyLeagueId, int limit)
    {
        FantasyLeague? fantasyLeague = await _fantasyLeagueRepository.GetAccessibleFantasyLeagueAsync(fantasyLeagueId, siteUser);

        if (fantasyLeague == null)
        {
            throw new ArgumentException("Fantasy League Id not found");
        }

        limit = Math.Min(limit, 100);

        return await _fantasyRepository.GetLastNMatchHighlights(fantasyLeagueId, limit);
    }
}