using csharp_ef_webapi.Models;
using Microsoft.CodeAnalysis;
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

        _logger.LogDebug($"Fantasy Players by Fantasy League Query: {fantasyPlayerLeagueQuery.ToQueryString()}");

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

        _logger.LogDebug($"Fantasy Drafts by User and Fantasy League Query: {fantasyDraftsUserQuery.ToQueryString()}");

        return await fantasyDraftsUserQuery.ToListAsync();
    }

    public async Task<IEnumerable<FantasyPlayerPoints>> FantasyPlayerPointsByFantasyLeagueAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching Fantasy Points for Fantasy League Id: {FantasyLeagueId}");

        var fantasyPlayerPointsByLeagueQuery = _dbContext.FantasyPlayerPointsView
            .Include(fppv => fppv.FantasyPlayer)
            .Include(fppv => fppv.Match)
            .Where(fpp => fpp.FantasyLeagueId == FantasyLeagueId);

        _logger.LogDebug($"Match Details Query: {fantasyPlayerPointsByLeagueQuery.ToQueryString()}");

        return await fantasyPlayerPointsByLeagueQuery.ToListAsync();
    }

    public async Task<IEnumerable<FantasyPlayerPointTotals>> FantasyPlayerPointTotalsByFantasyLeagueAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching Fantasy Point Totals for Fantasy League Id: {FantasyLeagueId}");

        var fantasyPlayerTotalsQuery = _dbContext.FantasyPlayerPointTotalsView
            .Include(fppt => fppt.FantasyPlayer)
            .Where(fppt => fppt.FantasyLeagueId == FantasyLeagueId)
            .OrderByDescending(fppt => (double)fppt.TotalMatchFantasyPoints); // double casted needed for Sqlite: https://learn.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations

        _logger.LogDebug($"Match Details Query: {fantasyPlayerTotalsQuery.ToQueryString()}");

        return await fantasyPlayerTotalsQuery.ToListAsync();
    }

    public async Task<IEnumerable<FantasyDraftPointTotals>> FantasyDraftPointsByFantasyLeagueAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching Fantasy Points for Fantasy League Id: {FantasyLeagueId}");

        var playerTotals = await _dbContext.FantasyPlayerPointTotalsView
            .Include(fppt => fppt.FantasyPlayer)
            .Where(fppt => fppt.FantasyLeagueId == FantasyLeagueId)
            .OrderByDescending(fppt => fppt.TotalMatchFantasyPoints)
            .ToListAsync();

        var fantasyDraftPointsByLeagueQuery = await _dbContext.FantasyDrafts
            .Where(fl => fl.FantasyLeagueId == FantasyLeagueId)
            .ToListAsync();

        var fantasyDraftPoints = fantasyDraftPointsByLeagueQuery
            .Select(fd => new FantasyDraftPointTotals
            {
                FantasyDraft = fd,
                IsTeam = _dbContext.Teams.Select(t => t.Id).Any(t => t == fd.DiscordAccountId),
                TeamId = _dbContext.Teams.Select(t => t.Id).Any(t => t == fd.DiscordAccountId) ?
                    _dbContext.Teams.FirstOrDefault(t => t.Id == fd.DiscordAccountId)?.Id ?? -1 :
                    null,
                DiscordName = _dbContext.Teams.Select(t => t.Id).Any(t => t == fd.DiscordAccountId) ?
                    _dbContext.Teams.FirstOrDefault(t => t.Id == fd.DiscordAccountId)?.Name ?? "Unknown Team" :
                    _dbContext.DiscordIds.FirstOrDefault(d => d.DiscordId == fd.DiscordAccountId)?.DiscordName ?? "Unknown User",
                FantasyPlayerPoints = playerTotals.Where(pt => fd.DraftPickPlayers.Select(dpp => dpp.FantasyPlayerId).Contains(pt.FantasyPlayer.Id)).ToList()
            })
            .ToList()
            ;

        return fantasyDraftPoints;
    }

    public async Task<FantasyDraftPointTotals?> FantasyDraftPointsByUserLeagueAsync(long UserDiscordAccountId, int FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching Fantasy Points for LeagueID: {FantasyLeagueId}");

        // var fantasyPlayerPoints = await FantasyPlayerPointsByFantasyLeagueAsync(FantasyLeagueId);
        // var playerTotals = AggregateFantasyPlayerPoints(fantasyPlayerPoints).ToList();
        var playerTotals = await _dbContext.FantasyPlayerPointTotalsView
            .Include(fppt => fppt.FantasyPlayer)
            .Where(fppt => fppt.FantasyLeagueId == FantasyLeagueId)
            .OrderByDescending(fppt => fppt.TotalMatchFantasyPoints)
            .ToListAsync();

        var fantasyDraftPointsByUserLeague = await _dbContext.FantasyDrafts
            .Where(fd => fd.FantasyLeagueId == FantasyLeagueId && fd.DiscordAccountId == UserDiscordAccountId)
            .FirstOrDefaultAsync();

        if (fantasyDraftPointsByUserLeague == null)
        {
            return null;
        }
        else
        {
            var fantasyDraftPoints = new FantasyDraftPointTotals
            {
                FantasyDraft = fantasyDraftPointsByUserLeague,
                IsTeam = _dbContext.Teams.Select(t => t.Id).Any(t => t == fantasyDraftPointsByUserLeague.DiscordAccountId),
                TeamId = _dbContext.Teams.Select(t => t.Id).Any(t => t == fantasyDraftPointsByUserLeague.DiscordAccountId) ?
                        _dbContext.Teams.FirstOrDefault(t => t.Id == fantasyDraftPointsByUserLeague.DiscordAccountId)?.Id ?? -1 :
                        null,
                DiscordName = _dbContext.Teams.Select(t => t.Id).Any(t => t == fantasyDraftPointsByUserLeague.DiscordAccountId) ?
                        _dbContext.Teams.FirstOrDefault(t => t.Id == fantasyDraftPointsByUserLeague.DiscordAccountId)?.Name ?? "Unknown Team" :
                        _dbContext.DiscordIds.FirstOrDefault(d => d.DiscordId == fantasyDraftPointsByUserLeague.DiscordAccountId)?.DiscordName ?? "Unknown User",
                FantasyPlayerPoints = playerTotals.Where(pt => fantasyDraftPointsByUserLeague.DraftPickPlayers.Select(dpp => dpp.FantasyPlayerId).Contains(pt.FantasyPlayer.Id)).ToList()
            };

            return fantasyDraftPoints;
        }
    }

    public async Task<IEnumerable<MetadataSummary>> AggregateMetadataAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Fetching Aggregate Metadata for Fantasy League Id: {FantasyLeagueId}");

        var metadataQuery = _dbContext.Leagues
                .SelectMany(
                    l => l.FantasyLeagues,
                    (left, right) => new { League = left, FantasyLeague = right }
                )
                .SelectMany(
                    l => l.League.MatchDetails,
                    (left, right) => new { left.League, left.FantasyLeague, MatchDetail = right }
                )
                //Matchdetail joins
                .SelectMany(mm => mm.MatchDetail.Players,
                    (left, right) => new { left.FantasyLeague, MatchDetail = left.MatchDetail, MatchDetailPlayer = right }
                )
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                //Metadata joins
                .SelectMany(mm => mm.MatchDetail.MatchMetadata.Teams,
                    (left, right) => new { left.FantasyLeague, MatchDetail = left.MatchDetail, MatchDetailPlayer = left.MatchDetailPlayer, MatchMetadataTeam = right }
                )
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                .SelectMany(mm => mm.MatchMetadataTeam.Players,
                    (left, right) => new { left.FantasyLeague, MatchDetail = left.MatchDetail, MatchDetailPlayer = left.MatchDetailPlayer, MatchMetadataPlayer = right }
                )
                .SelectMany(mm => mm.FantasyLeague.FantasyPlayers
                    .Where(fp => fp.DotaAccountId == mm.MatchDetailPlayer.AccountId),
                    (left, right) => new { left.FantasyLeague, MatchDetail = left.MatchDetail, MatchMetadataPlayer = left.MatchMetadataPlayer, MatchDetailPlayer = left.MatchDetailPlayer, FantasyPlayer = right }
                )
                .Where(l => l.FantasyLeague.Id == FantasyLeagueId)
                .Where(l =>
                    l.MatchDetail.StartTime >= l.FantasyLeague.LeagueStartTime &&
                    l.MatchDetail.StartTime <= l.FantasyLeague.LeagueEndTime
                )
                .Where(mp => mp.MatchMetadataPlayer.PlayerSlot == mp.MatchDetailPlayer.PlayerSlot)
                .OrderBy(m => m.MatchDetail.MatchId)
                .ThenBy(m => m.FantasyPlayer.Id)
                .GroupBy(fpm => fpm.FantasyPlayer);

        _logger.LogDebug($"Fantasy Metadata Query: {metadataQuery.ToQueryString()}");

        var metadataQueryList = await metadataQuery.ToListAsync();

        // When we do the select to new MetadataSummary before resolving the group list it destroys the includes, not sure why

        var aggregateMetadataQuery = metadataQueryList
                .Select(
                    final => new MetadataSummary
                    {
                        Player = new FantasyPlayer
                        {
                            DotaAccount = final.Key.DotaAccount,
                            DotaAccountId = final.Key.DotaAccountId,
                            FantasyLeague = new FantasyLeague(), // Need to omit this to avoid circular reference
                            FantasyLeagueId = final.Key.FantasyLeagueId,
                            Id = final.Key.Id,
                            Team = final.Key.Team,
                            TeamId = final.Key.TeamId,
                            TeamPosition = final.Key.TeamPosition
                        },
                        MatchesPlayed = final.Count(),
                        // Match Details
                        Kills = final.Sum(result => result.MatchDetailPlayer.Kills),
                        KillsAverage = final.Average(result => result.MatchDetailPlayer.Kills),
                        Deaths = final.Sum(result => result.MatchDetailPlayer.Deaths),
                        DeathsAverage = final.Average(result => result.MatchDetailPlayer.Deaths),
                        Assists = final.Sum(result => result.MatchDetailPlayer.Assists),
                        AssistsAverage = final.Average(result => result.MatchDetailPlayer.Assists),
                        LastHits = final.Sum(result => result.MatchDetailPlayer.LastHits),
                        LastHitsAverage = final.Average(result => result.MatchDetailPlayer.LastHits),
                        Denies = final.Sum(result => result.MatchDetailPlayer.Denies),
                        DeniesAverage = final.Average(result => result.MatchDetailPlayer.Denies),
                        GoldPerMin = (int?)final.Average(result => result.MatchDetailPlayer.GoldPerMin),
                        GoldPerMinAverage = (int?)final.Average(result => result.MatchDetailPlayer.GoldPerMin),
                        XpPerMin = (int?)final.Average(result => result.MatchDetailPlayer.XpPerMin),
                        XpPerMinAverage = (int?)final.Average(result => result.MatchDetailPlayer.XpPerMin),
                        Networth = final.Sum(result => result.MatchDetailPlayer.Networth),
                        NetworthAverage = final.Average(result => result.MatchDetailPlayer.Networth),
                        HeroDamage = final.Sum(result => result.MatchDetailPlayer.HeroDamage),
                        HeroDamageAverage = final.Average(result => result.MatchDetailPlayer.HeroDamage),
                        TowerDamage = final.Sum(result => result.MatchDetailPlayer.TowerDamage),
                        TowerDamageAverage = final.Average(result => result.MatchDetailPlayer.TowerDamage),
                        HeroHealing = final.Sum(result => result.MatchDetailPlayer.HeroHealing),
                        HeroHealingAverage = final.Average(result => result.MatchDetailPlayer.HeroHealing),
                        Gold = final.Sum(result => result.MatchDetailPlayer.Gold),
                        GoldAverage = final.Average(result => result.MatchDetailPlayer.Gold),
                        ScaledHeroDamage = final.Sum(result => result.MatchDetailPlayer.ScaledHeroDamage),
                        ScaledHeroDamageAverage = final.Average(result => result.MatchDetailPlayer.ScaledHeroDamage),
                        ScaledTowerDamage = final.Sum(result => result.MatchDetailPlayer.ScaledTowerDamage),
                        ScaledTowerDamageAverage = final.Average(result => result.MatchDetailPlayer.ScaledTowerDamage),
                        ScaledHeroHealing = final.Sum(result => result.MatchDetailPlayer.ScaledHeroHealing),
                        ScaledHeroHealingAverage = final.Average(result => result.MatchDetailPlayer.ScaledHeroHealing),
                        // Metadata Player
                        WinStreak = (uint)final.Sum(result => (int)result.MatchMetadataPlayer.WinStreak),
                        BestWinStreak = (uint)final.Sum(result => (int)result.MatchMetadataPlayer.BestWinStreak),
                        FightScore = final.Sum(result => result.MatchMetadataPlayer.FightScore),
                        FarmScore = final.Sum(result => result.MatchMetadataPlayer.FarmScore),
                        SupportScore = final.Sum(result => result.MatchMetadataPlayer.SupportScore),
                        PushScore = final.Sum(result => result.MatchMetadataPlayer.PushScore),
                        HeroXp = (uint)final.Sum(result => (int)result.MatchMetadataPlayer.HeroXp),
                        CampsStacked = (uint)final.Sum(result => (int)result.MatchMetadataPlayer.CampsStacked),
                        Rampages = (uint)final.Sum(result => (int)result.MatchMetadataPlayer.Rampages),
                        TripleKills = (uint)final.Sum(result => (int)result.MatchMetadataPlayer.TripleKills),
                        AegisSnatched = (uint)final.Sum(result => (int)result.MatchMetadataPlayer.AegisSnatched),
                        RapiersPurchased = (uint)final.Sum(result => (int)result.MatchMetadataPlayer.RapiersPurchased),
                        CouriersKilled = (uint)final.Sum(result => (int)result.MatchMetadataPlayer.CouriersKilled),
                        NetworthRank = (uint)final.Sum(result => (int)result.MatchMetadataPlayer.NetworthRank),
                        SupportGoldSpent = (uint)final.Sum(result => (int)result.MatchMetadataPlayer.SupportGoldSpent),
                        ObserverWardsPlaced = (uint)final.Sum(result => (int)result.MatchMetadataPlayer.ObserverWardsPlaced),
                        SentryWardsPlaced = (uint)final.Sum(result => (int)result.MatchMetadataPlayer.SentryWardsPlaced),
                        WardsDewarded = (uint)final.Sum(result => (int)result.MatchMetadataPlayer.WardsDewarded),
                        StunDuration = final.Sum(result => result.MatchMetadataPlayer.StunDuration),
                        FightScoreAverage = final.Average(result => result.MatchMetadataPlayer.FightScore),
                        FarmScoreAverage = final.Average(result => result.MatchMetadataPlayer.FarmScore),
                        SupportScoreAverage = final.Average(result => result.MatchMetadataPlayer.SupportScore),
                        PushScoreAverage = final.Average(result => result.MatchMetadataPlayer.PushScore),
                        HeroXpAverage = final.Average(result => (int)result.MatchMetadataPlayer.HeroXp),
                        CampsStackedAverage = final.Average(result => (int)result.MatchMetadataPlayer.CampsStacked),
                        RampagesAverage = final.Average(result => (int)result.MatchMetadataPlayer.Rampages),
                        TripleKillsAverage = final.Average(result => (int)result.MatchMetadataPlayer.TripleKills),
                        AegisSnatchedAverage = final.Average(result => (int)result.MatchMetadataPlayer.AegisSnatched),
                        RapiersPurchasedAverage = final.Average(result => (int)result.MatchMetadataPlayer.RapiersPurchased),
                        CouriersKilledAverage = final.Average(result => (int)result.MatchMetadataPlayer.CouriersKilled),
                        NetworthRankAverage = final.Average(result => (int)result.MatchMetadataPlayer.NetworthRank),
                        SupportGoldSpentAverage = final.Average(result => (int)result.MatchMetadataPlayer.SupportGoldSpent),
                        ObserverWardsPlacedAverage = final.Average(result => (int)result.MatchMetadataPlayer.ObserverWardsPlaced),
                        SentryWardsPlacedAverage = final.Average(result => (int)result.MatchMetadataPlayer.SentryWardsPlaced),
                        WardsDewardedAverage = final.Average(result => (int)result.MatchMetadataPlayer.WardsDewarded),
                        StunDurationAverage = final.Average(result => result.MatchMetadataPlayer.StunDuration)
                    }
                );

        return aggregateMetadataQuery.ToList();
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

        _logger.LogDebug($"Match History SQL Query: {matchHistoryQuery.ToQueryString()}");

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

        _logger.LogDebug($"Get Match Detail Query: {matchDetailsQuery.ToQueryString()}");

        return await matchDetailsQuery.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<GcMatchMetadata>> GetLeagueMetadataAsync(int LeagueId)
    {
        _logger.LogInformation($"Getting Match Metadata for League {LeagueId}");

        var leagueMetadataQuery = QueryLeagueMatchDetails(LeagueId)
            .Where(md => md.MatchMetadata != null)
            .Select(l => l.MatchMetadata ?? new GcMatchMetadata())
            .OrderByDescending(md => md.MatchId);

        _logger.LogDebug($"Match Metadata SQL Query: {leagueMetadataQuery.ToQueryString()}");

        return await leagueMetadataQuery.ToListAsync();
    }

    public async Task<IEnumerable<GcMatchMetadata>> GetLeagueMetadataAsync(int LeagueId, int Skip = 0, int Take = 50)
    {
        _logger.LogInformation($"Getting Match Metadata for League {LeagueId}");

        var leagueMetadataQuery = QueryLeagueMatchDetails(LeagueId)
            .Where(md => md.MatchMetadata != null)
            .Select(l => l.MatchMetadata ?? new GcMatchMetadata())
            .OrderByDescending(md => md.MatchId)
            .Skip(Skip)
            .Take(Take);

        _logger.LogDebug($"Match Metadata SQL Query: {leagueMetadataQuery.ToQueryString()}");

        return await leagueMetadataQuery.ToListAsync();
    }

    public async Task<IEnumerable<GcMatchMetadata>> GetFantasyLeagueMetadataAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Getting Match Metadata for Fantasy League {FantasyLeagueId}");

        var fantasyLeagueMetadataQuery = await QueryFantasyLeagueMatchDetails(FantasyLeagueId)
            .Where(md => md.MatchMetadata != null)
            .OrderByDescending(md => md.MatchId)
            .ToListAsync();

        var filtered = fantasyLeagueMetadataQuery
            .Select(l => l.MatchMetadata ?? new GcMatchMetadata())
            .ToList();

        return filtered;
    }

    public async Task<IEnumerable<GcMatchMetadata>> GetFantasyLeagueMetadataAsync(int FantasyLeagueId, int Skip = 0, int Take = 50)
    {
        _logger.LogInformation($"Getting Match Metadata for Fantasy League {FantasyLeagueId}");

        var fantasyLeagueMetadataQuery = QueryFantasyLeagueMatchDetails(FantasyLeagueId)
            .Where(md => md.MatchMetadata != null)
            .Select(md => md.MatchMetadata ?? new GcMatchMetadata()) // This should never be null but using it to suppress warning
            .OrderByDescending(md => md.MatchId)
            .Skip(Skip)
            .Take(Take);

        _logger.LogDebug($"Match Metadata SQL Query: {fantasyLeagueMetadataQuery.ToQueryString()}");

        return await fantasyLeagueMetadataQuery.ToListAsync();
    }

    public async Task<GcMatchMetadata?> GetMatchMetadataAsync(long MatchId)
    {
        _logger.LogInformation($"Getting Match Metadata for Match: {MatchId}");

        var matchMetadataQuery = _dbContext.GcMatchMetadata
                .Where(md => md.MatchId == MatchId);

        _logger.LogDebug($"Match Metadata Query: {matchMetadataQuery.ToQueryString()}");

        return await matchMetadataQuery.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<MatchHighlights>> GetLastNMatchHighlights(int FantasyLeagueId, int MatchCount)
    {
        _logger.LogInformation($"Getting {MatchCount} Match Highlights for Fantasy League ID: {FantasyLeagueId}");

        var matchHighlightsQuery = _dbContext.MatchHighlightsView
                .Include(mhv => mhv.FantasyPlayer)
                .Where(mhv => mhv.FantasyLeagueId == FantasyLeagueId)
                .OrderByDescending(mhv => mhv.StartTime)
                .Take(MatchCount);

        _logger.LogDebug($"Match Highlights Query: {matchHighlightsQuery.ToQueryString()}");

        return await matchHighlightsQuery.ToListAsync();
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

    public async Task<IEnumerable<FantasyNormalizedAverages>> GetFantasyNormalizedAveragesAsync(long FantasyPlayerId)
    {
        _logger.LogInformation($"Getting Player Averages");

        return await _dbContext.FantasyNormalizedAveragesView
                .Where(fnp => fnp.FantasyPlayerId == FantasyPlayerId)
                .ToListAsync();
    }

    public async Task<FantasyPlayerTopHeroes> GetFantasyPlayerTopHeroesAsync(long FantasyPlayerId)
    {
        _logger.LogInformation($"Getting Player Averages");

        FantasyPlayer? fantasyPlayer = await _dbContext.FantasyPlayers.FindAsync(FantasyPlayerId);

        if (fantasyPlayer == null)
        {
            // No player found
            return new FantasyPlayerTopHeroes();
        }

        var heroes = await GetHeroesAsync();

        var topHeroIds = await _dbContext.MatchDetailsPlayers
                .Where(mdp => mdp.AccountId == fantasyPlayer.DotaAccountId)
                .GroupBy(match => match.HeroId)
                .Select(group => new
                {
                    HeroId = group.Key,
                    Count = group.Count()
                })
                .OrderByDescending(group => group.Count)
                .Take(3)
                .ToArrayAsync();

        var topHeroes = topHeroIds.Select(thi => new TopHeroCount
        {
            Hero = heroes.First(h => h.Id == thi.HeroId),
            Count = thi.Count
        })
        .ToArray();

        return new FantasyPlayerTopHeroes
        {
            FantasyPlayer = fantasyPlayer,
            FantasyPlayerId = fantasyPlayer.Id,
            TopHeroes = topHeroes
        };
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

    #region Discord
    public async Task<DiscordIds?> GetDiscordIdAsync(long GetDiscordId)
    {
        _logger.LogInformation($"Getting Discord User {GetDiscordId}");

        return await _dbContext.DiscordIds.Where(di => di.DiscordId == GetDiscordId).FirstOrDefaultAsync();
    }
    #endregion Discord
}