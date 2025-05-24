namespace csharp_ef_data_loader.Services;

using DataAccessLibrary.Data;
using DataAccessLibrary.Models.ProMetadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

internal class MissingAccountsContext : DotaOperationContext
{
    private readonly ILogger<MissingAccountsContext> _logger;
    private readonly AghanimsFantasyContext _dbContext;

    public MissingAccountsContext(
            ILogger<MissingAccountsContext> logger,
            AghanimsFantasyContext dbContext,
            IServiceScope scope,
            Config config)
        : base(scope, config)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    protected override async Task OperateAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"Fetching missing accounts");
            List<uint> newAccounts = await _dbContext.GcDotaMatches
            .SelectMany(gdm => gdm.players)
            .Select(gdmp => gdmp.account_id)
            .Distinct()
            .Where(a => a != 0)
            .Where(
                pa => !_dbContext.Accounts
                    .Select(a => a.Id)
                    .Contains(pa))
            .ToListAsync();

            if (newAccounts.Count() > 0)
            {
                _logger.LogInformation($"Saving {newAccounts.Count()} new accounts.");

                List<Account> newAcountsToAdd = await GetAccountsAsync(newAccounts);

                await _dbContext.AddRangeAsync(newAcountsToAdd);
                await _dbContext.SaveChangesAsync();
            }

            _logger.LogInformation($"Missing accounts added");
        }
        catch (Exception ex)
        {
            // Handle exceptions here
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private async Task<List<Account>> GetAccountsAsync(List<uint> accountIds)
    {
        return await _dbContext.GcDotaMatches
        .SelectMany(gdm => gdm.players)
        .Where(gdmp => accountIds.Contains(gdmp.account_id))
        .GroupBy(gdmp => gdmp.account_id)
        .Select(group => new Account
        {
            Id = group.Key,
            Name = group.First().pro_name == string.Empty ? group.First().player_name : group.First().pro_name,
            SteamProfilePicture = "https://aghanimsfantasy.com/logos/unknown.png"
        })
        .ToListAsync();
    }
}
