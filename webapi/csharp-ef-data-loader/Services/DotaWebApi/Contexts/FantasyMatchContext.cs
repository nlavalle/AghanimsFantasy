namespace csharp_ef_data_loader.Services;

using DataAccessLibrary.Data;
using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.WebApi;
using DataAccessLibrary.Models.ProMetadata;
using SteamKit2.GC.Dota.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

internal class FantasyMatchContext : DotaOperationContext
{
    private readonly IProMetadataRepository _proMetadataRepository;
    private readonly IFantasyMatchRepository _fantasyMatchRepository;
    private readonly IFantasyMatchPlayerRepository _fantasyMatchPlayerRepository;
    private readonly IGcDotaMatchRepository _gcDotaMatchRepository;
    private readonly IMatchHistoryRepository _matchHistoryRepository;
    private readonly IMatchDetailRepository _matchDetailRepository;
    private readonly GameCoordinatorRepository _gameCoordinatorRepository;
    private readonly ILogger<FantasyMatchContext> _logger;

    public FantasyMatchContext(
            ILogger<FantasyMatchContext> logger,
            IProMetadataRepository proMetadataRepository,
            IFantasyMatchRepository fantasyMatchRepository,
            IFantasyMatchPlayerRepository fantasyMatchPlayerRepository,
            IGcDotaMatchRepository gcDotaMatchRepository,
            IMatchHistoryRepository matchHistoryRepository,
            IMatchDetailRepository matchDetailRepository,
            GameCoordinatorRepository gameCoordinatorRepository,
            IServiceScope scope,
            Config config
        )
        : base(scope, config)
    {
        _logger = logger;
        _proMetadataRepository = proMetadataRepository;
        _fantasyMatchRepository = fantasyMatchRepository;
        _fantasyMatchPlayerRepository = fantasyMatchPlayerRepository;
        _gcDotaMatchRepository = gcDotaMatchRepository;
        _matchHistoryRepository = matchHistoryRepository;
        _matchDetailRepository = matchDetailRepository;
        _gameCoordinatorRepository = gameCoordinatorRepository;
    }

    protected override async Task OperateAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Load new match histories not in the FantasyMatch table (limit 50)
            List<MatchHistory> newMatchHistories = await _matchHistoryRepository.GetNotInFantasyMatches(50);

            if (newMatchHistories.Count > 0)
            {
                List<FantasyMatch> newHistoryFantasyMatches = new List<FantasyMatch>();
                foreach (MatchHistory newMatchHistory in newMatchHistories)
                {
                    FantasyMatch newFantasyMatch = new FantasyMatch()
                    {
                        MatchId = newMatchHistory.MatchId,
                        DireScore = null,
                        DireTeam = await _proMetadataRepository.GetTeamAsync(newMatchHistory.DireTeamId),
                        Duration = null,
                        FirstBloodTime = null,
                        GameMode = null,
                        GcMetadataParsed = false,
                        LeagueId = newMatchHistory.LeagueId,
                        League = newMatchHistory.League,
                        LobbyType = newMatchHistory.LobbyType,
                        MatchDetailParsed = false,
                        MatchHistoryParsed = true,
                        PreGameDuration = null,
                        RadiantScore = null,
                        RadiantTeam = await _proMetadataRepository.GetTeamAsync(newMatchHistory.RadiantTeamId),
                        RadiantWin = null,
                        StartTime = newMatchHistory.StartTime
                    };

                    newHistoryFantasyMatches.Add(newFantasyMatch);
                }

                await _fantasyMatchRepository.AddRangeAsync(newHistoryFantasyMatches);

                _logger.LogInformation($"{newMatchHistories.Count} new fantasy matches added to table");
            }

            // Check for gc match details and update if exists
            List<FantasyMatch> gcMatchDetailsToParse = await _fantasyMatchRepository.GetNotGcDetailParsed(50);

