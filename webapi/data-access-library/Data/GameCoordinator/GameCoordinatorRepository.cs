namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models.GameCoordinator;
using SteamKit2.GC.Dota.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DataAccessLibrary.Models.Fantasy;

/// <summary>
/// Service to fetch and transform the Postgres data to read/write fantasy draft data for the webapi
/// example is to fetch all of the current scores, vs adding a new draft. Controllers should handle none of the business
/// logic 
/// </summary>
public class GameCoordinatorRepository : IGameCoordinatorRepository
{
    private readonly ILogger<GameCoordinatorRepository> _logger;
    private AghanimsFantasyContext _dbContext;
    public GameCoordinatorRepository(ILogger<GameCoordinatorRepository> logger, AghanimsFantasyContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    #region CMsgDotaMatch
    public async Task<CMsgDOTAMatch?> GetGcDotaMatch(ulong MatchId)
    {
        _logger.LogInformation($"Fetching Single GcDotaMatch {MatchId}");

        return await _dbContext.GcDotaMatches.FindAsync(MatchId);
    }

    public async Task UpsertGcDotaMatch(CMsgDOTAMatch upsertMatch)
    {
        _logger.LogInformation($"Upserting Dota Match {upsertMatch.match_id}");

        if (_dbContext.GcDotaMatches.Any(gdm => gdm.match_id == upsertMatch.match_id))
        {
            _dbContext.GcDotaMatches.Update(upsertMatch);
        }
        else
        {
            _dbContext.GcDotaMatches.Add(upsertMatch);
        }

        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task<List<(CMsgDOTAMatch, CMsgDOTAMatch.Player)>> GetGcDotaMatchesNotInFantasyMatchPlayers(int takeAmount)
    {
        _logger.LogInformation($"Getting new GcDotaMatches not loaded into Fantasy Match Players");

        var newGcDotaMatchQuery = _dbContext.GcDotaMatches
                .SelectMany(match => match.players,
                (left, right) => new { Match = left, MatchPlayer = right })
                .Where(gcmp =>
                    !_dbContext.FantasyMatchPlayers.Any(fmp => fmp.MatchId == (long)gcmp.Match.match_id && fmp.Account != null && fmp.Account.Id == gcmp.MatchPlayer.account_id)
                    && (gcmp.Match.match_outcome == EMatchOutcome.k_EMatchOutcome_RadVictory || gcmp.Match.match_outcome == EMatchOutcome.k_EMatchOutcome_DireVictory) // Filter out cancelled games
                )
                .Take(takeAmount);

        _logger.LogDebug($"GetGcDotaMatchesNotInFantasyMatchPlayers SQL Query: {newGcDotaMatchQuery.ToQueryString()}");

        var result = await newGcDotaMatchQuery.ToListAsync();

        return result.Select(rs => (rs.Match, rs.MatchPlayer)).ToList();
    }

    public async Task<List<CMsgDOTAMatch>> GetGcDotaMatchesNotInGcMetadata(int takeAmount)
    {
        _logger.LogInformation($"Getting new GcDotaMatches not loaded into Fantasy Match Players");

        var newGcDotaMatchQuery = _dbContext.GcDotaMatches
                .Where(match => _dbContext.Leagues
                        .Where(league => league.IsActive)
                        .Select(league => league.Id)
                        .Contains((int)match.leagueid)
                )
                .Where(gcdm => gcdm.replay_state == CMsgDOTAMatch.ReplayState.REPLAY_AVAILABLE &&
                    !_dbContext.GcMatchMetadata.Any(gcmm => (ulong)gcmm.MatchId == gcdm.match_id))
                .Take(takeAmount);

        _logger.LogDebug($"GetGcDotaMatchesNotInGcMetadata SQL Query: {newGcDotaMatchQuery.ToQueryString()}");

        return await newGcDotaMatchQuery.ToListAsync();
    }
    #endregion CMsgDotaMatch

    #region GcMatchMetadata
    public async Task AddGcMatchMetadata(GcMatchMetadata newGcMetadata)
    {
        _logger.LogInformation($"Adding GcMatchMetadata {newGcMetadata.MatchId}");
        await _dbContext.GcMatchMetadata.AddAsync(newGcMetadata);
        await _dbContext.SaveChangesAsync();
    }
    #endregion GcMatchMetadata

    public async Task<List<GcMatchMetadata>> GetLeagueMetadataAsync(int LeagueId)
    {
        _logger.LogInformation($"Getting Match Metadata for League {LeagueId}");

        var leagueMetadataQuery = QueryLeagueMatchMetadata(LeagueId)
            .OrderByDescending(md => md.MatchId);

        _logger.LogDebug($"Match Metadata SQL Query: {leagueMetadataQuery.ToQueryString()}");

        return await leagueMetadataQuery.ToListAsync();
    }

    public async Task<List<GcMatchMetadata>> GetLeagueMetadataAsync(int LeagueId, int Skip = 0, int Take = 50)
    {
        _logger.LogInformation($"Getting Match Metadata for League {LeagueId}");

        var leagueMetadataQuery = QueryLeagueMatchMetadata(LeagueId)
            .OrderByDescending(md => md.MatchId)
            .Skip(Skip)
            .Take(Take);

        _logger.LogDebug($"Match Metadata SQL Query: {leagueMetadataQuery.ToQueryString()}");

        return await leagueMetadataQuery.ToListAsync();
    }

    public async Task<List<GcMatchMetadata>> GetFantasyLeagueMetadataAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Getting Match Metadata for Fantasy League {FantasyLeagueId}");

        var fantasyLeagueMetadataQuery = await QueryFantasyLeagueMatchMetadata(FantasyLeagueId)
            .OrderByDescending(md => md.MatchId)
            .ToListAsync();

        return fantasyLeagueMetadataQuery;
    }

    public async Task<List<GcMatchMetadata>> GetFantasyLeagueMetadataAsync(int FantasyLeagueId, int Skip = 0, int Take = 50)
    {
        _logger.LogInformation($"Getting Match Metadata for Fantasy League {FantasyLeagueId}");

        var fantasyLeagueMetadataQuery = QueryFantasyLeagueMatchMetadata(FantasyLeagueId)
            .OrderByDescending(md => md.MatchId)
            .Skip(Skip)
            .Take(Take);

        _logger.LogDebug($"Match Metadata SQL Query: {fantasyLeagueMetadataQuery.ToQueryString()}");

        return await fantasyLeagueMetadataQuery.ToListAsync();
    }

    public async Task<GcMatchMetadataPlayer> GetMatchMetadataPlayerAsync(FantasyMatchPlayer fantasyMatchPlayer)
    {
        _logger.LogInformation($"Getting Match Metadata Player for FantasyMatchPlayer: {fantasyMatchPlayer.Id}");

        var matchMetadataPlayerQuery = _dbContext.GcMatchMetadata
            .Where(md => md.MatchId == fantasyMatchPlayer.MatchId)
            .SelectMany(md => md.Teams,
            (left, right) => new { Match = left, Team = right })
            .SelectMany(mdt => mdt.Team.Players,
            (left, right) => new { Match = left.Match, Team = left.Team, Player = right })
            .Where(mdp => mdp.Match.MatchId == fantasyMatchPlayer.MatchId && mdp.Player.PlayerSlot == fantasyMatchPlayer.PlayerSlot)
            .Select(result => result.Player);

        _logger.LogDebug($"GetMatchMetadataPlayerAsync SQL Query: {matchMetadataPlayerQuery.ToQueryString()}");

        return await matchMetadataPlayerQuery.FirstAsync();
    }

    public async Task<GcMatchMetadata?> GetMatchMetadataAsync(long MatchId)
    {
        _logger.LogInformation($"Getting Match Metadata for Match: {MatchId}");

        var matchMetadataQuery = _dbContext.GcMatchMetadata
                .Where(md => md.MatchId == MatchId);

        _logger.LogDebug($"Match Metadata Query: {matchMetadataQuery.ToQueryString()}");

        return await matchMetadataQuery.FirstOrDefaultAsync();
    }

    private IQueryable<GcMatchMetadata> QueryLeagueMatchMetadata(int? LeagueId)
    {
        // This logic is awkward so I'd rather do it all once here and return a Queryable
        var leagueMatchesQuery = _dbContext.Leagues
            .SelectMany(
                l => l.MatchMetadatas,
                (left, right) => new { League = left, MatchMetadata = right }
            )
            .Where(l => l.League.Id == LeagueId || LeagueId == null)
            .Where(l => l.MatchMetadata != null)
            .Select(l => l.MatchMetadata);

        return leagueMatchesQuery;
    }

    private IQueryable<GcMatchMetadata> QueryFantasyLeagueMatchMetadata(int FantasyLeagueId)
    {
        // This logic is awkward so I'd rather do it all once here and return a Queryable
        var fantasyLeagueMatchesQuery = _dbContext.Leagues
            .SelectMany(
                l => l.FantasyLeagues,
                (left, right) => new { League = left, FantasyLeague = right }
            )
            .SelectMany(
                l => l.League.MatchHistories,
                (left, right) => new { left.League, left.FantasyLeague, MatchHistory = right }
            )
            .SelectMany(
                l => l.League.MatchMetadatas,
                (left, right) => new { left.League, left.FantasyLeague, left.MatchHistory, MatchMetadata = right }
            )
            .Where(l => l.FantasyLeague.Id == FantasyLeagueId)
            .Where(l =>
                l.MatchHistory.StartTime >= l.FantasyLeague.LeagueStartTime &&
                l.MatchHistory.StartTime <= l.FantasyLeague.LeagueEndTime
            )
            .Where(l => l.MatchMetadata != null)
            .Select(l => l.MatchMetadata);

        return fantasyLeagueMatchesQuery;
    }
}