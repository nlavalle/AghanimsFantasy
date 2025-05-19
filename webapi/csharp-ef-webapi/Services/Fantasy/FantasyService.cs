using csharp_ef_webapi.ViewModels;
using DataAccessLibrary.Data;
using DataAccessLibrary.Data.Facades;
using DataAccessLibrary.Data.Identity;
using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace csharp_ef_webapi.Services;
public class FantasyService
{
    private readonly ILogger<FantasyService> _logger;
    private readonly AghanimsFantasyContext _dbContext;
    private readonly FantasyDraftFacade _fantasyDraftFacade;
    private readonly FantasyPointsFacade _fantasyPointsFacade;
    private readonly UserManager<AghanimsFantasyUser> _userManager;

    public FantasyService(
        ILogger<FantasyService> logger,
        AghanimsFantasyContext dbContext,
        FantasyDraftFacade fantasyDraftFacade,
        FantasyPointsFacade fantasyPointsFacade,
        UserManager<AghanimsFantasyUser> userManager
    )
    {
        _logger = logger;
        _dbContext = dbContext;
        _fantasyDraftFacade = fantasyDraftFacade;
        _fantasyPointsFacade = fantasyPointsFacade;
        _userManager = userManager;
    }

    public async Task<FantasyPlayer?> GetFantasyPlayerAsync(ClaimsPrincipal siteUser, int fantasyPlayerId)
    {
        IEnumerable<FantasyLeague> fantasyLeagues = await GetAccessibleFantasyLeaguesAsync(siteUser);
        FantasyPlayer? fantasyPlayer = await _dbContext.FantasyPlayers.FindAsync(fantasyPlayerId);

        if (fantasyPlayer != null && !fantasyLeagues.Contains(fantasyPlayer.FantasyLeague))
        {
            // User doesn't have access to this player
            return null;
        }

        return fantasyPlayer;
    }

    public async Task<List<FantasyPlayerViewModel>> GetFantasyPlayerViewModelsAsync(ClaimsPrincipal siteUser, int fantasyLeagueId)
    {
        // Construct the complete view model object to return all the relevant FantasyPlayer data for the front end
        List<FantasyLeague> fantasyLeagues = await GetAccessibleFantasyLeaguesAsync(siteUser);
        FantasyLeague? fantasyLeague = fantasyLeagues.FirstOrDefault(fl => fl.Id == fantasyLeagueId);
        if (fantasyLeague == null)
        {
            throw new ArgumentException("Fantasy League Id not found");
        }

        // FantasyPlayers
        var fantasyPlayers = await _dbContext.FantasyPlayers.Where(fp => fp.FantasyLeagueId == fantasyLeagueId).ToListAsync();
        // Costs
        var costs = await _fantasyDraftFacade.GetFantasyPlayerCostsAsync(fantasyLeagueId);
        // NormalizedAverages
        var averages = await _fantasyPointsFacade.GetFantasyNormalizedAveragesAsync(fantasyLeagueId);
        // TopHeroes
        var topHeroes = await _fantasyPointsFacade.GetFantasyPlayerTopHeroesAsync(fantasyLeagueId);

        return fantasyPlayers
            .Select(fp => new FantasyPlayerViewModel()
            {
                fantasyPlayer = fp,
                cost = costs.Where(c => c.Account.Id == fp.DotaAccountId).Sum(c => c.EstimatedCost),
                normalizedAverages = averages.FirstOrDefault(a => a.FantasyPlayer.Id == fp.Id),
                topHeroes = topHeroes.Where(th => th.FantasyPlayerId == fp.Id).ToList()
            })
            .ToList();
    }

    public async Task<FantasyLeagueWeight?> GetFantasyLeagueWeightAsync(ClaimsPrincipal siteUser, int fantasyLeagueWeightId)
    {
        IEnumerable<FantasyLeague> fantasyLeagues = await GetAccessibleFantasyLeaguesAsync(siteUser);
        FantasyLeagueWeight? fantasyLeagueWeight = await _dbContext.FantasyLeagueWeights.FindAsync(fantasyLeagueWeightId);

        if (fantasyLeagueWeight != null && !fantasyLeagues.Contains(fantasyLeagueWeight.FantasyLeague))
        {
            // User doesn't have access to this player
            return null;
        }

        return fantasyLeagueWeight;
    }