            if (gcMatchDetailsToParse.Count > 0)
            {
                foreach (FantasyMatch gcMatchDetailToParse in gcMatchDetailsToParse)
                {
                    CMsgDOTAMatch gcMatchDetailLookup = await _gcDotaMatchRepository.GetByIdAsync((ulong)gcMatchDetailToParse.MatchId) ?? throw new Exception($"Expected to find Game Coordinator data for {gcMatchDetailToParse.MatchId}");
                    gcMatchDetailToParse.DireScore = (int)gcMatchDetailLookup.dire_team_score;
                    gcMatchDetailToParse.Duration = (int)gcMatchDetailLookup.duration;
                    gcMatchDetailToParse.FirstBloodTime = (int)gcMatchDetailLookup.first_blood_time;
                    gcMatchDetailToParse.GameMode = (int)gcMatchDetailLookup.game_mode;
                    gcMatchDetailToParse.MatchDetailParsed = true;
                    gcMatchDetailToParse.PreGameDuration = (int)gcMatchDetailLookup.pre_game_duration;
                    gcMatchDetailToParse.RadiantScore = (int)gcMatchDetailLookup.radiant_team_score;
                    gcMatchDetailToParse.RadiantWin = gcMatchDetailLookup.match_outcome == EMatchOutcome.k_EMatchOutcome_RadVictory;
                }

                await _fantasyMatchRepository.UpdateRangeAsync(gcMatchDetailsToParse);

                _logger.LogInformation($"{gcMatchDetailsToParse.Count} matches updated with GC Match Detail data");
            }

            // Check for match details and update if exists
            // Call this second, we prefer the GC match detail data if we have it there, this is a backup in case one of them goes down
            List<FantasyMatch> matchDetailsToParse = await _fantasyMatchRepository.GetNotDetailParsed(50);

            if (matchDetailsToParse.Count > 0)
            {
                foreach (FantasyMatch matchDetailToParse in matchDetailsToParse)
                {
                    MatchDetail matchDetailLookup = await _matchDetailRepository.GetByIdAsync(matchDetailToParse.MatchId) ?? throw new Exception($"Expected Match Detail for {matchDetailToParse.MatchId}");
                    matchDetailToParse.DireScore = matchDetailLookup.DireScore;
                    matchDetailToParse.Duration = matchDetailLookup.Duration;
                    matchDetailToParse.FirstBloodTime = matchDetailLookup.FirstBloodTime;
                    matchDetailToParse.GameMode = matchDetailLookup.GameMode;
                    matchDetailToParse.MatchDetailParsed = true;
                    matchDetailToParse.PreGameDuration = matchDetailLookup.PreGameDuration;
                    matchDetailToParse.RadiantScore = matchDetailLookup.RadiantScore;
                    matchDetailToParse.RadiantWin = matchDetailLookup.RadiantWin;
                }

                await _fantasyMatchRepository.UpdateRangeAsync(matchDetailsToParse);

                _logger.LogInformation($"{matchDetailsToParse.Count} matches updated with WebApi Match Detail data");
            }

            // Check for gc match detail player and add match player if exists
            List<(CMsgDOTAMatch, CMsgDOTAMatch.Player)> gcMatchPlayers = await _gcDotaMatchRepository.GetNotInFantasyMatchPlayers(500);

