namespace csharp_ef_data_loader.Services;

using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
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
            await _dbContext.FantasyPlayerBudgetProbability.ExecuteDeleteAsync();
            List<FantasyPlayerBudgetProbability> fantasyPlayerBudgetProbabilities = await _dbContext.FantasyPlayerBudgetProbabilityView
                .Include(fpbp => fpbp.Account)
                .Include(fpbp => fpbp.FantasyLeague)
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
