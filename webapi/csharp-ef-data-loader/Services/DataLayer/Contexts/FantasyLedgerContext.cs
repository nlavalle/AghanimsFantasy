namespace csharp_ef_data_loader.Services;

using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

internal class FantasyLedgerContext : DotaOperationContext
{
    private readonly AghanimsFantasyContext _dbContext;
    private readonly ILogger<FantasyLedgerContext> _logger;

    public FantasyLedgerContext(ILogger<FantasyLedgerContext> logger, IServiceScope scope, Config config)
        : base(scope, config)
    {
        _dbContext = scope.ServiceProvider.GetRequiredService<AghanimsFantasyContext>();
        _logger = logger;
    }

    protected override async Task OperateAsync(CancellationToken cancellationToken)
    {
        try
        {
            // We could try to track at the fantasy league if it paid out, but that could beceome untethered from reality
            List<int> paidOutFantasyLeagues = await _dbContext.FantasyLedger.Where(fl => fl.SourceType == "FantasyLeague").Select(fl => fl.SourceId).Distinct().ToListAsync();

            long nowUnixEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            List<FantasyLeague> unpaidFantasyLeagues = await _dbContext.FantasyLeagues
                .Where(fl => !paidOutFantasyLeagues.Contains(fl.Id))
                .Where(fl => fl.LeagueEndTime <= nowUnixEpoch) // Filter only fantasy leagues that ended
                .ToListAsync();

            foreach (FantasyLeague unpaidFantasyLeague in unpaidFantasyLeagues)
            {
                List<FantasyPlayerBudgetProbabilityTable> fantasyBudgets = await _dbContext.FantasyPlayerBudgetProbability
                    .Include(fb => fb.Account)
                    .Where(fl => fl.FantasyLeague == unpaidFantasyLeague)
                    .ToListAsync();
                List<FantasyPlayerPointTotals> fantasyResults = await _dbContext.FantasyPlayerPointTotalsView
                    .Include(fppt => fppt.FantasyPlayer)
                    .Where(fppt => fppt.FantasyLeagueId == unpaidFantasyLeague.Id)
                    .ToListAsync();

                // Get all the drafts from the fantasy league
                List<FantasyDraft> unpaidFantasyDrafts = await _dbContext.FantasyDrafts
                    .Include(fd => fd.DraftPickPlayers)
                        .ThenInclude(dpp => dpp.FantasyPlayer)
                    .Where(fd => fd.FantasyLeagueId == unpaidFantasyLeague.Id)
                    .Where(fd => _dbContext.DiscordUsers.Select(du => du.Id).Contains(fd.DiscordAccountId.GetValueOrDefault()))
                    .ToListAsync();

                List<FantasyLedger> newFantasyLedgers = unpaidFantasyDrafts
                    .Select(fd => new FantasyLedger()
                    {
                        DiscordId = fd.DiscordAccountId.GetValueOrDefault(),
                        Amount = fd.DraftPickPlayers.Sum(dpp =>
                            {
                                var winnings = 300 - GetQuintile(dpp.FantasyPlayer!, fantasyResults) * 60;
                                var cost = fantasyBudgets.Where(fb => fb.Account.Id == dpp.FantasyPlayer!.DotaAccountId).Sum(fb => fb.EstimatedCost);
                                return dpp.FantasyPlayer != null ? winnings - cost : 0; // 0 if no player drafted
                            }),
                        SourceId = unpaidFantasyLeague.Id,
                        SourceType = "FantasyLeague"
                    })
                    .ToList();

                await _dbContext.FantasyLedger.AddRangeAsync(newFantasyLedgers);
                await _dbContext.SaveChangesAsync();
            }

            _logger.LogInformation($"Fantasy League Ledgers loaded");
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred: {ex.Message}");
        }
    }

    private int GetQuintile(FantasyPlayer fantasyPlayer, List<FantasyPlayerPointTotals> fantasyPlayerPointTotals)
    {
        var count = fantasyPlayerPointTotals.Count;
        var index = fantasyPlayerPointTotals
            .OrderByDescending(fppt => fppt.TotalMatchFantasyPoints)
            .Select(fppt => fppt.FantasyPlayer)
            .ToList()
            .IndexOf(fantasyPlayer) + 1;
        var quintileSize = count / 5;
        return (int)Math.Ceiling(index / (decimal)quintileSize);
    }
}
