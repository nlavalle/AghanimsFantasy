namespace csharp_ef_data_loader.Services.Modules;

using DataAccessLibrary.Data;
using DataAccessLibrary.Data.Facades;
using DataAccessLibrary.Data.Identity;
using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Fantasy;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class TipModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger<TipModule> _logger;
    private readonly AghanimsFantasyContext _dbContext;
    private AghanimsFantasyUser? _aghanimsUser;

    public TipModule(
        ILogger<TipModule> logger,
        AghanimsFantasyContext dbContext
        )
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [SlashCommand("tip", "Tip a player 50 shards")]
    public async Task Tip(IUser targetUser)
    {
        var currentUser = Context.User;
        await CheckUserAsync();

        if (_aghanimsUser == null)
        {
            await RespondAsync("Discord User not found, please register on aghanimsfantasy.com or connect your discord if you already have an account", ephemeral: true);
            return;
        }

        // Check targetUser
        var targetAghanimsUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.DiscordId == (long)targetUser.Id);
        if (targetAghanimsUser == null)
        {
            await RespondAsync("Target User not found, please tip someone who is registered on aghanimsfantasy.com. They may not have their discord connected", ephemeral: true);
            return;
        }

        if (targetAghanimsUser.Id == _aghanimsUser.Id)
        {
            await RespondAsync($"You can't tip yourself, silly", ephemeral: true);
            return;
        }

        var currentBalance = await _dbContext.FantasyLedger.Where(fl => fl.UserId == _aghanimsUser.Id).SumAsync(fl => fl.Amount);

        if (currentBalance < 50)
        {
            await RespondAsync($"You're too broke, your current balance is: {currentBalance}. Go play more fantasy", ephemeral: true);
            return;
        }

        await _dbContext.FantasyLedger.AddAsync(new FantasyLedger
        {
            UserId = _aghanimsUser.Id,
            User = _aghanimsUser,
            Amount = -50,
            DiscordId = (long)currentUser.Id,
            LedgerRecordedTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            SourceType = "Tip",
            SourceId = int.Parse(DateTime.UtcNow.ToString("yyyyMMdd"))
        });

        await _dbContext.FantasyLedger.AddAsync(new FantasyLedger
        {
            UserId = targetAghanimsUser.Id,
            User = targetAghanimsUser,
            Amount = 50,
            DiscordId = (long)targetUser.Id,
            LedgerRecordedTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            SourceType = "Tip",
            SourceId = int.Parse(DateTime.UtcNow.ToString("yyyyMMdd"))
        });

        await _dbContext.SaveChangesAsync();

        await RespondAsync($"{currentUser.Mention} - tipped 50 shards to - {targetUser.Mention}");
        return;
    }

    private async Task CheckUserAsync()
    {
        var discordUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.DiscordId == (long)Context.User.Id);
        if (discordUser == null)
        {
            await FollowupAsync("Discord User not found, please register on aghanimsfantasy.com or connect your discord if you already have an account", ephemeral: true);
            return;
        }
        else
        {
            _aghanimsUser = discordUser;
        }
    }
}