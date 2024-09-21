namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.GameCoordinator;
using DataAccessLibrary.Models.ProMetadata;

public interface IGcMatchMetadataRepository : IRepository<GcMatchMetadata, long>
{
    Task<List<GcMatchMetadata>> GetByLeagueAsync(League League, int Skip = 0, int Take = 50);
    Task<List<GcMatchMetadata>> GetByFantasyLeagueAsync(FantasyLeague FantasyLeague, int Skip = 0, int Take = 50);
}