    public async Task<List<FantasyLeagueWeight>> GetFantasyLeagueWeightsAsync(ClaimsPrincipal siteUser)
    {
        IEnumerable<FantasyLeague> fantasyLeagues = await GetAccessibleFantasyLeaguesAsync(siteUser);
        IEnumerable<FantasyLeagueWeight> fantasyLeagueWeights = await _dbContext.FantasyLeagueWeights.ToListAsync();

        return fantasyLeagueWeights.Where(flw => fantasyLeagues.Contains(flw.FantasyLeague)).ToList();
    }

    public async Task<FantasyLeague> GetAccessibleFantasyLeagueAsync(ClaimsPrincipal siteUser, int fantasyLeagueId)
    {
        List<FantasyLeague> fantasyLeagues = await GetAccessibleFantasyLeaguesAsync(siteUser);
        FantasyLeague? fantasyLeague = fantasyLeagues.FirstOrDefault(fl => fl.Id == fantasyLeagueId);
        if (fantasyLeague == null)
        {
            throw new ArgumentException("Fantasy League Id not found");
        }

        return fantasyLeague;
    }

    public async Task<List<FantasyLeague>> GetAccessibleFantasyLeaguesAsync(ClaimsPrincipal siteUser)
    {
        // Everyone has access to all public leagues
        var fantasyLeagues = await _dbContext.FantasyLeagues.Where(fl => !fl.IsPrivate).ToListAsync();

        if (siteUser.Identity?.IsAuthenticated ?? false)
        {
            AghanimsFantasyUser user = await GetUserFromContext(siteUser);

            // Get Private Fantasy Leagues user is a part of and append it to the list
            var privateLeagues = await _dbContext.FantasyPrivateLeaguePlayers
                .Where(pfp => pfp.UserId == user.Id && pfp.FantasyLeague != null)
                .Select(pfp => pfp.FantasyLeague)
                .Distinct()
                .ToListAsync();

            fantasyLeagues.AddRange(privateLeagues!);
        }

        return fantasyLeagues;
    }

    public async Task<IEnumerable<FantasyPlayer>> GetFantasyLeaguePlayersAsync(ClaimsPrincipal siteUser, int fantasyLeagueId)
    {
        FantasyLeague? fantasyLeague = await GetAccessibleFantasyLeagueAsync(siteUser, fantasyLeagueId);
        if (fantasyLeague == null)
        {
            throw new ArgumentException("Fantasy League Id not found");
        }

        return await GetFantasyPlayerByFantasyLeagueAsync(fantasyLeague);
    }

    public async Task<FantasyDraft?> GetFantasyDraft(ClaimsPrincipal siteUser, int fantasyLeagueId)
    {
        FantasyLeague? fantasyLeague = await GetAccessibleFantasyLeagueAsync(siteUser, fantasyLeagueId);

        if (fantasyLeague == null)
        {
            throw new ArgumentException("Fantasy League Id not found.");
        }

        AghanimsFantasyUser user = await GetUserFromContext(siteUser);

        return await _fantasyDraftFacade.GetByUserFantasyLeague(fantasyLeague, user);
    }

