namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models.GameCoordinator;

public interface IGameCoordinatorRepository
{
    // Matches
    Task<List<GcMatchMetadata>> GetLeagueMetadataAsync(int LeagueId);
    Task<List<GcMatchMetadata>> GetLeagueMetadataAsync(int LeagueId, int Skip, int Take);
    Task<List<GcMatchMetadata>> GetFantasyLeagueMetadataAsync(int FantasyLeagueId);
    Task<List<GcMatchMetadata>> GetFantasyLeagueMetadataAsync(int FantasyLeagueId, int Skip, int Take);
    Task<GcMatchMetadata?> GetMatchMetadataAsync(long MatchId);
}