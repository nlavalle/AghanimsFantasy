namespace DataAccessLibrary.Data;

using System.Threading.Tasks;
using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.GameCoordinator;
using DataAccessLibrary.Models.ProMetadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Service to fetch and transform the Postgres data to read/write fantasy draft data for the webapi
/// example is to fetch all of the current scores, vs adding a new draft. Controllers should handle none of the business
/// logic 
/// </summary>
public class GcMatchMetadataRepository : IGcMatchMetadataRepository
{
    private readonly ILogger<GcMatchMetadataRepository> _logger;
    private AghanimsFantasyContext _dbContext;
    public GcMatchMetadataRepository(ILogger<GcMatchMetadataRepository> logger, AghanimsFantasyContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<List<GcMatchMetadata>> GetByLeagueAsync(League League, int Skip = 0, int Take = 50)
    {
        _logger.LogInformation($"Getting Match Metadata for League {League.Id}");

        var leagueMetadataQuery = QueryLeagueMatchMetadata(League.Id)
            .OrderByDescending(md => md.MatchId)
            .Skip(Skip)
            .Take(Take);

        _logger.LogDebug($"Match Metadata SQL Query: {leagueMetadataQuery.ToQueryString()}");

        return await leagueMetadataQuery.ToListAsync();
    }

    public async Task<List<GcMatchMetadata>> GetByFantasyLeagueAsync(FantasyLeague FantasyLeague, int Skip = 0, int Take = 50)
    {
        _logger.LogInformation($"Getting Match Metadata for Fantasy League {FantasyLeague.Id}");

        var fantasyLeagueMetadataQuery = QueryFantasyLeagueMatchMetadata(FantasyLeague.Id)
            .OrderByDescending(md => md.MatchId)
            .Skip(Skip)
            .Take(Take);

        _logger.LogDebug($"Match Metadata SQL Query: {fantasyLeagueMetadataQuery.ToQueryString()}");

        return await fantasyLeagueMetadataQuery.ToListAsync();
    }

    public async Task<GcMatchMetadata?> GetByIdAsync(long MatchId)
    {
        _logger.LogInformation($"Fetching Single GcMatchmetadata {MatchId}");

        return await _dbContext.GcMatchMetadata.FirstOrDefaultAsync(gmm => gmm.MatchId == MatchId);
    }

    public async Task<IEnumerable<GcMatchMetadata>> GetAllAsync()
    {
        _logger.LogInformation($"Get GcMatchmetadata");

        return await _dbContext.GcMatchMetadata.ToListAsync();
    }

    public async Task AddAsync(GcMatchMetadata addGcMatchMetadata)
    {
        _logger.LogInformation($"Adding new GcMatchmetadata {addGcMatchMetadata.MatchId}");

        await _dbContext.GcMatchMetadata.AddAsync(addGcMatchMetadata);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task DeleteAsync(GcMatchMetadata deleteGcMatchMetadata)
    {
        _logger.LogInformation($"Removing GcMatchmetadata {deleteGcMatchMetadata.MatchId}");

        _dbContext.GcMatchMetadata.Remove(deleteGcMatchMetadata);
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task UpdateAsync(GcMatchMetadata updateGcMatchMetadata)
    {
        _logger.LogInformation($"Updating GcMatchmetadata {updateGcMatchMetadata.MatchId}");

        _dbContext.Entry(updateGcMatchMetadata).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        return;
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