    public async Task<FantasyDraft> UpdateFantasyDraft(ClaimsPrincipal siteUser, FantasyDraft fantasyDraft)
    {
        FantasyLeague? fantasyLeague = await _dbContext.FantasyLeagues.FindAsync(fantasyDraft.FantasyLeagueId);
        if (fantasyLeague == null)
        {
            throw new ArgumentException("Draft created for invalid Fantasy League Id");
        }

        AghanimsFantasyUser user = await GetUserFromContext(siteUser);

        FantasyDraft? existingUserDraft = await _fantasyDraftFacade.GetByUserFantasyLeague(fantasyLeague, user);
        if (DateTime.UtcNow > DateTime.UnixEpoch.AddSeconds(fantasyLeague.FantasyDraftLocked))
        {
            // TODO: Set this up so that a user can draft late, but then the points only count starting from that time
            // If a user hasn't drafted yet let them add it in late, if they already have a draft though return a bad request cannot update
            throw new ArgumentException("Draft is locked for this league");
        }

        // Ensure player has posted a draft that is one of each team position, if there's 2 of the same position then reject it as a bad request
        var fantasyPlayers = await GetFantasyPlayerByFantasyLeagueAsync(fantasyLeague);

        // Populate fantasy players by ID for draft picks
        foreach (FantasyDraftPlayer pick in fantasyDraft.DraftPickPlayers)
        {
            pick.FantasyPlayer = pick.FantasyPlayer ?? fantasyPlayers.FirstOrDefault(fp => fp.Id == pick.FantasyPlayerId);
        }

        if (fantasyPlayers.Where(fp => fantasyDraft.DraftPickPlayers.Where(dpp => dpp.FantasyPlayer != null).Any(dpp => dpp.FantasyPlayer!.Id == fp.Id)).GroupBy(fp => fp.TeamPosition).Where(grp => grp.Count() > 1).Count() > 0)
        {
            throw new ArgumentException("Can only draft one of each team position");
        }

        var fantasyPlayerCosts = await _dbContext.FantasyPlayerBudgetProbability
                .Include(fpbp => fpbp.Account)
                .Where(fpbp => fpbp.FantasyLeague.Id == fantasyLeague.Id)
                .ToListAsync();

        var testPlayers = fantasyPlayerCosts.Where(fpc => fantasyDraft.DraftPickPlayers.Where(fp => fp.FantasyPlayer?.DotaAccount != null)
                .Select(fp => fp.FantasyPlayer?.DotaAccount!.Id)
                .Contains(fpc.Account.Id))
                .ToList();

        var testSum = testPlayers.Sum(fpc => fpc.EstimatedCost);

        if (fantasyPlayerCosts.Where(fpc => fantasyDraft.DraftPickPlayers
                .Where(fp => fp.FantasyPlayer != null)
                .Select(fp => fp.FantasyPlayer!.DotaAccountId)
                .Contains(fpc.Account.Id))
                .Sum(fpc => fpc.EstimatedCost) > 600)
        {
            throw new ArgumentException("Draft exceeds 600g budget");
        }

        if (existingUserDraft != null)
        {
            // To handle partial drafts we're going to always clear the current draft then add the picks
            await _fantasyDraftFacade.ClearPicksAsync(existingUserDraft);
        }
        else
        {
            AghanimsFantasyUser existingUser = await GetUserFromContext(siteUser);
            existingUserDraft = new FantasyDraft
            {
                FantasyLeagueId = fantasyDraft.FantasyLeagueId,
                // DiscordAccountId = siteUser.Id,
                UserId = existingUser.Id,
                DraftCreated = DateTime.UtcNow,
                DraftPickPlayers = []
            };
            await _dbContext.FantasyDrafts.AddAsync(existingUserDraft);
            await _dbContext.SaveChangesAsync();
        }

        // Fantasy Draft may be incomplete, so go through and add the IDs passed
        for (int i = 0; i < fantasyDraft.DraftPickPlayers.Count(); i++)
        {
            if (fantasyDraft.DraftPickPlayers[i].FantasyPlayer != null)
            {
                existingUserDraft = await _fantasyDraftFacade.AddPlayerPickAsync(existingUserDraft, fantasyDraft.DraftPickPlayers[i].FantasyPlayer!);
            }
        }

        return existingUserDraft;
    }

    public async Task<IEnumerable<FantasyDraftPointTotals>> GetFantasyDraftPointTotals(ClaimsPrincipal siteUser, int leagueId)
    {
        IEnumerable<FantasyLeague> fantasyLeagues = await GetAccessibleFantasyLeaguesAsync(siteUser);
        fantasyLeagues = fantasyLeagues.Where(fl => fl.LeagueId == leagueId);

        AghanimsFantasyUser user = await GetUserFromContext(siteUser);
        List<FantasyDraftPointTotals> fantasyDraftPointsTotals = new List<FantasyDraftPointTotals>();
        foreach (FantasyLeague fantasyLeague in fantasyLeagues)
        {
            FantasyDraft? fantasyDraft = await _fantasyDraftFacade.GetByUserFantasyLeague(fantasyLeague, user);
            if (fantasyDraft != null)
            {
                FantasyDraftPointTotals? fantasyPointTotal = await _fantasyDraftFacade.DraftPointTotalAsync(fantasyDraft);
                if (fantasyPointTotal != null)
                {
                    fantasyDraftPointsTotals.Add(fantasyPointTotal);
                }
            }
        }

        return fantasyDraftPointsTotals;
    }

