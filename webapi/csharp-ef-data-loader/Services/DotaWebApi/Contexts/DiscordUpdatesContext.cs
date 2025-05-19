namespace csharp_ef_data_loader.Services;

using DataAccessLibrary.Data;
using DataAccessLibrary.Data.Facades;
using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Discord.WebSocket;

internal class DiscordUpdatesContext : DotaOperationContext
{
    private readonly ILogger<FantasyMatchContext> _logger;
    private readonly AghanimsFantasyContext _dbContext;
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly DiscordFacade _discordFacade;
    private readonly FantasyPointsFacade _fantasyPointsFacade;
    private readonly FantasyDraftFacade _fantasyDraftFacade;

    public DiscordUpdatesContext(
            ILogger<FantasyMatchContext> logger,
            AghanimsFantasyContext dbContext,
            DiscordSocketClient discordSocketClient,
            DiscordFacade discordFacade,
            FantasyPointsFacade fantasyPointsFacade,
            FantasyDraftFacade fantasyDraftFacade,
            IServiceScope scope,
            Config config
        )
        : base(scope, config)
    {
        _logger = logger;
        _dbContext = dbContext;
        _discordSocketClient = discordSocketClient;
        _discordFacade = discordFacade;
        _fantasyPointsFacade = fantasyPointsFacade;
        _fantasyDraftFacade = fantasyDraftFacade;
    }

    protected override async Task OperateAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Get FantasyMatches from FantasyPlayerPoints (calculated matches) that aren't uploaded into DiscordOutbox
            var unsentFantasyMessages = await _discordFacade.GetFantasyMatchesNotInDiscordOutboxAsync();

            _logger.LogInformation($"Notifying Discord for {unsentFantasyMessages.Count()} matches.");

            await Task.Delay(10000);

            if (_discordSocketClient.LoginState == Discord.LoginState.LoggedIn)
            {
                foreach (SocketGuild guild in _discordSocketClient.Guilds)
                {
                    foreach (FantasyMatch fantasyMatch in unsentFantasyMessages)
                    {
                        await guild.TextChannels.First(tc => tc.Name == "bot-fantasy-events").SendMessageAsync(await FantasyMatchToDiscordMessage(fantasyMatch));
                        await _dbContext.DiscordOutbox.AddAsync(new DataAccessLibrary.Models.Discord.DiscordOutbox
                        {
                            EventObject = "FantasyMatch",
                            EventType = "Scored",
                            ObjectKey = fantasyMatch.MatchId.ToString(),
                            MessageSentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                        });
                    }

                    if (unsentFantasyMessages.Count() > 0)
                    {
                        var affectedMatches = await _fantasyPointsFacade.GetFantasyPlayerPointsByMatchesAsync(unsentFantasyMessages);
                        var affectedFantasyLeagues = affectedMatches.Select(am => am.FantasyLeagueId).Distinct().ToList();
                        _logger.LogInformation($"Pulling {affectedFantasyLeagues.Count()} fantasy leagues to update leaderboard");
                        foreach (int affectedLeague in affectedFantasyLeagues)
                        {
                            var fantasyLeague = await _dbContext.FantasyLeagues.FindAsync(affectedLeague);
                            if (fantasyLeague == null || fantasyLeague.IsPrivate) continue;
                            List<FantasyDraftPointTotals> fantasyPoints = await _fantasyDraftFacade.AllDraftPointTotalsByFantasyLeagueAsync(fantasyLeague);
                            if (fantasyPoints.Count() == 0) continue;

                            List<FantasyDraftPointTotals> top10Players = fantasyPoints.OrderByDescending(fp => fp.DraftTotalFantasyPoints).Take(10).ToList();
                            await guild.TextChannels.First(tc => tc.Name == "bot-fantasy-leaderboard").SendMessageAsync(LeaderboardToDiscordMessage(top10Players));
                        }
                    }
                }
                await _dbContext.SaveChangesAsync();
            }

