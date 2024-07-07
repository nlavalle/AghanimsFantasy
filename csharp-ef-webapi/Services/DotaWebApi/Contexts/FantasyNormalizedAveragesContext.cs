using csharp_ef_webapi.Data;
using csharp_ef_webapi.Models;
using csharp_ef_webapi.Models.Fantasy;
using Microsoft.EntityFrameworkCore;

namespace csharp_ef_webapi.Services;
internal class FantasyNormalizedAveragesContext : DotaOperationContext
{
    // private readonly DotaWebApiService _apiService;
    private readonly AghanimsFantasyContext _dbContext;
    private readonly ILogger<FantasyNormalizedAveragesContext> _logger;

    public FantasyNormalizedAveragesContext(ILogger<FantasyNormalizedAveragesContext> logger, IServiceScope scope, Config config)
        : base(scope, config)
    {
        _dbContext = scope.ServiceProvider.GetRequiredService<AghanimsFantasyContext>();
        _logger = logger;
    }

    protected override async Task OperateAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Clear the fantasy normalized averages table and load it with the current view
            await _dbContext.FantasyNormalizedAverages.ExecuteDeleteAsync();
            List<FantasyNormalizedAverages> fantasyNormalizedAverages = await _dbContext.FantasyNormalizedAveragesView.ToListAsync();

            List<FantasyNormalizedAveragesTable> convertedTableRows = fantasyNormalizedAverages.Select(f => new FantasyNormalizedAveragesTable()
            {
                AvgAegisSnatchedPoints = f.AvgAegisSnatchedPoints,
                AvgAssistsPoints = f.AvgAssistsPoints,
                AvgCampsStackedPoints = f.AvgCampsStackedPoints,
                AvgCouriersKilledPoints = f.AvgCouriersKilledPoints,
                AvgDeathsPoints = f.AvgDeathsPoints,
                AvgFarmScore = f.AvgFarmScore,
                AvgFightScore = f.AvgFightScore,
                AvgGoldPerMinPoints = f.AvgGoldPerMinPoints,
                AvgGoldPoints = f.AvgGoldPoints,
                AvgHeroDamagePoints = f.AvgHeroDamagePoints,
                AvgHeroHealingPoints = f.AvgHeroHealingPoints,
                AvgHeroXpPoints = f.AvgHeroXpPoints,
                AvgKillsPoints = f.AvgKillsPoints,
                AvgLastHitsPoints = f.AvgLastHitsPoints,
                AvgMatchFantasyPoints = f.AvgMatchFantasyPoints,
                AvgNetworthPoints = f.AvgNetworthPoints,
                AvgObserverWardsPlacedPoints = f.AvgObserverWardsPlacedPoints,
                AvgPushScore = f.AvgPushScore,
                AvgRampagesPoints = f.AvgRampagesPoints,
                AvgRapiersPurchasedPoints = f.AvgRapiersPurchasedPoints,
                AvgSentryWardsPlacedPoints = f.AvgSentryWardsPlacedPoints,
                AvgStunDurationPoints = f.AvgStunDurationPoints,
                AvgSupportGoldSpentPoints = f.AvgSupportGoldSpentPoints,
                AvgSupportScore = f.AvgSupportScore,
                AvgTowerDamagePoints = f.AvgTowerDamagePoints,
                AvgTripleKillsPoints = f.AvgTripleKillsPoints,
                AvgWardsDewardedPoints = f.AvgWardsDewardedPoints,
                AvgXpPerMinPoints = f.AvgXpPerMinPoints,
                FantasyPlayer = f.FantasyPlayer,
                FantasyPlayerId = f.FantasyPlayerId,
                TotalMatches = f.TotalMatches
            }
            ).ToList();

            await _dbContext.FantasyNormalizedAverages.AddRangeAsync(convertedTableRows);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Fantasy Normalized Averages table refreshed");
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred: {ex.Message}");
        }
    }
}
