namespace csharp_ef_data_loader.Services;

using DataAccessLibrary.Data;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Discord.WebSocket;
using DataAccessLibrary.Models;

internal class DiscordUpdatesContext : DotaOperationContext
{
    private readonly IFantasyDraftRepository _fantasyDraftRepository;
    private readonly IFantasyLeagueRepository _fantasyLeagueRepository;
    private readonly IFantasyMatchPlayerRepository _fantasyMatchPlayerRepository;
    private readonly ILogger<FantasyMatchContext> _logger;
    private readonly IDiscordRepository _discordRepository;
    private readonly DiscordSocketClient _discordSocketClient;

    public DiscordUpdatesContext(
            ILogger<FantasyMatchContext> logger,
            IFantasyDraftRepository fantasyDraftRepository,
            IFantasyLeagueRepository fantasyLeagueRepository,
            IFantasyMatchPlayerRepository fantasyMatchPlayerRepository,
            IDiscordRepository discordRepository,
            DiscordSocketClient discordSocketClient,
            IServiceScope scope,
            Config config
        )
        : base(scope, config)
    {
        _logger = logger;
        _fantasyDraftRepository = fantasyDraftRepository;
        _fantasyLeagueRepository = fantasyLeagueRepository;
        _fantasyMatchPlayerRepository = fantasyMatchPlayerRepository;
        _discordRepository = discordRepository;
        _discordSocketClient = discordSocketClient;
    }

    protected override async Task OperateAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Get FantasyMatches from FantasyPlayerPoints (calculated matches) that aren't uploaded into DiscordOutbox
            var unsentFantasyMessages = await _discordRepository.GetFantasyMatchesNotInDiscordOutboxAsync();

            await Task.Delay(10000);

            if (_discordSocketClient.LoginState == Discord.LoginState.LoggedIn)
            {
                foreach (SocketGuild guild in _discordSocketClient.Guilds)
                {
                    foreach (FantasyMatch fantasyMatch in unsentFantasyMessages.Take(2))
                    {
                        await guild.TextChannels.First(tc => tc.Name == "bot-fantasy-events").SendMessageAsync(await FantasyMatchToDiscordMessage(fantasyMatch));
                        await _discordRepository.AddDiscordOutboxAsync(new DataAccessLibrary.Models.Discord.DiscordOutbox
                        {
                            EventObject = "FantasyMatch",
                            EventType = "Scored",
                            ObjectKey = fantasyMatch.MatchId.ToString(),
                            MessageSentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                        });
                    }

                    if (unsentFantasyMessages.Count() > 1)
                    {
                        var affectedMatches = await _fantasyMatchPlayerRepository.GetFantasyPlayerPointsByMatchesAsync(unsentFantasyMessages.Select(fm => fm.MatchId).ToList());
                        var affectedFantasyLeagues = affectedMatches.Select(am => am.FantasyLeagueId).Distinct().ToList();
                        foreach (int affectedLeague in affectedFantasyLeagues)
                        {
                            var fantasyLeague = await _fantasyLeagueRepository.GetByIdAsync(affectedLeague);
                            if (fantasyLeague == null || fantasyLeague.IsPrivate) continue;
                            List<FantasyDraftPointTotals> fantasyPoints = await _fantasyDraftRepository.AllDraftPointTotalsByFantasyLeagueAsync(fantasyLeague);
                            if (fantasyPoints.Count() == 0) continue;

                            List<FantasyDraftPointTotals> top10Players = fantasyPoints.Where(fp => !fp.IsTeam).OrderByDescending(fp => fp.DraftTotalFantasyPoints).Take(10).ToList();
                            await guild.TextChannels.First(tc => tc.Name == "bot-fantasy-leaderboard").SendMessageAsync(LeaderboardToDiscordMessage(top10Players));
                        }
                    }
                }
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
        var fantasyMatchPlayerPoints = await _fantasyMatchPlayerRepository.GetFantasyPlayerPointsByMatchAsync(fantasyMatch.MatchId);
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
            leaderboardString += $"{i}.  {top10Drafts[i - 1].DiscordName}\n";
        }

        return $"""
```
Updated Leaderboard for Fantasy League: {top10Drafts.First().FantasyDraft.FantasyLeague!.Name}
{leaderboardString}
```
""";
    }
}
