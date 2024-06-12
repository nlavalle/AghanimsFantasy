using csharp_ef_webapi.Models.GameCoordinator;

namespace csharp_ef_webapi.Data;
public interface IGameCoordinatorRepository
{
    // Matches
    Task<IEnumerable<GcMatchMetadata>> GetLeagueMetadataAsync(int LeagueId);
    Task<IEnumerable<GcMatchMetadata>> GetLeagueMetadataAsync(int LeagueId, int Skip, int Take);
    Task<IEnumerable<GcMatchMetadata>> GetFantasyLeagueMetadataAsync(int FantasyLeagueId);
    Task<IEnumerable<GcMatchMetadata>> GetFantasyLeagueMetadataAsync(int FantasyLeagueId, int Skip, int Take);
    Task<GcMatchMetadata?> GetMatchMetadataAsync(long MatchId);
}