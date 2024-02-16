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

    public async Task<IEnumerable<FantasyLeague>> GetFantasyLeaguesAsync(bool? IsActive)
    {
        _logger.LogInformation($"Fetching All Fantasy Leagues");

        return await _dbContext.FantasyLeagues
                .Where(l => IsActive == null || l.IsActive == IsActive)
                .ToListAsync();
    }

    public async Task<FantasyLeague?> GetFantasyLeagueAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching Single Fantasy League {FantasyLeagueId}");

        return await _dbContext.FantasyLeagues.FindAsync(FantasyLeagueId);
    }

    public async Task<IEnumerable<FantasyPlayer>> FantasyPlayersByFantasyLeagueAsync(int? FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching Fantasy Players Fantasy League Id: {FantasyLeagueId}");

        var fantasyPlayerLeagueQuery = _dbContext.FantasyPlayers
            .Include(fp => fp.Team)
            .Include(fp => fp.DotaAccount)
            .Where(fp => fp.FantasyLeagueId == FantasyLeagueId || FantasyLeagueId == null)
            .OrderBy(fp => fp.Team.Name)
                .ThenBy(fp => fp.DotaAccount.Name);

        _logger.LogInformation($"Fantasy Players by Fantasy League Query: {fantasyPlayerLeagueQuery.ToQueryString()}");

        return await fantasyPlayerLeagueQuery.ToListAsync();
    }

    public async Task<IEnumerable<FantasyDraft>> FantasyDraftsByUserLeagueAsync(long UserDiscordAccountId, int FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching User {UserDiscordAccountId} Fantasy Draft for Fantasy League Id: {FantasyLeagueId}");

        var fantasyDraftsUserQuery = _dbContext.FantasyDrafts
            .Where(fd => fd.FantasyLeagueId == FantasyLeagueId && fd.DiscordAccountId == UserDiscordAccountId)
            .Include(fd => fd.DraftPickPlayers)
                .ThenInclude(fp => fp.FantasyPlayer)
                .ThenInclude(fp => fp.Team)
            .Include(fd => fd.DraftPickPlayers)
                .ThenInclude(fp => fp.FantasyPlayer)
                .ThenInclude(fp => fp.DotaAccount);

        _logger.LogInformation($"Fantasy Drafts by User and Fantasy League Query: {fantasyDraftsUserQuery.ToQueryString()}");

        return await fantasyDraftsUserQuery.ToListAsync();
    }

    public async Task<IEnumerable<FantasyPlayerPoints>> FantasyPlayerPointsByFantasyLeagueAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching Fantasy Points for Fantasy League Id: {FantasyLeagueId}");

        var fantasyPlayerPointsByLeagueQuery = _dbContext.Leagues
            .SelectMany(
                l => l.FantasyLeagues,
                (left, right) => new { League = left, FantasyLeague = right })
            .SelectMany(
                l => l.FantasyLeague.FantasyPlayers,
                (left, right) => new { League = left.League, FantasyLeague = left.FantasyLeague, FantasyPlayer = right }
            )
            // LEFT JOIN
            .SelectMany(
                fdp => fdp.League.MatchDetails
                    .SelectMany(md => md.Players,
                        (left, right) => new { MatchDetail = left, MatchDetailPlayer = right }
                    )
                    .Where(md => md.MatchDetailPlayer.AccountId == fdp.FantasyPlayer.DotaAccountId &&
                        md.MatchDetail.StartTime >= fdp.FantasyLeague.LeagueStartTime &&
                        md.MatchDetail.StartTime <= fdp.FantasyLeague.LeagueEndTime)
                    .DefaultIfEmpty(),
                (left, right) => new
                {
                    FantasyLeague = left.FantasyLeague,
                    FantasyPlayer = left.FantasyPlayer,
                    MatchInfo = right
                }
            )
            .Where(fdp => fdp.FantasyLeague.Id == FantasyLeagueId)
            .Distinct()
            .Select(fpm => new FantasyPlayerPoints
            {
                FantasyDraft = new FantasyDraft(),
                Match = fpm.MatchInfo != null ? fpm.MatchInfo.MatchDetailPlayer : null,
                FantasyPlayer = fpm.FantasyPlayer
            });

        _logger.LogInformation($"Match Details Query: {fantasyPlayerPointsByLeagueQuery.ToQueryString()}");

        return await fantasyPlayerPointsByLeagueQuery.ToListAsync();
    }

    public async Task<IEnumerable<FantasyPlayerPoints>> FantasyDraftPointsByFantasyLeagueAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching Fantasy Points for Fantasy League Id: {FantasyLeagueId}");

        var fantasyDraftPointsByLeagueQuery = _dbContext.Leagues
            .SelectMany(
                l => l.FantasyLeagues,
                (left, right) => new { League = left, FantasyLeague = right })
            .SelectMany(
                l => l.FantasyLeague.FantasyDrafts,
                (left, right) => new { League = left.League, FantasyLeague = left.FantasyLeague, FantasyDraft = right }
            )
            .SelectMany(
                l => l.FantasyDraft.DraftPickPlayers,
                (left, right) => new { League = left.League, FantasyLeague = left.FantasyLeague, FantasyDraft = left.FantasyDraft, DraftPick = right, FantasyPlayer = right.FantasyPlayer }
            )
            // LEFT JOIN
            .SelectMany(
                fdp => fdp.League.MatchDetails
                    .SelectMany(md => md.Players,
                        (left, right) => new { MatchDetail = left, MatchDetailPlayer = right }
                    )
                    .Where(md => md.MatchDetailPlayer.AccountId == fdp.FantasyPlayer.DotaAccountId &&
                        md.MatchDetail.StartTime >= fdp.FantasyLeague.LeagueStartTime &&
                        md.MatchDetail.StartTime <= fdp.FantasyLeague.LeagueEndTime)
                    .DefaultIfEmpty(),
                (left, right) => new { FantasyLeague = left.FantasyLeague, FantasyDraft = left.FantasyDraft, FantasyPlayer = left.FantasyPlayer, MatchInfo = right }
            )
            .Where(fdp => fdp.FantasyLeague.Id == FantasyLeagueId)
            .Distinct()
            .Select(
                fdp => new FantasyPlayerPoints
                {
                    FantasyDraft = fdp.FantasyDraft,
                    Match = fdp.MatchInfo != null ? fdp.MatchInfo.MatchDetailPlayer : null,
                    FantasyPlayer = fdp.FantasyPlayer
                }
            );

        _logger.LogInformation($"Fantasy Draft Points by Fantasy League Query: {fantasyDraftPointsByLeagueQuery.ToQueryString()}");

        return await fantasyDraftPointsByLeagueQuery.ToListAsync();
    }

    public async Task<IEnumerable<FantasyPlayerPoints>> FantasyDraftPointsByUserLeagueAsync(long UserDiscordAccountId, int FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching Fantasy Points for LeagueID: {FantasyLeagueId}");

        var fantasyDraftPointsByUserLeagueQuery = _dbContext.Leagues
            .SelectMany(
                l => l.FantasyLeagues,
                (left, right) => new { League = left, FantasyLeague = right })
            .SelectMany(
                l => l.FantasyLeague.FantasyDrafts,
                (left, right) => new { League = left.League, FantasyLeague = left.FantasyLeague, FantasyDraft = right }
            )
            .SelectMany(
                l => l.FantasyDraft.DraftPickPlayers,
                (left, right) => new { League = left.League, FantasyLeague = left.FantasyLeague, FantasyDraft = left.FantasyDraft, DraftPick = right, FantasyPlayer = right.FantasyPlayer }
            )
            // LEFT JOIN
            .SelectMany(
                fdp => fdp.League.MatchDetails
                    .SelectMany(md => md.Players,
                        (left, right) => new { MatchDetail = left, MatchDetailPlayer = right }
                    )
                    .Where(md => md.MatchDetailPlayer.AccountId == fdp.FantasyPlayer.DotaAccountId &&
                        md.MatchDetail.StartTime >= fdp.FantasyLeague.LeagueStartTime &&
                        md.MatchDetail.StartTime <= fdp.FantasyLeague.LeagueEndTime)
                    .DefaultIfEmpty(),
                (left, right) => new { FantasyLeague = left.FantasyLeague, FantasyDraft = left.FantasyDraft, FantasyPlayer = left.FantasyPlayer, MatchInfo = right }
            )
            .Where(fdp => fdp.FantasyLeague.Id == FantasyLeagueId && fdp.FantasyDraft.DiscordAccountId == UserDiscordAccountId)
            .Distinct()
            .Select(
                fdp => new FantasyPlayerPoints
                {
                    FantasyDraft = fdp.FantasyDraft,
                    Match = fdp.MatchInfo != null ? fdp.MatchInfo.MatchDetailPlayer : null,
                    FantasyPlayer = fdp.FantasyPlayer
                }
            );

        _logger.LogInformation($"Fantasy Draft Points by Fantasy User League: {fantasyDraftPointsByUserLeagueQuery.ToQueryString()}");

        return await fantasyDraftPointsByUserLeagueQuery.ToListAsync();
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
                          TeamId = _dbContext.Teams.Select(t => t.Id).Any(t => t == fdp.DiscordAccountId) ?
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

    public async Task<DateTime> GetLeagueLockedDate(int FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching Draft Locked Date for Fantasy League Id: {FantasyLeagueId}");

        return DateTimeOffset.FromUnixTimeSeconds(
                await _dbContext.FantasyLeagues.Where(l => l.Id == FantasyLeagueId).Select(l => l.FantasyDraftLocked).FirstOrDefaultAsync()
            ).DateTime;
    }

    public async Task ClearUserFantasyPlayersAsync(long UserDiscordAccountId, int FantasyLeagueId)
    {
        var updateDraft = await _dbContext.FantasyDrafts
            .Include(fd => fd.DraftPickPlayers)
            .Where(fd => fd.FantasyLeagueId == FantasyLeagueId && fd.DiscordAccountId == UserDiscordAccountId)
            .FirstOrDefaultAsync();

        if (updateDraft == null)
        {
            return;
        }

        updateDraft.DraftPickPlayers.Clear();
        _dbContext.FantasyDrafts.Update(updateDraft);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task<FantasyDraft> AddNewUserFantasyPlayerAsync(long UserDiscordAccountId, int FantasyLeagueId, long? FantasyPlayerId, int DraftOrder)
    {
        // We will receive a 0 if the user wants to clear the draft pick, so we can avoid nulls
        if (DraftOrder > 5 || DraftOrder < 1)
        {
            throw new Exception("Invalid Draft Order, must be between 1 to 5");
        }

        var updateDraft = await _dbContext.FantasyDrafts
            .Include(fd => fd.DraftPickPlayers)
            .Where(fd => fd.FantasyLeagueId == FantasyLeagueId && fd.DiscordAccountId == UserDiscordAccountId)
            .FirstOrDefaultAsync();

        if (updateDraft == null)
        {
            // User hasn't created a draft yet, so we'll create that
            updateDraft = new FantasyDraft
            {
                DiscordAccountId = UserDiscordAccountId,
                FantasyLeagueId = FantasyLeagueId,
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

    public async Task<IEnumerable<MatchHistory>> GetMatchHistoryByFantasyLeagueAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Getting Match History for Fantasy League Id: {FantasyLeagueId}");

        var matchHistoryQuery = _dbContext.Leagues
            .SelectMany(
                l => l.MatchHistories,
                (left, right) => new { League = left, MatchHistory = right }
            )
            .SelectMany(
                l => l.League.FantasyLeagues,
                (left, right) => new { League = left.League, MatchHistory = left.MatchHistory, FantasyLeague = right }
            )
            .Where(l => l.FantasyLeague.Id == FantasyLeagueId &&
                    l.MatchHistory.StartTime >= l.FantasyLeague.LeagueStartTime &&
                    l.MatchHistory.StartTime <= l.FantasyLeague.LeagueEndTime)
            .Select(l => l.MatchHistory)
            .Include(mh => mh.Players);

        _logger.LogInformation($"Match History SQL Query: {matchHistoryQuery.ToQueryString()}");

        return await matchHistoryQuery.ToListAsync();
    }
    #endregion League

    #region Match
    public async Task<IEnumerable<MatchDetailsPlayer>> GetMatchDetailPlayersByLeagueAsync(int? LeagueId)
    {
        _logger.LogInformation($"Getting Match Details Players for League ID: {LeagueId}");

        var matchDetailPlayerLeagueQuery = QueryLeagueMatchDetails(LeagueId)
            .SelectMany(md => md.Players)
            .Where(p => p.LeaverStatus != 1); // Filter out games players left (typically false starts)

        return await matchDetailPlayerLeagueQuery.ToListAsync();
    }

    public async Task<IEnumerable<MatchDetail>> GetMatchDetailsByFantasyLeagueAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Getting Match Details for Fantasy League {FantasyLeagueId}");

        var leagueMatchDetailsQuery = QueryFantasyLeagueMatchDetails(FantasyLeagueId)
            .Include(md => md.PicksBans);

        return await leagueMatchDetailsQuery.ToListAsync();
    }

    public async Task<MatchDetail?> GetMatchDetailAsync(long MatchId)
    {
        _logger.LogInformation($"Getting Match Detail for Match: {MatchId}");

        var matchDetailsQuery = _dbContext.MatchDetails
                .Where(md => md.MatchId == MatchId)
                .Include(md => md.PicksBans)
                .Include(md => md.Players).ThenInclude(p => p.AbilityUpgrades);

        _logger.LogInformation($"Get Match Detail Query: {matchDetailsQuery.ToQueryString()}");

        return await matchDetailsQuery.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<GcMatchMetadata>> GetLeagueMetadataAsync(int LeagueId)
    {
        _logger.LogInformation($"Getting Match Metadata for League {LeagueId}");

        var leagueMetadataQuery = QueryLeagueMatchDetails(LeagueId)
            .Where(md => md.MatchMetadata != null)
            .Select(l => l.MatchMetadata ?? new GcMatchMetadata())
            .Include(md => md.Teams)
                .ThenInclude(mdt => mdt.Players)
                .ThenInclude(mdp => mdp.Kills)
            .Include(md => md.MatchTips)
            .OrderByDescending(md => md.MatchId);

        _logger.LogInformation($"Match Metadata SQL Query: {leagueMetadataQuery.ToQueryString()}");

        return await leagueMetadataQuery.ToListAsync();
    }

    public async Task<IEnumerable<GcMatchMetadata>> GetLeagueMetadataAsync(int LeagueId, int Skip = 0, int Take = 50)
    {
        _logger.LogInformation($"Getting Match Metadata for League {LeagueId}");

        var leagueMetadataQuery = QueryLeagueMatchDetails(LeagueId)
            .Where(md => md.MatchMetadata != null)
            .Select(l => l.MatchMetadata ?? new GcMatchMetadata())
            .Include(md => md.Teams)
                .ThenInclude(mdt => mdt.Players)
                .ThenInclude(mdp => mdp.Kills)
            .Include(md => md.MatchTips)
            .OrderByDescending(md => md.MatchId)
            .Skip(Skip)
            .Take(Take);

        _logger.LogInformation($"Match Metadata SQL Query: {leagueMetadataQuery.ToQueryString()}");

        return await leagueMetadataQuery.ToListAsync();
    }

    public async Task<IEnumerable<GcMatchMetadata>> GetFantasyLeagueMetadataAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Getting Match Metadata for Fantasy League {FantasyLeagueId}");

        var fantasyLeagueMetadataQuery = QueryFantasyLeagueMatchDetails(FantasyLeagueId)
            .Where(md => md.MatchMetadata != null)
            .Select(l => l.MatchMetadata ?? new GcMatchMetadata())
            .Include(md => md.Teams)
                .ThenInclude(mdt => mdt.Players)
                .ThenInclude(mdp => mdp.Kills)
            .Include(md => md.MatchTips)
            .OrderByDescending(md => md.MatchId);

        _logger.LogInformation($"Match Metadata SQL Query: {fantasyLeagueMetadataQuery.ToQueryString()}");

        return await fantasyLeagueMetadataQuery.ToListAsync();
    }

    public async Task<IEnumerable<GcMatchMetadata>> GetFantasyLeagueMetadataAsync(int FantasyLeagueId, int Skip = 0, int Take = 50)
    {
        _logger.LogInformation($"Getting Match Metadata for Fantasy League {FantasyLeagueId}");

        var fantasyLeagueMetadataQuery = QueryFantasyLeagueMatchDetails(FantasyLeagueId)
            .Where(md => md.MatchMetadata != null)
            .Select(md => md.MatchMetadata ?? new GcMatchMetadata()) // This should never be null but using it to suppress warning
            .Include(md => md.Teams)
                .ThenInclude(mdt => mdt.Players)
                .ThenInclude(mdp => mdp.Kills)
            .Include(md => md.MatchTips)
            .OrderByDescending(md => md.MatchId)
            .Skip(Skip)
            .Take(Take);

        _logger.LogInformation($"Match Metadata SQL Query: {fantasyLeagueMetadataQuery.ToQueryString()}");

        return await fantasyLeagueMetadataQuery.ToListAsync();
    }

    public async Task<GcMatchMetadata?> GetMatchMetadataAsync(long MatchId)
    {
        _logger.LogInformation($"Getting Match Metadata for Match: {MatchId}");

        var matchMetadataQuery = _dbContext.GcMatchMetadata
                .Where(md => md.MatchId == MatchId)
                .Include(md => md.Teams)
                    .ThenInclude(mdt => mdt.Players)
                    .ThenInclude(mdp => mdp.Kills)
                .Include(md => md.MatchTips);

        _logger.LogInformation($"Match Metadata Query: {matchMetadataQuery.ToQueryString()}");

        return await matchMetadataQuery.FirstOrDefaultAsync();
    }

    private IQueryable<MatchDetail> QueryLeagueMatchDetails(int? LeagueId)
    {
        // This logic is awkward so I'd rather do it all once here and return a Queryable
        var leagueMatchesQuery = _dbContext.Leagues
            .SelectMany(
                l => l.MatchDetails,
                (left, right) => new { League = left, MatchDetail = right }
            )
            .Where(l => l.League.Id == LeagueId || LeagueId == null)
            .Select(l => l.MatchDetail);

        return leagueMatchesQuery;
    }

    private IQueryable<MatchDetail> QueryFantasyLeagueMatchDetails(int FantasyLeagueId)
    {
        // This logic is awkward so I'd rather do it all once here and return a Queryable
        var fantasyLeagueMatchesQuery = _dbContext.Leagues
            .SelectMany(
                l => l.FantasyLeagues,
                (left, right) => new { League = left, FantasyLeague = right }
            )
            .SelectMany(
                l => l.League.MatchDetails,
                (left, right) => new { left.League, left.FantasyLeague, MatchDetail = right }
            )
            .Where(l => l.FantasyLeague.Id == FantasyLeagueId)
            .Where(l =>
                l.MatchDetail.StartTime >= l.FantasyLeague.LeagueStartTime &&
                l.MatchDetail.StartTime <= l.FantasyLeague.LeagueEndTime
            )
            .Select(l => l.MatchDetail);

        return fantasyLeagueMatchesQuery;
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