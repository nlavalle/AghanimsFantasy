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
    public async Task<IEnumerable<FantasyPlayer>> FantasyPlayersByLeagueAsync(int? LeagueId)
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

    public async Task<IEnumerable<FantasyDraft>> FantasyDraftsByUserLeagueAsync(long UserDiscordAccountId, int LeagueId)
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
                            .ToListAsync();
    }

    public async Task<IEnumerable<FantasyPlayerPoints>> FantasyPlayerPointsByLeagueAsync(int LeagueId)
    {
        _logger.LogInformation($"Fetching Fantasy Points for LeagueID: {LeagueId}");
        List<FantasyPlayerPoints> fantasyPlayerMatches = await _dbContext.FantasyPlayers
            .Where(fdp => fdp.LeagueId == LeagueId)
                    .Include(fdp => fdp.Team)
                    .Include(fdp => fdp.DotaAccount)
            .Join(
                _dbContext.Leagues,
                fp => fp.LeagueId,
                lg => lg.Id,
                (fp, lg) => new { League = lg, FantasyPlayer = fp }
            )
            .SelectMany(
                md => _dbContext.MatchDetails.Where(
                    mdl => mdl.LeagueId == md.League.LeagueId && mdl.StartTime >= md.League.LeagueStartTime && mdl.StartTime <= md.League.LeagueEndTime
                ).DefaultIfEmpty(),
                (fp, md) => new { fp.League, fp.FantasyPlayer, MatchDetail = md }
            )
            .SelectMany(
                group => _dbContext.MatchDetailsPlayers.Where(mdp => mdp.MatchId == group.MatchDetail.MatchId && mdp.AccountId == group.FantasyPlayer.DotaAccountId).DefaultIfEmpty(),
                (fdp, mdp) => new FantasyPlayerPoints
                {
                    FantasyDraft = new FantasyDraft(),
                    Match = mdp,
                    FantasyPlayer = fdp.FantasyPlayer
                })
            .ToListAsync();

        return fantasyPlayerMatches;
    }

    public async Task<IEnumerable<FantasyPlayerPoints>> FantasyDraftPointsByLeagueAsync(int LeagueId)
    {
        _logger.LogInformation($"Fetching Fantasy Points for LeagueID: {LeagueId}");
        List<FantasyPlayerPoints> fantasyPlayerMatches = await _dbContext.FantasyDrafts
            .Where(fd => fd.LeagueId == LeagueId)
            .Include(fd => fd.DraftPickPlayers)
            .Join(
                _dbContext.FantasyDraftPlayers,
                fd => fd.Id,
                fdp => fdp.FantasyDraftId,
                (fd, fdp) => new { FantasyDraft = fd, FantasyDraftPlayer = fdp }
            )
            .Join(
                _dbContext.FantasyPlayers,
                fdp => fdp.FantasyDraftPlayer.FantasyPlayerId,
                fp => fp.Id,
                (fdp, fp) => new { fdp.FantasyDraft, fdp.FantasyDraftPlayer, FantasyPlayer = fp }
            )
            .Join(
                _dbContext.Leagues,
                fp => fp.FantasyDraft.LeagueId,
                lg => lg.Id,
                (fp, lg) => new { League = lg, fp.FantasyDraft, fp.FantasyDraftPlayer, fp.FantasyPlayer }
            )
            //Left join matches in case league hasn't started
            .SelectMany(
                md => _dbContext.MatchDetails.Where(
                    mdl => mdl.LeagueId == md.League.LeagueId && mdl.StartTime >= md.League.LeagueStartTime && mdl.StartTime <= md.League.LeagueEndTime
                ),
                (fp, md) => new { fp.League, fp.FantasyDraft, fp.FantasyDraftPlayer, fp.FantasyPlayer, MatchDetail = md }
            )
            .SelectMany(
                group => _dbContext.MatchDetailsPlayers.Where(mdp => mdp.MatchId == group.MatchDetail.MatchId && mdp.AccountId == group.FantasyPlayer.DotaAccountId),
                (fdp, mdp) => new FantasyPlayerPoints
                {
                    FantasyDraft = fdp.FantasyDraft,
                    Match = mdp,
                    FantasyPlayer = fdp.FantasyPlayer
                })
            .ToListAsync();


        return fantasyPlayerMatches;
    }

    public async Task<IEnumerable<FantasyPlayerPoints>> FantasyDraftPointsByUserLeagueAsync(long UserDiscordAccountId, int LeagueId)
    {
        _logger.LogInformation($"Fetching Fantasy Points for LeagueID: {LeagueId}");
        List<FantasyPlayerPoints> fantasyPlayerMatches = await _dbContext.FantasyDrafts
            .Where(fd => fd.LeagueId == LeagueId && fd.DiscordAccountId == UserDiscordAccountId)
            .Include(fd => fd.DraftPickPlayers)
            .Join(
                _dbContext.FantasyDraftPlayers,
                fd => fd.Id,
                fdp => fdp.FantasyDraftId,
                (fd, fdp) => new { FantasyDraft = fd, FantasyDraftPlayer = fdp }
            )
            .Join(
                _dbContext.FantasyPlayers,
                fdp => fdp.FantasyDraftPlayer.FantasyPlayerId,
                fp => fp.Id,
                (fdp, fp) => new { fdp.FantasyDraft, fdp.FantasyDraftPlayer, FantasyPlayer = fp }
            )
            .Join(
                _dbContext.Leagues,
                fp => fp.FantasyDraft.LeagueId,
                lg => lg.Id,
                (fp, lg) => new { League = lg, fp.FantasyDraft, fp.FantasyDraftPlayer, fp.FantasyPlayer }
            )
            //Left join matches in case league hasn't started
            .SelectMany(
                md => _dbContext.MatchDetails.Where(
                    mdl => mdl.LeagueId == md.League.LeagueId && mdl.StartTime >= md.League.LeagueStartTime && mdl.StartTime <= md.League.LeagueEndTime
                ),
                (fp, md) => new { fp.League, fp.FantasyDraft, fp.FantasyDraftPlayer, fp.FantasyPlayer, MatchDetail = md }
            )
            .SelectMany(
                group => _dbContext.MatchDetailsPlayers.Where(mdp => mdp.MatchId == group.MatchDetail.MatchId && mdp.AccountId == group.FantasyPlayer.DotaAccountId),
                (fdp, mdp) => new FantasyPlayerPoints
                {
                    FantasyDraft = fdp.FantasyDraft,
                    Match = mdp ?? new MatchDetailsPlayer(),
                    FantasyPlayer = fdp.FantasyPlayer
                })
            .ToListAsync();


        return fantasyPlayerMatches;
    }

    public IEnumerable<FantasyPlayerPointTotals> AggregateFantasyPlayerPoints(IEnumerable<FantasyPlayerPoints> fantasyPlayerPoints)
    {
        return fantasyPlayerPoints
                .GroupBy(fp => fp.FantasyPlayer)
                .Select(group => new FantasyPlayerPointTotals
                {
                    FantasyPlayer = group.Key,
                    TotalMatches = group.Where(g => g.Match != null).Count(),
                    TotalKills = group.Sum(result => result.Kills),
                    TotalKillsPoints = group.Sum(result => result.KillsPoints),
                    TotalDeaths = group.Sum(result => result.Deaths),
                    TotalDeathsPoints = group.Sum(result => result.DeathsPoints),
                    TotalAssists = group.Sum(result => result.Assists),
                    TotalAssistsPoints = group.Sum(result => result.AssistsPoints),
                    TotalLastHits = group.Sum(result => result.LastHits),
                    TotalLastHitsPoints = group.Sum(result => result.LastHitsPoints),
                    AvgGoldPerMin = group.Where(result => result.Match != null).Select(result => result.GoldPerMin).DefaultIfEmpty().Average(),
                    TotalGoldPerMinPoints = group.Sum(result => result.GoldPerMinPoints),
                    AvgXpPerMin = group.Where(result => result.Match != null).Select(result => result.XpPerMin).DefaultIfEmpty().Average(),
                    TotalXpPerMinPoints = group.Sum(result => result.XpPerMinPoints),
                    TotalMatchFantasyPoints = group.Sum(result => result.TotalMatchFantasyPoints)
                })
                .OrderByDescending(fdp => fdp.TotalMatchFantasyPoints)
                .ToList();
    }

    public IEnumerable<FantasyDraftPointTotals> AggregateFantasyDraftPoints(IEnumerable<FantasyPlayerPoints> fantasyPlayerPoints)
    {
        var distinctPlayers = fantasyPlayerPoints
                .GroupBy(fp => new { fp.FantasyPlayer, fp.Match })
                .Select(group => group.First())
                .ToList();

        List<FantasyPlayerPointTotals> playerTotals = AggregateFantasyPlayerPoints(distinctPlayers).ToList();

        return fantasyPlayerPoints
                .GroupBy(fp => fp.FantasyDraft)
                .Select(group => group.Key)
                .Select(fdp => new FantasyDraftPointTotals
                {
                    FantasyDraft = fdp,
                    IsTeam = _dbContext.Teams.Select(t => t.Id).Any(t => t == fdp.DiscordAccountId),
                    TeamId =  _dbContext.Teams.Select(t => t.Id).Any(t => t == fdp.DiscordAccountId) ?
                        _dbContext.Teams.FirstOrDefault(t => t.Id == fdp.DiscordAccountId)?.Id ?? -1 :
                        null,
                    DiscordName = _dbContext.Teams.Select(t => t.Id).Any(t => t == fdp.DiscordAccountId) ?
                        _dbContext.Teams.FirstOrDefault(t => t.Id == fdp.DiscordAccountId)?.Name ?? "Unknown Team" :
                        _dbContext.DiscordIds.FirstOrDefault(d => d.DiscordId == fdp.DiscordAccountId)?.DiscordName ?? "Unknown User",
                    DraftPickOnePoints =
                        playerTotals.Find(pt =>
                            pt.FantasyPlayer.Id == fdp.DraftPickPlayers
                                                    .Where(dpp => dpp.DraftOrder == 1)
                                                    .Select(dpp => dpp.FantasyPlayerId)
                                                    .FirstOrDefault(0)
                                        )?.TotalMatchFantasyPoints ?? 0M,
                    DraftPickTwoPoints =
                        playerTotals.Find(pt =>
                            pt.FantasyPlayer.Id == fdp.DraftPickPlayers
                                                    .Where(dpp => dpp.DraftOrder == 2)
                                                    .Select(dpp => dpp.FantasyPlayerId)
                                                    .FirstOrDefault(0)
                                        )?.TotalMatchFantasyPoints ?? 0M,
                    DraftPickThreePoints =
                        playerTotals.Find(pt =>
                            pt.FantasyPlayer.Id == fdp.DraftPickPlayers
                                                    .Where(dpp => dpp.DraftOrder == 3)
                                                    .Select(dpp => dpp.FantasyPlayerId)
                                                    .FirstOrDefault(0)
                                        )?.TotalMatchFantasyPoints ?? 0M,
                    DraftPickFourPoints =
                        playerTotals.Find(pt =>
                            pt.FantasyPlayer.Id == fdp.DraftPickPlayers
                                                    .Where(dpp => dpp.DraftOrder == 4)
                                                    .Select(dpp => dpp.FantasyPlayerId)
                                                    .FirstOrDefault(0)
                                        )?.TotalMatchFantasyPoints ?? 0M,
                    DraftPickFivePoints =
                        playerTotals.Find(pt =>
                            pt.FantasyPlayer.Id == fdp.DraftPickPlayers
                                                    .Where(dpp => dpp.DraftOrder == 5)
                                                    .Select(dpp => dpp.FantasyPlayerId)
                                                    .FirstOrDefault(0)
                                        )?.TotalMatchFantasyPoints ?? 0M,
                })
                .OrderByDescending(fdp => fdp.DraftTotalFantasyPoints)
                .ToList();
    }

    public async Task<DateTime> GetLeagueLockedDate(int LeagueId)
    {
        _logger.LogInformation($"Fetching Draft Locked Date for LeagueID: {LeagueId}");
        return DateTimeOffset.FromUnixTimeSeconds(
                await _dbContext.Leagues.Where(l => l.Id == LeagueId).Select(l => l.FantasyDraftLocked).FirstOrDefaultAsync()
            ).DateTime;
    }

    public async Task ClearUserFantasyPlayersAsync(long UserDiscordAccountId, int LeagueId)
    {
        var updateDraft = await _dbContext.FantasyDrafts.Include(fd => fd.DraftPickPlayers).Where(fd => fd.LeagueId == LeagueId && fd.DiscordAccountId == UserDiscordAccountId).FirstOrDefaultAsync();

        if (updateDraft == null)
        {
            return;
        }

        updateDraft.DraftPickPlayers.Clear();
        _dbContext.FantasyDrafts.Update(updateDraft);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task<FantasyDraft> AddNewUserFantasyPlayerAsync(long UserDiscordAccountId, int LeagueId, long? FantasyPlayerId, int DraftOrder)
    {
        // We will receive a 0 if the user wants to clear the draft pick, so we can avoid nulls
        if (DraftOrder > 5 || DraftOrder < 1)
        {
            throw new Exception("Invalid Draft Order, must be between 1 to 5");
        }

        var updateDraft = await _dbContext.FantasyDrafts.Include(fd => fd.DraftPickPlayers).Where(fd => fd.LeagueId == LeagueId && fd.DiscordAccountId == UserDiscordAccountId).FirstOrDefaultAsync();

        if (updateDraft == null)
        {
            // User hasn't created a draft yet, so we'll create that
            updateDraft = new FantasyDraft
            {
                DiscordAccountId = UserDiscordAccountId,
                LeagueId = LeagueId,
                DraftCreated = DateTime.UtcNow,
            };
            _dbContext.FantasyDrafts.Add(updateDraft);
            await _dbContext.SaveChangesAsync();
        }

        if (FantasyPlayerId == null)
        {
            var currentDraftOrder = updateDraft.DraftPickPlayers.Where(dpp => dpp.DraftOrder == DraftOrder).FirstOrDefault();
            if (currentDraftOrder != null)
            {
                updateDraft.DraftPickPlayers.Remove(currentDraftOrder);
            }
        }
        else
        {
            FantasyPlayer fantasyPlayerLookup = await _dbContext.FantasyPlayers.FindAsync(FantasyPlayerId) ?? throw new Exception("Invalid Fantasy Player ID");

            FantasyDraftPlayer? updateFantasyDraftPlayer = await _dbContext.FantasyDrafts
                                            .Where(fd => fd.Id == updateDraft.Id)
                                            .SelectMany(fd => fd.DraftPickPlayers)
                                            .Where(fdp => fdp.DraftOrder == DraftOrder)
                                            .FirstOrDefaultAsync();

            if (updateFantasyDraftPlayer == null)
            {
                // Create FantasyDraftPlayer join table record if it doesn't exist
                updateFantasyDraftPlayer = new FantasyDraftPlayer() { FantasyPlayer = fantasyPlayerLookup, DraftOrder = DraftOrder };
            }
            else
            {
                // Otherwise remove existing draft player lookup from draft pick players
                updateDraft.DraftPickPlayers.Remove(updateFantasyDraftPlayer);
            }

            updateFantasyDraftPlayer.FantasyPlayer = fantasyPlayerLookup;
            updateDraft.DraftPickPlayers.Add(updateFantasyDraftPlayer);
        }

        updateDraft.DraftLastUpdated = DateTime.UtcNow;
        _dbContext.FantasyDrafts.Update(updateDraft);

        await _dbContext.SaveChangesAsync();

        return updateDraft;
    }
    #endregion Fantasy

    #region League
    public async Task<IEnumerable<League>> GetLeaguesAsync(bool? IsActive)
    {
        _logger.LogInformation($"Fetching All Leagues");
        return await _dbContext.Leagues
                .Where(l => IsActive == null || l.IsActive == IsActive)
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
        var leagueStartTime = _dbContext.Leagues.Find(LeagueId)?.LeagueStartTime ?? 0;
        var leagueEndTime = _dbContext.Leagues.Find(LeagueId)?.LeagueEndTime ?? 0;
        return await _dbContext.MatchHistory
            .Where(mh => mh.LeagueId == LeagueId &&
                    mh.StartTime >= leagueStartTime &&
                    mh.StartTime <= leagueEndTime)
            .Include(mh => mh.Players)
            .ToListAsync();
    }

    public async Task<IEnumerable<MatchDetail>> GetMatchDetailsAsync(int LeagueId)
    {
        _logger.LogInformation($"Getting Match Details for League {LeagueId}");
        var league = _dbContext.Leagues.Find(LeagueId);
        var leagueId = league?.LeagueId ?? 0;
        var leagueStartTime = league?.LeagueStartTime ?? 0;
        var leagueEndTime = league?.LeagueEndTime ?? 0;
        return await _dbContext.MatchDetails
            .Where(md => md.LeagueId == leagueId &&
                    md.StartTime >= leagueStartTime &&
                    md.StartTime <= leagueEndTime)
            .Include(md => md.PicksBans)
            .ToListAsync();
    }

    public async Task<MatchDetail?> GetMatchDetailAsync(int LeagueId, long MatchId)
    {
        _logger.LogInformation($"Getting Match Detail for Match: {MatchId} League: {LeagueId}");
        var league = _dbContext.Leagues.Find(LeagueId);
        var leagueId = league?.LeagueId ?? 0;
        var leagueStartTime = league?.LeagueStartTime ?? 0;
        var leagueEndTime = league?.LeagueEndTime ?? 0;
        return await _dbContext.MatchDetails
                .Where(md => md.LeagueId == leagueId && md.MatchId == MatchId &&
                    md.StartTime >= leagueStartTime &&
                    md.StartTime <= leagueEndTime)
                .Include(md => md.PicksBans)
                .Include(md => md.Players).ThenInclude(p => p.AbilityUpgrades)
                .FirstOrDefaultAsync();
    }
    #endregion League

    #region Match
    public async Task<IEnumerable<MatchDetailsPlayer>> GetMatchDetailPlayersByLeagueAsync(int? LeagueId)
    {
        _logger.LogInformation($"Getting Match Details Players for League ID: {LeagueId}");
        var league = _dbContext.Leagues.Find(LeagueId);
        var leagueId = league?.LeagueId ?? 0;
        var leagueStartTime = league?.LeagueStartTime ?? 0;
        var leagueEndTime = league?.LeagueEndTime ?? 0;
        return await _dbContext.MatchDetails
                .Where(md =>
                    (md.LeagueId == leagueId &&
                    md.StartTime >= leagueStartTime &&
                    md.StartTime <= leagueEndTime)
                    || LeagueId == null)
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