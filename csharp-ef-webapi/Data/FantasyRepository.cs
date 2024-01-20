using csharp_ef_webapi.Models;
using Microsoft.EntityFrameworkCore;

namespace csharp_ef_webapi.Data;

/// <summary>
/// Service to fetch and transform the Postgres data to read/write fantasy draft data for the webapi
/// example is to fetch all of the current scores, vs adding a new draft. Controllers should handle none of the business
/// logic 
/// </summary>
public class FantasyRepository : IFantasyRepository
{
    private readonly ILogger<FantasyRepository> _logger;
    private AghanimsFantasyContext _dbContext;
    public FantasyRepository(ILogger<FantasyRepository> logger, AghanimsFantasyContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    #region Fantasy
    public async Task<IEnumerable<FantasyPlayer>> GetFantasyPlayersAsync(int? LeagueId)
    {
        _logger.LogInformation($"Fetching Fantasy Players LeagueID: {LeagueId}");
        return await _dbContext.FantasyPlayers
                            .Where(fp => fp.LeagueId == LeagueId || LeagueId == null)
                            .Include(fp => fp.Team)
                            .Include(fp => fp.DotaAccount)
                            .OrderBy(fp => fp.Team.Name)
                            .ThenBy(fp => fp.DotaAccount.Name)
                            .ToListAsync();
    }

    public async Task<IEnumerable<object>> GetUserFantasyDraftsByLeagueAsync(long UserDiscordAccountId, int LeagueId)
    {
        _logger.LogInformation($"Fetching User {UserDiscordAccountId} Fantasy Draft for LeagueID: {LeagueId}");
        return await _dbContext.FantasyDrafts
                            .Where(fd => fd.LeagueId == LeagueId && fd.DiscordAccountId == UserDiscordAccountId)
                            .Include(fd => fd.DraftPickPlayers)
                                .ThenInclude(fp => fp.FantasyPlayer)
                                .ThenInclude(fp => fp.Team)
                            .Include(fd => fd.DraftPickPlayers)
                                .ThenInclude(fp => fp.FantasyPlayer)
                                .ThenInclude(fp => fp.DotaAccount)
                            .Select(fdp => new
                            {
                                fdp.DiscordAccountId,
                                fdp.LeagueId,
                                fdp.DraftCreated,
                                fdp.DraftLastUpdated,
                                Players = fdp.DraftPickPlayers.OrderBy(dpp => dpp.DraftOrder).Select(dpp => dpp.FantasyPlayer).ToList()
                            })
                            .ToListAsync();
    }

    public async Task<IEnumerable<FantasyPlayerPoints>> GetFantasyPlayerPointsAsync(int LeagueId)
    {
        _logger.LogInformation($"Fetching Fantasy Points for LeagueID: {LeagueId}");
        List<FantasyPlayerPoints> fantasyPlayerMatches = await _dbContext.MatchDetails
            .Where(md => md.LeagueId == LeagueId)
            .SelectMany(md => md.Players)
            .Join(
                _dbContext.FantasyPlayers,
                matchDetail => matchDetail.AccountId,
                fantasyPlayer => fantasyPlayer.DotaAccountId,
                (matchDetail, fantasyPlayer) => new FantasyPlayerPoints { Match = matchDetail, FantasyPlayer = fantasyPlayer })
            .ToListAsync();

        return fantasyPlayerMatches;
    }

    public async Task<object?> GetUserTotalFantasyPointsByLeagueAsync(long UserDiscordAccountId, int LeagueId)
    {
        _logger.LogInformation($"Fetching User {UserDiscordAccountId} Fantasy Points for LeagueID: {LeagueId}");
        var fantasyPoints = await GetFantasyPlayerPointsAsync(LeagueId);
        var fantasyTotalLeaguePoints = fantasyPoints
                                        .GroupBy(fp => fp.FantasyPlayer)
                                        .Select(group => new
                                        {
                                            FantasyPlayer = group.Key,
                                            TotalLeaguePoints = group.Sum(result => result.TotalMatchFantasyPoints)
                                        })
                                        .ToList();
        var userDraft = await _dbContext.FantasyDrafts
                                .Where(fd => fd.LeagueId == LeagueId && fd.DiscordAccountId == UserDiscordAccountId)
                                .Select(fd => fd)
                                .FirstOrDefaultAsync();

        if (userDraft == null)
        {
            return null;
        }
        var userDraftWithPoints = new
        {
            userDraft.LeagueId,
            userDraft.DraftCreated,
            userDraft.DraftLastUpdated,
            userDraft.DraftPickOne,
            DraftPickOnePoints = fantasyTotalLeaguePoints.Where(tlp => tlp.FantasyPlayer.Id == userDraft.DraftPickOne).Select(tlp => tlp.TotalLeaguePoints).FirstOrDefault(),
            userDraft.DraftPickTwo,
            DraftPickTwoPoints = fantasyTotalLeaguePoints.Where(tlp => tlp.FantasyPlayer.Id == userDraft.DraftPickTwo).Select(tlp => tlp.TotalLeaguePoints).FirstOrDefault(),
            userDraft.DraftPickThree,
            DraftPickThreePoints = fantasyTotalLeaguePoints.Where(tlp => tlp.FantasyPlayer.Id == userDraft.DraftPickThree).Select(tlp => tlp.TotalLeaguePoints).FirstOrDefault(),
            userDraft.DraftPickFour,
            DraftPickFourPoints = fantasyTotalLeaguePoints.Where(tlp => tlp.FantasyPlayer.Id == userDraft.DraftPickFour).Select(tlp => tlp.TotalLeaguePoints).FirstOrDefault(),
            userDraft.DraftPickFive,
            DraftPickFivePoints = fantasyTotalLeaguePoints.Where(tlp => tlp.FantasyPlayer.Id == userDraft.DraftPickFive).Select(tlp => tlp.TotalLeaguePoints).FirstOrDefault()
        };
        return userDraftWithPoints;
    }

    public async Task<DateTime> GetLeagueLockedDate(int LeagueId)
    {
        _logger.LogInformation($"Fetching Draft Locked Date for LeagueID: {LeagueId}");
        return await _dbContext.Leagues.Where(l => l.id == LeagueId).Select(l => l.fantasyDraftLocked).FirstOrDefaultAsync();
    }

    public async Task<object?> AddNewUserFantasyDraftAsync(long UserDiscordAccountId, FantasyDraft FantasyDraft)
    {
        FantasyDraft.DraftLastUpdated = DateTime.UtcNow;
        FantasyDraft.DiscordAccountId = UserDiscordAccountId;
        FantasyPlayer pickOne = await _dbContext.FantasyPlayers.FindAsync(FantasyDraft.DraftPickOne) ?? throw new Exception("Invalid Player 1");
        FantasyPlayer pickTwo = await _dbContext.FantasyPlayers.FindAsync(FantasyDraft.DraftPickTwo) ?? throw new Exception("Invalid Player 2");
        FantasyPlayer pickThree = await _dbContext.FantasyPlayers.FindAsync(FantasyDraft.DraftPickThree) ?? throw new Exception("Invalid Player 3");
        FantasyPlayer pickFour = await _dbContext.FantasyPlayers.FindAsync(FantasyDraft.DraftPickFour) ?? throw new Exception("Invalid Player 4");
        FantasyPlayer pickFive = await _dbContext.FantasyPlayers.FindAsync(FantasyDraft.DraftPickFive) ?? throw new Exception("Invalid Player 5");

        var updateDraft = await _dbContext.FantasyDrafts.Where(fd => fd.LeagueId == FantasyDraft.LeagueId && fd.DiscordAccountId == UserDiscordAccountId).FirstOrDefaultAsync();
        FantasyDraft.DraftCreated = updateDraft?.DraftCreated ?? DateTime.UtcNow;

        List<FantasyDraftPlayer> newDraftPlayers = new List<FantasyDraftPlayer>
        {
            new FantasyDraftPlayer() { FantasyPlayer = pickOne, DraftOrder = 1 },
            new FantasyDraftPlayer() { FantasyPlayer = pickTwo, DraftOrder = 2  },
            new FantasyDraftPlayer() { FantasyPlayer = pickThree, DraftOrder = 3  },
            new FantasyDraftPlayer() { FantasyPlayer = pickFour, DraftOrder = 4  },
            new FantasyDraftPlayer() { FantasyPlayer = pickFive, DraftOrder = 5  }
        };
        FantasyDraft.DraftPickPlayers = newDraftPlayers;

        // Update draft if it exists else insert a new one
        if (updateDraft == null)
        {
            _dbContext.FantasyDrafts.Add(FantasyDraft);
        }
        else
        {
            List<FantasyDraftPlayer> fantasyDraftPlayers = await _dbContext.FantasyDraftPlayers.Where(fdp => fdp.FantasyDraftId == updateDraft.Id).ToListAsync();
            _dbContext.FantasyDraftPlayers.RemoveRange(fantasyDraftPlayers);

            updateDraft.DraftPickOne = FantasyDraft.DraftPickOne;
            updateDraft.DraftPickTwo = FantasyDraft.DraftPickTwo;
            updateDraft.DraftPickThree = FantasyDraft.DraftPickThree;
            updateDraft.DraftPickFour = FantasyDraft.DraftPickFour;
            updateDraft.DraftPickFive = FantasyDraft.DraftPickFive;
            updateDraft.DraftLastUpdated = FantasyDraft.DraftLastUpdated;
            updateDraft.DraftPickPlayers = FantasyDraft.DraftPickPlayers;
            _dbContext.FantasyDrafts.Update(updateDraft);
        }

        await _dbContext.SaveChangesAsync();

        var createOutputFormatted = new
        {
            FantasyDraft.DiscordAccountId,
            FantasyDraft.LeagueId,
            FantasyDraft.DraftCreated,
            FantasyDraft.DraftLastUpdated,
            Players = FantasyDraft.DraftPickPlayers.Select(dpp => dpp.FantasyPlayer).ToList()
        };
        return createOutputFormatted;
    }
    #endregion Fantasy

    #region League
    public async Task<IEnumerable<League>> GetLeaguesAsync(bool? IsActive)
    {
        _logger.LogInformation($"Fetching All Leagues");
        return await _dbContext.Leagues
                .Where(l => IsActive == null || l.isActive == IsActive)
                .ToListAsync();
    }

    public async Task<League?> GetLeagueAsync(int LeagueId)
    {
        _logger.LogInformation($"Fetching Single League {LeagueId}");
        return await _dbContext.Leagues.FindAsync(LeagueId);
    }

    public async Task<IEnumerable<MatchHistory>> GetMatchHistoryAsync(int LeagueId)
    {
        _logger.LogInformation($"Getting Match History for League {LeagueId}");
        return await _dbContext.MatchHistory
            .Where(mh => mh.LeagueId == LeagueId)
            .Include(mh => mh.Players)
            .ToListAsync();
    }

    public async Task<IEnumerable<MatchDetail>> GetMatchDetailsAsync(int LeagueId)
    {
        _logger.LogInformation($"Getting Match Details for League {LeagueId}");
        return await _dbContext.MatchDetails
            .Where(md => md.LeagueId == LeagueId)
            .Include(md => md.PicksBans)
            .ToListAsync();
    }

    public async Task<MatchDetail?> GetMatchDetailAsync(int LeagueId, long MatchId)
    {
        _logger.LogInformation($"Getting Match Detail for Match: {MatchId} League: {LeagueId}");
        return await _dbContext.MatchDetails
                .Where(md => md.LeagueId == LeagueId && md.MatchId == MatchId)
                .Include(md => md.PicksBans)
                .Include(md => md.Players).ThenInclude(p => p.AbilityUpgrades)
                .FirstOrDefaultAsync();
    }
    #endregion League

    #region Match
    public async Task<IEnumerable<MatchDetailsPlayer>> GetMatchDetailPlayersByLeagueAsync(int? LeagueId)
    {
        _logger.LogInformation($"Getting Match Details Players for League ID: {LeagueId}");
        return await _dbContext.MatchDetails
                .Where(md => md.LeagueId == LeagueId || LeagueId == null)
                .SelectMany(md => md.Players)
                .Where(p => p.LeaverStatus != 1) // Filter out games players left (typically false starts)
                .ToListAsync();
    }
    #endregion Match

    #region Player
    public async Task<IEnumerable<Account>> GetPlayerAccounts()
    {
        _logger.LogInformation($"Getting Player Accounts");
        return await _dbContext.Accounts
                .ToListAsync();
    }
    #endregion Player

    #region Team
    public async Task<IEnumerable<Team>> GetTeamsAsync()
    {
        _logger.LogInformation($"Getting Teams loaded into DB");
        return await _dbContext.Teams.ToListAsync();
    }
    #endregion Team

    #region Hero
    public async Task<IEnumerable<Hero>> GetHeroesAsync()
    {
        _logger.LogInformation($"Getting Heroes loaded into DB");
        return await _dbContext.Heroes.ToListAsync();
    }

    #endregion Hero
}