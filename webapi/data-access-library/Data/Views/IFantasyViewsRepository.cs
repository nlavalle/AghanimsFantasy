using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Fantasy;

namespace DataAccessLibrary.Data;

public interface IFantasyViewsRepository
{
    IQueryable<FantasyPlayerPoints> GetPlayerPointsQueryable();
    IQueryable<FantasyPlayerPointTotals> GetPlayerPointTotalsQueryable();
    IQueryable<MatchHighlights> GetMatchHighlightsQueryable();
    IQueryable<MetadataSummary> GetMetadataSummariesQueryable();
    IQueryable<FantasyNormalizedAveragesTable> GetFantasyNormalizedAveragesQueryable();
}