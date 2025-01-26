using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLibrary.Data;

// Views don't have add/update/delete/save functionality but we'll still use the gets
public class FantasyViewsRepository : IFantasyViewsRepository
{
    private AghanimsFantasyContext _dbContext;
    public FantasyViewsRepository(AghanimsFantasyContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IQueryable<FantasyPlayerPoints> GetPlayerPointsQueryable()
    {
        return _dbContext.FantasyPlayerPointsView
                    .Include(fppv => fppv.FantasyPlayer)
                        .ThenInclude(fp => fp.DotaAccount)
                    .Include(fppv => fppv.FantasyPlayer)
                        .ThenInclude(fp => fp.Team)
                    .Include(fppv => fppv.FantasyMatchPlayer)
                        .ThenInclude(fmp => fmp!.Hero);
    }

    public IQueryable<FantasyPlayerPointTotals> GetPlayerPointTotalsQueryable()
    {
        return _dbContext.FantasyPlayerPointTotalsView
                    .Include(fppv => fppv.FantasyPlayer)
                        .ThenInclude(fp => fp.DotaAccount)
                    .Include(fppv => fppv.FantasyPlayer)
                        .ThenInclude(fp => fp.Team);
    }

    public IQueryable<MatchHighlights> GetMatchHighlightsQueryable()
    {
        return _dbContext.MatchHighlightsView
                    .Include(fppv => fppv.FantasyPlayer);
    }

    public IQueryable<MetadataSummary> GetMetadataSummariesQueryable()
    {
        return _dbContext.MetadataSummaries
                    .Include(fppv => fppv.FantasyPlayer);
    }

    public IQueryable<FantasyNormalizedAveragesTable> GetFantasyNormalizedAveragesQueryable()
    {
        return _dbContext.FantasyNormalizedAverages;
    }
}