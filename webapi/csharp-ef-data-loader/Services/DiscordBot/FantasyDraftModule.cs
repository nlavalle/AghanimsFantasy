namespace csharp_ef_data_loader.Services.Modules;

using DataAccessLibrary.Data;
using DataAccessLibrary.Data.Facades;
using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Discord;
using DataAccessLibrary.Models.Fantasy;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class FantasyDraftModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger<FantasyDraftModule> _logger;
    private readonly AghanimsFantasyContext _dbContext;
    private readonly FantasyDraftFacade _fantasyDraftFacade;
    private DiscordUser? _discordUser;

    public FantasyDraftModule(
        ILogger<FantasyDraftModule> logger,
        AghanimsFantasyContext dbContext,
        FantasyDraftFacade fantasyDraftFacade
        )
    {
        _logger = logger;
        _dbContext = dbContext;
        _fantasyDraftFacade = fantasyDraftFacade;
    }

    [SlashCommand("set-fantasy-draft", "Draft your 5 Fantasy Players for a given Fantasy League")]
    public async Task FantasyDraft()
    {
        await CheckUserAsync();

        await RespondAsync($"Let's begin drafting fantasy players", ephemeral: true);

        var fantasyDraftMessage = await GetOriginalResponseAsync();

        var fantasyLeagues = await _dbContext.FantasyLeagues.Include(fl => fl.League).ToListAsync();
        var openFantasyLeagues = fantasyLeagues.Where(fl => DateTime.UtcNow < DateTime.UnixEpoch.AddSeconds(fl.FantasyDraftLocked)).ToList();

        // Bomb out if nothing is active
        if (openFantasyLeagues.Count() == 0)
        {
            await FollowupAsync("No Fantasy Leagues are currently open.", ephemeral: true);
            return;
        }

        var fantasyLeagueSelectMenu = BuildFantasyLeagueSelectMenu(openFantasyLeagues);

        var builder = new ComponentBuilder()
            .WithSelectMenu(fantasyLeagueSelectMenu);

        // Send step prompt with select menu
        await ModifyOriginalResponseAsync(msg =>
        {
            msg.Content = $"What fantasy league are you drafting for?";
            msg.Components = builder.Build();
        });

        // Wait for the user's response
        var userResponse = await NextSelectMenuResponseAsync(Context.User, fantasyLeagueSelectMenu.CustomId);
        if (userResponse == null)
        {
            await ModifyOriginalResponseAsync(msg =>
            {
                msg.Content = "You took too long to respond. Setup aborted.";
            });
            return;
        }

        var selectedFantasyLeague = openFantasyLeagues.First(fl => fl.Id.ToString() == userResponse);
        var availableFantasyPlayers = await _dbContext.FantasyPlayers
            .Include(fp => fp.FantasyLeague)
            .Include(fp => fp.Team)
            .Include(fp => fp.DotaAccount)
            .Where(fp => fp.FantasyLeagueId == selectedFantasyLeague.Id)
            .OrderBy(fp => fp.Team!.Name)
                .ThenBy(fp => fp.DotaAccount!.Name)
            .ToListAsync();

        var fantasyPlayerCosts = await _dbContext.FantasyPlayerBudgetProbability
            .Include(fpbp => fpbp.Account)
            .Where(fpbp => fpbp.FantasyLeague.Id == selectedFantasyLeague.Id)
            .ToListAsync();

        List<FantasyPlayer> selectedFantasyPlayers = new List<FantasyPlayer>();

        for (int position = 1; position <= 5; position++)
        {
            try
            {
                var fullTeams = selectedFantasyPlayers.GroupBy(sfp => sfp.Team!.Id).Where(grp => grp.Count() >= 2);
                var filteredPlayers = availableFantasyPlayers.Where(player =>
                    !fullTeams.Any(team => player.Team!.Id == team.Key)
                ).ToList();
                var fantasyPlayerMenu = BuildFantasyPlayerSelectMenu(filteredPlayers, position, fantasyPlayerCosts);

                var fantasyPlayerBuilder = new ComponentBuilder()
                    .WithSelectMenu(fantasyPlayerMenu);

                await ModifyOriginalResponseAsync(msg =>
                {
                    msg.Content = $@"
Fantasy League: {selectedFantasyLeague.League!.Name} {selectedFantasyLeague.Name}
Choose a Fantasy Player for position {position}
{(selectedFantasyPlayers.Count() > 0 ?
    string.Join(", ", selectedFantasyPlayers.Select(fp => $"{fp.DotaAccount!.Name} - {Math.Round(fantasyPlayerCosts.Where(fpc => fpc.Account.Id == fp.DotaAccountId).Sum(fpc => fpc.EstimatedCost))}g")) :
    "")}
";
                    msg.Components = fantasyPlayerBuilder.Build();
                });

                var fantasyPlayerUserResponse = await NextSelectMenuResponseAsync(Context.User, fantasyPlayerMenu.CustomId);
                if (fantasyPlayerUserResponse == null)
                {
                    await ModifyOriginalResponseAsync(msg =>
                    {
                        msg.Content = "You took too long to respond. Setup aborted.";
                    });
                    return;
                }
                else
                {
                    selectedFantasyPlayers.Add(availableFantasyPlayers.First(fp => fp.Id.ToString() == fantasyPlayerUserResponse));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        var draftConfirmBuilder = new ComponentBuilder()
            .WithButton("Confirm", customId: "confirm_draft", ButtonStyle.Success)
            .WithButton("Cancel", customId: "cancel_draft", ButtonStyle.Danger);

        if (Math.Round(fantasyPlayerCosts.Where(fpc => selectedFantasyPlayers.Select(fp => fp.DotaAccountId).Contains(fpc.Account.Id)).Sum(fpc => fpc.EstimatedCost)) > 600)
        {
            // User spent too much money on players, TODO: add a back functionality but if this is unpopular it's low ROI right now
            await ModifyOriginalResponseAsync(msg =>
            {
                msg.Content = $"You exceeded the budget of 600g, please retry the draft";
                msg.Components = null;
            });
            return;
        }

        await ModifyOriginalResponseAsync(msg =>
        {
            msg.Content = $@"
Here's your picks: {string.Join(", ", selectedFantasyPlayers.Select(fp => $"{fp.DotaAccount!.Name} - {Math.Round(fantasyPlayerCosts.Where(fpc => fpc.Account.Id == fp.DotaAccountId).Sum(fpc => fpc.EstimatedCost))}g"))}
Total Cost: {Math.Round(fantasyPlayerCosts.Where(fpc => selectedFantasyPlayers.Select(fp => fp.DotaAccountId).Contains(fpc.Account.Id)).Sum(fpc => fpc.EstimatedCost))}g";
            msg.Components = draftConfirmBuilder.Build();
        });

        var draftConfirm = await DraftConfirmResponseAsync(Context.User);

        if (draftConfirm == null)
        {
            await ModifyOriginalResponseAsync(msg =>
            {
                msg.Content = $"Unknown selection, aborting";
            });
            return;
        }
        else if (draftConfirm == true)
        {
            await UpdateFantasyDraft(selectedFantasyLeague, selectedFantasyPlayers);
            await ModifyOriginalResponseAsync(msg =>
            {
                msg.Content = $"Fantasy Draft Submitted! ✅";
                msg.Components = null;
            });
        }
        else if (draftConfirm == false)
        {
            await ModifyOriginalResponseAsync(msg =>
            {
                msg.Content = $"Draft Cancelled ❌";
                msg.Components = null;
            });
        }
    }

    private async Task CheckUserAsync()
    {
        var discordUser = await _dbContext.DiscordUsers.FindAsync((long)Context.User.Id);
        if (discordUser != null)
        {
            _discordUser = discordUser;
            return;
        }
        else
        {
            DiscordUser newUser = new DiscordUser()
            {
                Id = (long)Context.User.Id,
                IsAdmin = false,
                Username = Context.User.Username
            };
            await _dbContext.DiscordUsers.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();
            _discordUser = newUser;
        }
        return;
    }

    private SelectMenuBuilder BuildFantasyLeagueSelectMenu(IEnumerable<FantasyLeague> fantasyLeagues)
    {
        var options = new List<SelectMenuOptionBuilder>();
        foreach (FantasyLeague fantasyLeague in fantasyLeagues)
        {
            options.Add(new SelectMenuOptionBuilder(
                label: $"{fantasyLeague.League!.Name} - {fantasyLeague.Name}",
                value: $"{fantasyLeague.Id}"
            ));
        }

        return new SelectMenuBuilder()
            .WithPlaceholder($"Choose an option for Fantasy League")
            .WithCustomId($"fantasydraftleague_menu")
            .WithMinValues(1)
            .WithMaxValues(1)
            .WithOptions(options);
    }

    private SelectMenuBuilder BuildFantasyPlayerSelectMenu(IEnumerable<FantasyPlayer> fantasyPlayers, int teamPosition, IEnumerable<FantasyPlayerBudgetProbabilityTable> fantasyBudgets)
    {
        var options = new List<SelectMenuOptionBuilder>();
        foreach (FantasyPlayer fantasyPlayer in fantasyPlayers.Where(fp => fp.TeamPosition == teamPosition))
        {
            var playerBudget = fantasyBudgets.Where(fb => fb.Account.Id == fantasyPlayer.DotaAccountId).Sum(fb => fb.EstimatedCost);
            options.Add(new SelectMenuOptionBuilder(
                label: $"{fantasyPlayer.DotaAccount!.Name} - {fantasyPlayer.Team!.Name} - {Math.Round(playerBudget)}g",
                value: $"{fantasyPlayer.Id}"
            ));
        }

        return new SelectMenuBuilder()
            .WithPlaceholder($"Choose a Fantasy Player")
            .WithCustomId($"fantasyplayer{teamPosition}_menu")
            .WithMinValues(1)
            .WithMaxValues(1)
            .WithOptions(options);
    }

    // Await the next select menu response
    private async Task<string?> NextSelectMenuResponseAsync(IUser user, string customId)
    {
        var completionSource = new TaskCompletionSource<string>();

        async Task Handler(SocketMessageComponent component)
        {
            if (component.User.Id == user.Id && component.Data.CustomId == customId)
            {
                await component.DeferAsync();
                completionSource.SetResult(component.Data.Values.FirstOrDefault() ?? "");
            }
        }

        Context.Client.SelectMenuExecuted += Handler;

        // Wait for the user's response (timeout: 60 seconds)
        var task = await Task.WhenAny(completionSource.Task, Task.Delay(60000));

        Context.Client.SelectMenuExecuted -= Handler;

        if (task == completionSource.Task)
            return completionSource.Task.Result;

        return null;
    }

    private async Task<bool?> DraftConfirmResponseAsync(IUser user)
    {
        var completionSource = new TaskCompletionSource<bool>();

        async Task Handler(SocketMessageComponent component)
        {
            if (component.User.Id == user.Id)
            {
                switch (component.Data.CustomId)
                {
                    case "confirm_draft":
                        completionSource.SetResult(true);
                        break;

                    case "cancel_draft":
                        completionSource.SetResult(false);
                        break;

                    default:
                        completionSource.SetCanceled();
                        break;
                }
                await component.DeferAsync();
            }
        }

        Context.Client.ButtonExecuted += Handler;

        // Wait for the user's response (timeout: 60 seconds)
        var task = await Task.WhenAny(completionSource.Task, Task.Delay(60000));

        Context.Client.ButtonExecuted -= Handler;

        if (task == completionSource.Task)
            return completionSource.Task.Result;

        return null;
    }

    private async Task UpdateFantasyDraft(FantasyLeague fantasyLeague, IEnumerable<FantasyPlayer> updateFantasyPlayers)
    {
        var discordUser = await _dbContext.DiscordUsers.FindAsync((long)Context.User.Id);
        if (discordUser == null)
        {
            return;
        }
        if (fantasyLeague == null)
        {
            throw new ArgumentException("Draft created for invalid Fantasy League Id");
        }

        FantasyDraft? existingUserDraft = await _dbContext.FantasyDrafts
                    .Include(fd => fd.DraftPickPlayers)
                    .FirstOrDefaultAsync(fd => fd.FantasyLeagueId == fantasyLeague.Id && fd.DiscordAccountId == discordUser.Id);
        if (DateTime.UtcNow > DateTime.UnixEpoch.AddSeconds(fantasyLeague.FantasyDraftLocked))
        {
            // TODO: Set this up so that a user can draft late, but then the points only count starting from that time
            // If a user hasn't drafted yet let them add it in late, if they already have a draft though return a bad request cannot update
            throw new ArgumentException("Draft is locked for this league");
        }

        // Ensure player has posted a draft that is one of each team position, if there's 2 of the same position then reject it as a bad request
        var fantasyPlayers = await _dbContext.FantasyPlayers
            .Include(fp => fp.FantasyLeague)
            .Include(fp => fp.Team)
            .Include(fp => fp.DotaAccount)
            .Where(fp => fp.FantasyLeagueId == fantasyLeague.Id)
            .OrderBy(fp => fp.Team!.Name)
                .ThenBy(fp => fp.DotaAccount!.Name)
            .ToListAsync();

        List<FantasyDraftPlayer> draftPickPlayers = new List<FantasyDraftPlayer>();

        // Populate fantasy players by ID for draft picks
        foreach (FantasyPlayer updatePlayer in updateFantasyPlayers)
        {
            FantasyDraftPlayer pick = new FantasyDraftPlayer()
            {
                FantasyPlayer = updatePlayer,
                FantasyPlayerId = updatePlayer.Id,
                DraftOrder = updatePlayer.TeamPosition
            };
            draftPickPlayers.Add(pick);
        }

        if (fantasyPlayers.Where(fp => draftPickPlayers.Where(dpp => dpp.FantasyPlayer != null).Any(dpp => dpp.FantasyPlayer!.Id == fp.Id)).GroupBy(fp => fp.TeamPosition).Where(grp => grp.Count() > 1).Count() > 0)
        {
            throw new ArgumentException("Can only draft one of each team position");
        }

        var fantasyPlayerCosts = await _dbContext.FantasyPlayerBudgetProbability
                .Where(fpbp => fpbp.FantasyLeague.Id == fantasyLeague.Id)
                .ToListAsync();

        if (fantasyPlayerCosts.Where(fpc => fantasyPlayers.Where(fp => fp.DotaAccount != null)
                .Select(fp => fp.DotaAccount!.Id)
                .Contains(fpc.Account.Id))
                .Sum(fpc => fpc.EstimatedCost) > 600)
        {
            throw new ArgumentException("Draft exceeds 600g budget");
        }

        if (existingUserDraft != null)
        {
            // To handle partial drafts we're going to always clear the current draft then add the picks
            await _fantasyDraftFacade.ClearPicksAsync(existingUserDraft);
        }
        else
        {
            existingUserDraft = new FantasyDraft
            {
                FantasyLeagueId = fantasyLeague.Id,
                DiscordAccountId = discordUser.Id,
                DraftCreated = DateTime.UtcNow,
                DraftPickPlayers = []
            };
            await _dbContext.FantasyDrafts.AddAsync(existingUserDraft);
            await _dbContext.SaveChangesAsync();
        }

        // Fantasy Draft may be incomplete, so go through and add the IDs passed
        for (int i = 0; i < draftPickPlayers.Count(); i++)
        {
            if (draftPickPlayers[i].FantasyPlayer != null)
            {
                existingUserDraft = await _fantasyDraftFacade.AddPlayerPickAsync(existingUserDraft, draftPickPlayers[i].FantasyPlayer!);
            }
        }
    }
}