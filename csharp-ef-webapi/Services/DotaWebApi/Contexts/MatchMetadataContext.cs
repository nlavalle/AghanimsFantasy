using System.Collections.Immutable;
using System.Diagnostics;
using csharp_ef_webapi.Data;
using csharp_ef_webapi.Models;
using ICSharpCode.SharpZipLib.BZip2;
using Microsoft.EntityFrameworkCore;
using ProtoBuf;
using SteamKit2.GC.Dota.Internal;

namespace csharp_ef_webapi.Services;
internal class MatchMetadataContext : DotaOperationContext
{
    private readonly AghanimsFantasyContext _dbContext;
    private readonly ILogger<MatchDetailsContext> _logger;
    private readonly HttpClient _httpClient;

    // Our match history list: just going to use a locked list, contention should be very low
    private readonly List<GcMatchMetadata> _matches = new List<GcMatchMetadata>();

    // Status values
    private int _remainingMatches = 0;
    private int _startedMatches = 0;
    private int _totalMatches = 0;

    public MatchMetadataContext(ILogger<MatchDetailsContext> logger, HttpClient httpClient, IServiceScope scope, Config config)
        : base(scope, config)
    {
        _dbContext = scope.ServiceProvider.GetRequiredService<AghanimsFantasyContext>();
        _logger = logger;
        _httpClient = httpClient;
    }

    protected override async Task OperateAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Find all the match histories without match detail rows and add tasks to fetch them all
            // Take 50 at a time since this runs every 5 minutes, no reason to blast it all at once
            List<CMsgDOTAMatch> matchesWithoutDetails = _dbContext.GcDotaMatches
                .Where(gcdm => gcdm.replay_state == CMsgDOTAMatch.ReplayState.REPLAY_AVAILABLE &&
                    !_dbContext.GcMatchMetadata.Any(gcmm => (ulong)gcmm.MatchId == gcdm.match_id))
                .Take(50)
                .ToList();

