namespace csharp_ef_data_loader.Services;

using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

internal class FantasyPlayerBudgetProbabilityContext : DotaOperationContext
{
    private readonly AghanimsFantasyContext _dbContext;
    private readonly ILogger<FantasyPlayerBudgetProbabilityContext> _logger;

    public FantasyPlayerBudgetProbabilityContext(ILogger<FantasyPlayerBudgetProbabilityContext> logger, IServiceScope scope, Config config)
        : base(scope, config)
    {
        _dbContext = scope.ServiceProvider.GetRequiredService<AghanimsFantasyContext>();
        _logger = logger;
    }

    protected override async Task OperateAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Clear the fantasy budget probabilities table and load it with the current view

            // For the sake of performance we're going to perform this only for fantasy leagues that haven't started
            List<FantasyLeague> activeFantasyLeagues = await _dbContext.FantasyLeagues.Where(fl => DateTime.UnixEpoch.AddSeconds(fl.LeagueStartTime) >= DateTime.UtcNow).ToListAsync();

            List<FantasyPlayerBudgetProbability> fantasyPlayerBudgetProbabilities = await _dbContext.FantasyPlayerBudgetProbabilityView
                .Include(fpbp => fpbp.Account)
                .Include(fpbp => fpbp.FantasyLeague)
                .Where(fpbp => activeFantasyLeagues.Select(afl => afl.Id).Contains(fpbp.FantasyLeagueId))
                .ToListAsync();

            List<FantasyPlayerBudgetProbabilityTable> convertedTableRows = fantasyPlayerBudgetProbabilities.Select(f => new FantasyPlayerBudgetProbabilityTable()
            {
                FantasyLeague = f.FantasyLeague,
                Account = f.Account,
                Quintile = f.Quintile,
                Probability = f.Probability,
                CumulativeProbability = f.CumulativeProbability,
            }
            ).ToList();

            await _dbContext.FantasyPlayerBudgetProbability.Where(fpbp => activeFantasyLeagues.Select(afl => afl.Id).Contains(fpbp.FantasyLeague.Id)).ExecuteDeleteAsync();
            await _dbContext.FantasyPlayerBudgetProbability.AddRangeAsync(convertedTableRows);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Fantasy Budget Probabilities table refreshed");
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred: {ex.Message}");
        }
    }
}