    public async Task<IEnumerable<FantasyPlayerPoints>> GetFantasyPlayerPoints(ClaimsPrincipal siteUser, int fantasyLeagueId, int limit)
    {
        FantasyLeague? fantasyLeague = await GetAccessibleFantasyLeagueAsync(siteUser, fantasyLeagueId);

        if (fantasyLeague == null)
        {
            throw new ArgumentException("Fantasy League Id not found.");
        }

        AghanimsFantasyUser user = await GetUserFromContext(siteUser);
        FantasyDraft? fantasyDraft = await _fantasyDraftFacade.GetByUserFantasyLeague(fantasyLeague, user);

        if (fantasyDraft == null)
        {
            throw new ArgumentException("Fantasy Draft not found.");
        }

        var fantasyPlayerPointTotals = await _fantasyDraftFacade.FantasyPlayerPointsAsync(fantasyDraft, limit);

        return fantasyPlayerPointTotals;
    }

    public async Task<IEnumerable<FantasyPlayerPointTotals>> GetFantasyLeaguePlayerPointTotalsAsync(ClaimsPrincipal siteUser, int fantasyLeagueId)
    {
        FantasyLeague? fantasyLeague = await GetAccessibleFantasyLeagueAsync(siteUser, fantasyLeagueId);

        if (fantasyLeague == null)
        {
            throw new ArgumentException("Fantasy League Id not found");
        }

        return await _fantasyDraftFacade.FantasyPlayerPointTotalsByFantasyLeagueAsync(fantasyLeague);
    }

    public async Task<IEnumerable<FantasyPlayerPoints>> GetFantasyLeaguePlayerPointsByMatchAsync(ClaimsPrincipal siteUser, int fantasyLeagueId, int limit)
    {
        FantasyLeague? fantasyLeague = await GetAccessibleFantasyLeagueAsync(siteUser, fantasyLeagueId);

        if (fantasyLeague == null)
        {
            throw new ArgumentException("Fantasy League Id not found");
        }

        return await _fantasyPointsFacade.FantasyPlayerPointsByFantasyLeagueAsync(fantasyLeague.Id, limit);
    }

    public async Task<IEnumerable<MetadataSummary>> GetMetadataSummariesByFantasyLeagueAsync(ClaimsPrincipal siteUser, int fantasyLeagueId)
    {
        FantasyLeague? fantasyLeague = await GetAccessibleFantasyLeagueAsync(siteUser, fantasyLeagueId);

        if (fantasyLeague == null)
        {
            throw new ArgumentException("Fantasy League Id not found");
        }

        List<MetadataSummary>? matchSummary = await _fantasyPointsFacade.MetadataSummariesByFantasyLeagueAsync(fantasyLeague);

        if (matchSummary == null)
        {
            throw new ArgumentException("Metadata not found for Fantasy League Id");
        }

        return matchSummary = matchSummary.OrderBy(m => m.FantasyPlayer?.DotaAccount!.Name).ToList();
    }