            if (matchesWithoutDetails.Count() > 0)
            {
                var length = matchesWithoutDetails.Count;

                // Knowing the length triggers a lot of stuff
                _totalMatches = length;
                Volatile.Write(ref _remainingMatches, length);
                _logger.LogInformation($"Dota Web API - Fetching {matchesWithoutDetails.Count()} new match metadata.");

                List<Task<GcMatchMetadata?>> fetchMatchMetadataTasks = new List<Task<GcMatchMetadata?>>();

                Task[] tasks = new Task[length];

                for (int i = 0; i < length; i++)
                {
                    var match = matchesWithoutDetails[i];

                    tasks[i] = GetMatchMetadataAsync(match, cancellationToken);
                }

                await Task.WhenAll(tasks);

                foreach (GcMatchMetadata matchDetail in _matches)
                {
                    Debug.Assert(_dbContext.MatchDetails.Any(md => md.MatchId == matchDetail.MatchId));
                    _dbContext.GcMatchMetadata.Add(matchDetail);
                }
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Dota Web API - Missing match metadata fetch done");

            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred: {ex.Message}");
        }
    }

    private async Task GetMatchMetadataAsync(CMsgDOTAMatch match, CancellationToken cancellationToken)
    {
        try
        {
            UriBuilder uriBuilder = new UriBuilder($"http://replay{match.cluster}.valve.net/570/{match.match_id}_{match.replay_salt}.meta.bz2");

            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

            await WaitNextTaskScheduleAsync(cancellationToken);
            HttpResponseMessage response = await _httpClient.SendAsync(httpRequest);
            _logger.LogInformation($"Request submitted at {DateTime.Now.Ticks}");
            response.EnsureSuccessStatusCode();

            CDOTAMatchMetadataFile matchMetadata;

            // Decompress the bz2
            using (var compressedStream = await response.Content.ReadAsStreamAsync())
            using (var decompressedStream = new BZip2InputStream(compressedStream))
            {
                matchMetadata = Serializer.Deserialize<CDOTAMatchMetadataFile>(decompressedStream);
                Debug.Assert(matchMetadata != null);
            }

            var matchResponse = CastGcToModel(matchMetadata.metadata);

            matchResponse.MatchId = (long)match.match_id;

            lock (_matches)
            {
                _matches.Add(matchResponse);
            }
        }
        catch (Exception ex)
        {
            // Log error but we don't want to stop the whole job because one replay failed to retrieve
            _logger.LogError($"GetMatchMetadataAsync - An error occurred: {ex.Message}");
        }
        finally
        {
            Interlocked.Add(ref _startedMatches, 1);
            Interlocked.Decrement(ref _remainingMatches);
        }


        // // If we want to do something when the process is done, we can use this instead
        // if (Interlocked.Decrement(ref _remainingLeagues) == 0)
        // {
        // }
    }

    public string GetMatchDetailFetchStatus()
    {
        Interlocked.MemoryBarrier();

        int totalMatches = _totalMatches;
        int completeLeagues = totalMatches - _remainingMatches;
        return $"{completeLeagues} of {totalMatches} missing match details fetched";
    }

    private GcMatchMetadata CastGcToModel(CDOTAMatchMetadata matchMetadata)
    {
        GcMatchMetadata gcMatchMetadata = new GcMatchMetadata
        {
            LobbyId = matchMetadata.lobby_id,
            ReportUntilTime = matchMetadata.report_until_time,
            PrimaryEventId = matchMetadata.primary_event_id,
            Teams = new List<GcMatchMetadataTeam>(),
            MatchTips = new List<GcMatchMetadataTip>()
        };

        foreach (var metadataTeam in matchMetadata.teams)
        {
            GcMatchMetadataTeam gcMatchmetadataTeam = new GcMatchMetadataTeam
            {
                CmCaptainPlayerId = metadataTeam.cm_captain_player_id,
                CmFirstPick = metadataTeam.cm_first_pick,
                CmPenalty = metadataTeam.cm_penalty,
                DotaTeam = metadataTeam.dota_team,
                GraphExperience = metadataTeam.graph_experience,
                GraphGoldEarned = metadataTeam.graph_gold_earned,
                GraphNetworth = metadataTeam.graph_net_worth,
                Players = new List<GcMatchMetadataPlayer>()
            };

            foreach (var metadataPlayer in metadataTeam.players)
            {
                GcMatchMetadataPlayer gcMatchMetadataPlayer = new GcMatchMetadataPlayer
                {
                    AbilityUpgrades = metadataPlayer.ability_upgrades,
                    AegisSnatched = metadataPlayer.aegis_snatched,
                    AvgAssistsX16 = metadataPlayer.avg_assists_x16,
                    AvgDeathsX16 = metadataPlayer.avg_deaths_x16,
                    AvgGpmX16 = metadataPlayer.avg_gpm_x16,
                    AvgKillsX16 = metadataPlayer.avg_kills_x16,
                    AvgStatsCalibrated = metadataPlayer.avg_stats_calibrated,
                    AvgXpmX16 = metadataPlayer.avg_xpm_x16,
                    BestAssistsX16 = metadataPlayer.best_assists_x16,
                    BestGpmX16 = metadataPlayer.best_gpm_x16,
                    BestKillsX16 = metadataPlayer.best_kills_x16,
                    BestWinStreak = metadataPlayer.best_win_streak,
                    BestXpmX16 = metadataPlayer.best_xpm_x16,
                    CampsStacked = metadataPlayer.camps_stacked,
                    CouriersKilled = metadataPlayer.couriers_killed,
                    FarmScore = metadataPlayer.farm_score,
                    FeaturedHeroStickerIndex = metadataPlayer.featured_hero_sticker_index,
                    FeaturedHeroStickerQuality = metadataPlayer.featured_hero_sticker_quality,
                    FightScore = metadataPlayer.fight_score,
                    GamePlayerId = metadataPlayer.game_player_id,
                    HeroXp = metadataPlayer.hero_xp,
                    Items = new List<GcMatchMetadataItemPurchase>(),
                    Kills = new List<GcMatchMetadataPlayerKill>(),
                    LaneSelectionFlags = metadataPlayer.lane_selection_flags,
                    NetworthRank = metadataPlayer.net_worth_rank,
                    ObserverWardsPlaced = metadataPlayer.observer_wards_placed,
                    PlayerSlot = metadataPlayer.player_slot,
                    PushScore = metadataPlayer.push_score,
                    Rampages = metadataPlayer.rampages,
                    RapiersPurchased = metadataPlayer.rapiers_purchased,
                    SentryWardsPlaced = metadataPlayer.sentry_wards_placed,
                    StunDuration = metadataPlayer.stun_duration,
                    SupportGoldSpent = metadataPlayer.support_gold_spent,
                    SupportScore = metadataPlayer.support_score,
                    TeamSlot = metadataPlayer.team_slot,
                    TripleKills = metadataPlayer.triple_kills,
                    WardsDewarded = metadataPlayer.wards_dewarded,
                    WinStreak = metadataPlayer.win_streak
                };

                foreach (var metadataItem in metadataPlayer.items)
                {
                    GcMatchMetadataItemPurchase gcMatchMetadataItemPurchase = new GcMatchMetadataItemPurchase
                    {
                        ItemId = (uint)metadataItem.item_id,
                        PurchaseTime = (uint)metadataItem.purchase_time
                    };
                    gcMatchMetadataPlayer.Items.Add(gcMatchMetadataItemPurchase);
                }

                foreach (var metadataKill in metadataPlayer.kills)
                {
                    GcMatchMetadataPlayerKill gcMatchMetadataPlayerKill = new GcMatchMetadataPlayerKill
                    {
                        VictimSlot = metadataKill.victim_slot,
                        Count = metadataKill.count
                    };
                    gcMatchMetadataPlayer.Kills.Add(gcMatchMetadataPlayerKill);
                }

                gcMatchmetadataTeam.Players.Add(gcMatchMetadataPlayer);
            }
            gcMatchMetadata.Teams.Add(gcMatchmetadataTeam);
        }

        foreach (var metadataTip in matchMetadata.match_tips)
        {
            GcMatchMetadataTip gcMatchMetadataTip = new GcMatchMetadataTip
            {
                SourcePlayerSlot = metadataTip.source_player_slot,
                TargetPlayerSlot = metadataTip.target_player_slot,
                TipAmount = metadataTip.tip_amount
            };
            gcMatchMetadata.MatchTips.Add(gcMatchMetadataTip);
        }

        return gcMatchMetadata;
    }
}