            if (gcMatchPlayers.Count() > 0)
            {
                foreach ((CMsgDOTAMatch, CMsgDOTAMatch.Player) gcMatchDetailPlayer in gcMatchPlayers)
                {
                    // We don't want to insert anything with nulls, so flag errors so I can catch them in the logging and add them in
                    Account? accountLookup = await _proMetadataRepository.GetPlayerAccount(gcMatchDetailPlayer.Item2.account_id);
                    if (accountLookup == null)
                    {
                        _logger.LogError($"Missing Account {gcMatchDetailPlayer.Item2.account_id}, need to add it to nadcl.dota_accounts");
                        continue;
                    }

                    Hero? heroLookup = await _proMetadataRepository.GetHeroAsync(gcMatchDetailPlayer.Item2.hero_id);
                    if (heroLookup == null)
                    {
                        _logger.LogError($"Missing Hero {gcMatchDetailPlayer.Item2.hero_id}, need to add it to nadcl.dota_heroes");
                        continue;
                    }

                    long teamId = gcMatchDetailPlayer.Item2.team_number == DOTA_GC_TEAM.DOTA_GC_TEAM_BAD_GUYS ? gcMatchDetailPlayer.Item1.dire_team_id : gcMatchDetailPlayer.Item1.radiant_team_id;
                    Team? teamLookup = await _proMetadataRepository.GetTeamAsync(teamId);
                    if (teamLookup == null)
                    {
                        _logger.LogError($"Missing Team {teamId}, need to add it to nadcl.dota_teams");
                        continue;
                    }

                    FantasyMatch? fantasyMatch = await _fantasyMatchRepository.GetByIdAsync((long)gcMatchDetailPlayer.Item1.match_id);
                    if (fantasyMatch == null)
                    {
                        _logger.LogInformation($"Missing Match parent {gcMatchDetailPlayer.Item1.match_id}, skipping and will retry.");
                        continue;
                    }


                    FantasyMatchPlayer newPlayer = new FantasyMatchPlayer()
                    {
                        AccountId = accountLookup.Id,
                        Account = accountLookup,
                        Assists = (int)gcMatchDetailPlayer.Item2.assists,
                        // CampsStacked = ,
                        Deaths = (int)gcMatchDetailPlayer.Item2.deaths,
                        Denies = (int)gcMatchDetailPlayer.Item2.denies,
                        // Dewards = ,
                        DotaTeamSide = (int)gcMatchDetailPlayer.Item2.team_number == 1,
                        Gold = (int)gcMatchDetailPlayer.Item2.gold,
                        GoldPerMin = (int)gcMatchDetailPlayer.Item2.gold_per_min,
                        Hero = heroLookup,
                        HeroDamage = (int)gcMatchDetailPlayer.Item2.hero_damage,
                        HeroHealing = (int)gcMatchDetailPlayer.Item2.hero_healing,
                        Kills = (int)gcMatchDetailPlayer.Item2.kills,
                        LastHits = (int)gcMatchDetailPlayer.Item2.last_hits,
                        Level = (int)gcMatchDetailPlayer.Item2.level,
                        MatchDetailPlayerParsed = true,
                        FantasyMatchId = fantasyMatch.MatchId,
                        Match = fantasyMatch,
                        Networth = (int)gcMatchDetailPlayer.Item2.net_worth,
                        // ObserverWardsPlaced = ,
                        PlayerSlot = (int)gcMatchDetailPlayer.Item2.player_slot,
                        // SentyWardsPlaced = ,
                        // StunDuration = ,
                        SupportGoldSpent = (int)gcMatchDetailPlayer.Item2.support_gold,
                        TeamId = teamLookup.Id,
                        Team = teamLookup,
                        TowerDamage = (int)gcMatchDetailPlayer.Item2.tower_damage,
                        XpPerMin = (int)gcMatchDetailPlayer.Item2.xp_per_min,
                    };

                    await _fantasyMatchPlayerRepository.AddAsync(newPlayer);
                }

                _logger.LogInformation($"{gcMatchPlayers.Count} new fantasy match players added to table");
            }
            // Check for match metadata player and update match player if exists
            List<FantasyMatchPlayer> metadataPlayersToParse = await _fantasyMatchPlayerRepository.GetNotGcDetailParsed(500);

            if (metadataPlayersToParse.Count > 0)
            {
                foreach (FantasyMatchPlayer metadataPlayerToParse in metadataPlayersToParse)
                {
                    var matchMetadataLookup = await _gameCoordinatorRepository.GetMatchMetadataPlayerAsync(metadataPlayerToParse);

                    metadataPlayerToParse.CampsStacked = (int)matchMetadataLookup.CampsStacked;
                    metadataPlayerToParse.Dewards = (int)matchMetadataLookup.WardsDewarded;
                    metadataPlayerToParse.GcMetadataPlayerParsed = true;
                    metadataPlayerToParse.ObserverWardsPlaced = (int)matchMetadataLookup.ObserverWardsPlaced;
                    metadataPlayerToParse.SentyWardsPlaced = (int)matchMetadataLookup.SentryWardsPlaced;
                    metadataPlayerToParse.StunDuration = (int)matchMetadataLookup.StunDuration;
                    metadataPlayerToParse.FightScore = matchMetadataLookup.FightScore;
                    metadataPlayerToParse.FarmScore = matchMetadataLookup.FarmScore;
                    metadataPlayerToParse.SupportScore = matchMetadataLookup.SupportScore;
                    metadataPlayerToParse.PushScore = matchMetadataLookup.PushScore;
                    metadataPlayerToParse.HeroXp = matchMetadataLookup.HeroXp;
                    metadataPlayerToParse.Rampages = matchMetadataLookup.Rampages;
                    metadataPlayerToParse.TripleKills = matchMetadataLookup.TripleKills;
                    metadataPlayerToParse.AegisSnatched = matchMetadataLookup.AegisSnatched;
                    metadataPlayerToParse.RapiersPurchased = matchMetadataLookup.RapiersPurchased;
                    metadataPlayerToParse.CouriersKilled = matchMetadataLookup.CouriersKilled;
                }

                await _fantasyMatchPlayerRepository.UpdateRangeAsync(metadataPlayersToParse);

                _logger.LogInformation($"{metadataPlayersToParse.Count} players updated with Metadata Detail data");
            }

            _logger.LogInformation($"Fantasy Match/Match Player tables updated");
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred: {ex.Message}");
        }
    }
}
