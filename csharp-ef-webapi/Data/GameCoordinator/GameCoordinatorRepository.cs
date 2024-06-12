using System.Diagnostics.Tracing;
using csharp_ef_webapi.Models;
using csharp_ef_webapi.Models.GameCoordinator;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace csharp_ef_webapi.Data;

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


    public async Task<IEnumerable<GcMatchMetadata>> GetLeagueMetadataAsync(int LeagueId)
    {
        _logger.LogInformation($"Getting Match Metadata for League {LeagueId}");

        var leagueMetadataQuery = QueryLeagueMatchMetadata(LeagueId)
            .OrderByDescending(md => md.MatchId);

        _logger.LogDebug($"Match Metadata SQL Query: {leagueMetadataQuery.ToQueryString()}");

        return await leagueMetadataQuery.ToListAsync();
    }

    public async Task<IEnumerable<GcMatchMetadata>> GetLeagueMetadataAsync(int LeagueId, int Skip = 0, int Take = 50)
    {
        _logger.LogInformation($"Getting Match Metadata for League {LeagueId}");

        var leagueMetadataQuery = QueryLeagueMatchMetadata(LeagueId)
            .OrderByDescending(md => md.MatchId)
            .Skip(Skip)
            .Take(Take);

        _logger.LogDebug($"Match Metadata SQL Query: {leagueMetadataQuery.ToQueryString()}");

        return await leagueMetadataQuery.ToListAsync();
    }

    public async Task<IEnumerable<GcMatchMetadata>> GetFantasyLeagueMetadataAsync(int FantasyLeagueId)
    {
        _logger.LogInformation($"Getting Match Metadata for Fantasy League {FantasyLeagueId}");

        var fantasyLeagueMetadataQuery = await QueryFantasyLeagueMatchMetadata(FantasyLeagueId)
            .OrderByDescending(md => md.MatchId)
            .ToListAsync();

        return fantasyLeagueMetadataQuery;
    }

    public async Task<IEnumerable<GcMatchMetadata>> GetFantasyLeagueMetadataAsync(int FantasyLeagueId, int Skip = 0, int Take = 50)
    {
        _logger.LogInformation($"Getting Match Metadata for Fantasy League {FantasyLeagueId}");

        var fantasyLeagueMetadataQuery = QueryFantasyLeagueMatchMetadata(FantasyLeagueId)
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