    public async Task<IEnumerable<FantasyDraftPointTotals>?> GetTop10FantasyDraftsAsync(ClaimsPrincipal siteUser, int fantasyLeagueId)
    {
        FantasyLeague? fantasyLeague = await GetAccessibleFantasyLeagueAsync(siteUser, fantasyLeagueId);

        if (fantasyLeague == null)
        {
            throw new ArgumentException("Fantasy League Id not found");
        }

        List<FantasyDraftPointTotals> fantasyPoints = await _fantasyDraftFacade.AllDraftPointTotalsByFantasyLeagueAsync(fantasyLeague);
        if (fantasyPoints.Count() == 0)
        {
            // League doesn't have fantasy players/points yet
            return null;
        }

        List<FantasyDraftPointTotals> top10Players = fantasyPoints.OrderByDescending(fp => fp.DraftTotalFantasyPoints).Take(10).ToList();

        AghanimsFantasyUser user = await GetUserFromContext(siteUser);
        // We want the user included even if they're not top 10
        if (!top10Players.Any(tp => tp.FantasyDraft.UserId == user.Id))
        {
            var currentPlayer = fantasyPoints.Where(fp => fp.FantasyDraft.UserId == user.Id).FirstOrDefault();
            if (currentPlayer != null)
            {
                top10Players.Add(currentPlayer);
            }
        }

        return top10Players.Select(
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
                UserName = lb.UserName,
                FantasyPlayerPoints = lb.FantasyPlayerPoints
            }
        )
        .OrderByDescending(fp => fp.DraftTotalFantasyPoints)
        .ToList();
    }

    public async Task<LeaderboardStats> GetLeaderboardStatsAsync(ClaimsPrincipal siteUser, int fantasyLeagueId)
    {
        FantasyLeague? fantasyLeague = await GetAccessibleFantasyLeagueAsync(siteUser, fantasyLeagueId);

        if (fantasyLeague == null)
        {
            throw new ArgumentException("Fantasy League Id not found");
        }

        List<FantasyDraftPointTotals> fantasyPoints = await _fantasyDraftFacade.AllDraftPointTotalsByFantasyLeagueAsync(fantasyLeague);
        if (fantasyPoints.Count() == 0)
        {
            // League doesn't have fantasy players/points yet
            return new LeaderboardStats
            {
                DrafterPercentile = 0,
                TotalDrafts = 0
            };
        }

        LeaderboardStats leaderboardStats = new LeaderboardStats();
        leaderboardStats.TotalDrafts = fantasyPoints.Count();

        AghanimsFantasyUser user = await GetUserFromContext(siteUser);
        // Percentile = (Number of Values Below “x” / Total Number of Values) × 100
        var siteUserPoints = fantasyPoints.FirstOrDefault(fp => fp.UserName == user.DisplayName)?.DraftTotalFantasyPoints ?? 0;
        leaderboardStats.DrafterPercentile = decimal.Divide(fantasyPoints.Count(fp => fp.DraftTotalFantasyPoints < siteUserPoints), fantasyPoints.Count()) * 100;

        return leaderboardStats;
    }

    public async Task<IEnumerable<MatchHighlights>> GetMatchHighlightsAsync(ClaimsPrincipal siteUser, int fantasyLeagueId, int limit)
    {
        FantasyLeague? fantasyLeague = await GetAccessibleFantasyLeagueAsync(siteUser, fantasyLeagueId);

        if (fantasyLeague == null)
        {
            throw new ArgumentException("Fantasy League Id not found");
        }

        limit = Math.Min(limit, 100);

        return await _fantasyPointsFacade.GetLastNMatchHighlights(fantasyLeagueId, limit);
    }

    public async Task<decimal> GetUserBalance(ClaimsPrincipal siteUser)
    {
        AghanimsFantasyUser user = await GetUserFromContext(siteUser);
        return await _dbContext.FantasyLedger.Where(fl => fl.UserId == user.Id).SumAsync(fl => fl.Amount);
    }

    private async Task<List<FantasyPlayer>> GetFantasyPlayerByFantasyLeagueAsync(FantasyLeague fantasyLeague)
    {
        _logger.LogDebug($"Get Fantasy Players by Fantasy League Id: {fantasyLeague.Id}");

        var fantasyPlayerLeagueQuery = _dbContext.FantasyPlayers
            .Include(fp => fp.FantasyLeague)
            .Include(fp => fp.Team)
            .Include(fp => fp.DotaAccount)
            .Where(fp => fp.FantasyLeagueId == fantasyLeague.Id)
            .OrderBy(fp => fp.Team!.Name)
                .ThenBy(fp => fp.DotaAccount!.Name);

        _logger.LogDebug($"Get Fantasy Players by Fantasy League Query: {fantasyPlayerLeagueQuery.ToQueryString()}");

        return await fantasyPlayerLeagueQuery.ToListAsync();
    }

    private async Task<AghanimsFantasyUser> GetUserFromContext(ClaimsPrincipal siteUser)
    {
        return await _userManager.GetUserAsync(siteUser) ?? throw new Exception("Invalid user");
    }
}