            _logger.LogInformation($"New Fantasy Matches Published via Discord Bot");
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred: {ex.Message}");
        }
    }

    private async Task<string> FantasyMatchToDiscordMessage(FantasyMatch fantasyMatch)
    {
        var fantasyMatchPlayerPoints = await _fantasyPointsFacade.GetFantasyPlayerPointsByMatchAsync(fantasyMatch.MatchId);
        return $"""
```
New Match Id: {fantasyMatch.MatchId}
========= Radiant =========
- {fantasyMatchPlayerPoints.FirstOrDefault(p => p.FantasyMatchPlayer!.PlayerSlotFormatted == "blue")?.FantasyMatchPlayer?.Account?.Name ?? "unknown"} - Points: {fantasyMatchPlayerPoints.FirstOrDefault(p => p.FantasyMatchPlayer!.PlayerSlotFormatted == "blue")?.TotalMatchFantasyPoints ?? 0}
- {fantasyMatchPlayerPoints.FirstOrDefault(p => p.FantasyMatchPlayer!.PlayerSlotFormatted == "teal")?.FantasyMatchPlayer?.Account?.Name ?? "unknown"} - Points: {fantasyMatchPlayerPoints.FirstOrDefault(p => p.FantasyMatchPlayer!.PlayerSlotFormatted == "teal")?.TotalMatchFantasyPoints ?? 0}
- {fantasyMatchPlayerPoints.FirstOrDefault(p => p.FantasyMatchPlayer!.PlayerSlotFormatted == "purple")?.FantasyMatchPlayer?.Account?.Name ?? "unknown"} - Points: {fantasyMatchPlayerPoints.FirstOrDefault(p => p.FantasyMatchPlayer!.PlayerSlotFormatted == "purple")?.TotalMatchFantasyPoints ?? 0}
- {fantasyMatchPlayerPoints.FirstOrDefault(p => p.FantasyMatchPlayer!.PlayerSlotFormatted == "yellow")?.FantasyMatchPlayer?.Account?.Name ?? "unknown"} - Points: {fantasyMatchPlayerPoints.FirstOrDefault(p => p.FantasyMatchPlayer!.PlayerSlotFormatted == "yellow")?.TotalMatchFantasyPoints ?? 0}
- {fantasyMatchPlayerPoints.FirstOrDefault(p => p.FantasyMatchPlayer!.PlayerSlotFormatted == "orange")?.FantasyMatchPlayer?.Account?.Name ?? "unknown"} - Points: {fantasyMatchPlayerPoints.FirstOrDefault(p => p.FantasyMatchPlayer!.PlayerSlotFormatted == "orange")?.TotalMatchFantasyPoints ?? 0}

========= Dire =========
- {fantasyMatchPlayerPoints.FirstOrDefault(p => p.FantasyMatchPlayer!.PlayerSlotFormatted == "pink")?.FantasyMatchPlayer?.Account?.Name ?? "unknown"} - Points: {fantasyMatchPlayerPoints.FirstOrDefault(p => p.FantasyMatchPlayer!.PlayerSlotFormatted == "pink")?.TotalMatchFantasyPoints ?? 0}
- {fantasyMatchPlayerPoints.FirstOrDefault(p => p.FantasyMatchPlayer!.PlayerSlotFormatted == "grey")?.FantasyMatchPlayer?.Account?.Name ?? "unknown"} - Points: {fantasyMatchPlayerPoints.FirstOrDefault(p => p.FantasyMatchPlayer!.PlayerSlotFormatted == "grey")?.TotalMatchFantasyPoints ?? 0}
- {fantasyMatchPlayerPoints.FirstOrDefault(p => p.FantasyMatchPlayer!.PlayerSlotFormatted == "light blue")?.FantasyMatchPlayer?.Account?.Name ?? "unknown"} - Points: {fantasyMatchPlayerPoints.FirstOrDefault(p => p.FantasyMatchPlayer!.PlayerSlotFormatted == "light blue")?.TotalMatchFantasyPoints ?? 0}
- {fantasyMatchPlayerPoints.FirstOrDefault(p => p.FantasyMatchPlayer!.PlayerSlotFormatted == "dark green")?.FantasyMatchPlayer?.Account?.Name ?? "unknown"} - Points: {fantasyMatchPlayerPoints.FirstOrDefault(p => p.FantasyMatchPlayer!.PlayerSlotFormatted == "dark green")?.TotalMatchFantasyPoints ?? 0}
- {fantasyMatchPlayerPoints.FirstOrDefault(p => p.FantasyMatchPlayer!.PlayerSlotFormatted == "brown")?.FantasyMatchPlayer?.Account?.Name ?? "unknown"} - Points: {fantasyMatchPlayerPoints.FirstOrDefault(p => p.FantasyMatchPlayer!.PlayerSlotFormatted == "brown")?.TotalMatchFantasyPoints ?? 0}
```
""";
    }

    private string LeaderboardToDiscordMessage(List<FantasyDraftPointTotals> top10Drafts)
    {
        var leaderboardString = "";
        for (int i = 1; i <= top10Drafts.Count(); i++)
        {
            leaderboardString += $"{i}.  {top10Drafts[i - 1].UserName} - {Math.Round(top10Drafts[i - 1].DraftTotalFantasyPoints)}\n";
        }

        return $"""
```
Updated Leaderboard for Fantasy League: {top10Drafts.First().FantasyDraft.FantasyLeague!.Name}
{leaderboardString}
```
""";
    }
}
