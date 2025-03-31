namespace csharp_ef_data_loader.Services;

using DataAccessLibrary.Data;
using DataAccessLibrary.Data.Facades;
using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.WebApi;
using DataAccessLibrary.Models.ProMetadata;
using SteamKit2.GC.Dota.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

internal class FantasyMatchContext : DotaOperationContext
{
    private readonly ILogger<FantasyMatchContext> _logger;
    private readonly AghanimsFantasyContext _dbContext;
    private readonly FantasyMatchFacade _fantasyMatchFacade;

    public FantasyMatchContext(
            ILogger<FantasyMatchContext> logger,
            AghanimsFantasyContext dbContext,
            FantasyMatchFacade fantasyMatchFacade,
            IServiceScope scope,
            Config config
        )
        : base(scope, config)
    {
        _logger = logger;
        _dbContext = dbContext;
        _fantasyMatchFacade = fantasyMatchFacade;
    }

    protected override async Task OperateAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Load new match histories not in the FantasyMatch table (limit 50)
            List<MatchHistory> newMatchHistories = await _fantasyMatchFacade.GetNotInFantasyMatches(50);

            if (newMatchHistories.Count > 0)
            {
                List<FantasyMatch> newHistoryFantasyMatches = new List<FantasyMatch>();
                foreach (MatchHistory newMatchHistory in newMatchHistories)
                {
                    FantasyMatch newFantasyMatch = new FantasyMatch()
                    {
                        MatchId = newMatchHistory.MatchId,
                        DireScore = null,
                        DireTeam = await _dbContext.Teams.FindAsync(newMatchHistory.DireTeamId),
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
                        RadiantTeam = await _dbContext.Teams.FindAsync(newMatchHistory.RadiantTeamId),
                        RadiantWin = null,
                        StartTime = newMatchHistory.StartTime
                    };

                    newHistoryFantasyMatches.Add(newFantasyMatch);
                }

                await _dbContext.FantasyMatches.AddRangeAsync(newHistoryFantasyMatches);

                _logger.LogInformation($"{newMatchHistories.Count} new fantasy matches added to table");
            }

            // Check for gc match details and update if exists
            List<FantasyMatch> gcMatchDetailsToParse = await _fantasyMatchFacade.GetMatchNotGcDetailParsed(50);

            if (gcMatchDetailsToParse.Count > 0)
            {
                foreach (FantasyMatch gcMatchDetailToParse in gcMatchDetailsToParse)
                {
                    CMsgDOTAMatch gcMatchDetailLookup = await _dbContext.GcDotaMatches.FindAsync((ulong)gcMatchDetailToParse.MatchId) ?? throw new Exception($"Expected to find Game Coordinator data for {gcMatchDetailToParse.MatchId}");
                    gcMatchDetailToParse.DireScore = (int)gcMatchDetailLookup.dire_team_score;
                    gcMatchDetailToParse.Duration = (int)gcMatchDetailLookup.duration;
                    gcMatchDetailToParse.FirstBloodTime = (int)gcMatchDetailLookup.first_blood_time;
                    gcMatchDetailToParse.GameMode = (int)gcMatchDetailLookup.game_mode;
                    gcMatchDetailToParse.MatchDetailParsed = true;
                    gcMatchDetailToParse.PreGameDuration = (int)gcMatchDetailLookup.pre_game_duration;
                    gcMatchDetailToParse.RadiantScore = (int)gcMatchDetailLookup.radiant_team_score;
                    gcMatchDetailToParse.RadiantWin = gcMatchDetailLookup.match_outcome == EMatchOutcome.k_EMatchOutcome_RadVictory;
                }

                _dbContext.FantasyMatches.UpdateRange(gcMatchDetailsToParse);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"{gcMatchDetailsToParse.Count} matches updated with GC Match Detail data");
            }

            // Check for match details and update if exists
            // Call this second, we prefer the GC match detail data if we have it there, this is a backup in case one of them goes down
            List<FantasyMatch> matchDetailsToParse = await _fantasyMatchFacade.GetNotDetailParsed(50);

            if (matchDetailsToParse.Count > 0)
            {
                foreach (FantasyMatch matchDetailToParse in matchDetailsToParse)
                {
                    MatchDetail matchDetailLookup = await _dbContext.MatchDetails.FindAsync(matchDetailToParse.MatchId) ?? throw new Exception($"Expected Match Detail for {matchDetailToParse.MatchId}");
                    matchDetailToParse.DireScore = matchDetailLookup.DireScore;
                    matchDetailToParse.Duration = matchDetailLookup.Duration;
                    matchDetailToParse.FirstBloodTime = matchDetailLookup.FirstBloodTime;
                    matchDetailToParse.GameMode = matchDetailLookup.GameMode;
                    matchDetailToParse.MatchDetailParsed = true;
                    matchDetailToParse.PreGameDuration = matchDetailLookup.PreGameDuration;
                    matchDetailToParse.RadiantScore = matchDetailLookup.RadiantScore;
                    matchDetailToParse.RadiantWin = matchDetailLookup.RadiantWin;
                }

                _dbContext.FantasyMatches.UpdateRange(matchDetailsToParse);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"{matchDetailsToParse.Count} matches updated with WebApi Match Detail data");
            }

            // Check for gc match detail player and add match player if exists
            List<(CMsgDOTAMatch, CMsgDOTAMatch.Player)> gcMatchPlayers = await _fantasyMatchFacade.GetNotInFantasyMatchPlayers(500);

            if (gcMatchPlayers.Count() > 0)
            {
                foreach ((CMsgDOTAMatch, CMsgDOTAMatch.Player) gcMatchDetailPlayer in gcMatchPlayers)
                {
                    // We don't want to insert anything with nulls, so flag errors so I can catch them in the logging and add them in
                    Account? accountLookup = await _dbContext.Accounts.FindAsync((long)gcMatchDetailPlayer.Item2.account_id);
                    if (accountLookup == null)
                    {
                        _logger.LogError($"Missing Account {gcMatchDetailPlayer.Item2.account_id}, need to add it to nadcl.dota_accounts");
                        continue;
                    }

                    Hero? heroLookup = await _dbContext.Heroes.FindAsync((long)gcMatchDetailPlayer.Item2.hero_id);
                    if (heroLookup == null)
                    {
                        _logger.LogError($"Missing Hero {gcMatchDetailPlayer.Item2.hero_id}, need to add it to nadcl.dota_heroes");
                        continue;
                    }

                    long teamId = gcMatchDetailPlayer.Item2.team_number == DOTA_GC_TEAM.DOTA_GC_TEAM_BAD_GUYS ? gcMatchDetailPlayer.Item1.dire_team_id : gcMatchDetailPlayer.Item1.radiant_team_id;
                    Team? teamLookup = await _dbContext.Teams.FindAsync(teamId);
                    if (teamLookup == null)
                    {
                        _logger.LogError($"Missing Team {teamId}, need to add it to nadcl.dota_teams");
                        continue;
                    }

                    FantasyMatch? fantasyMatch = await _dbContext.FantasyMatches.FindAsync((long)gcMatchDetailPlayer.Item1.match_id);
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

                    await _dbContext.FantasyMatchPlayers.AddAsync(newPlayer);
                    await _dbContext.SaveChangesAsync();
                }

                _logger.LogInformation($"{gcMatchPlayers.Count} new fantasy match players added to table");
            }
            // Check for match metadata player and update match player if exists
            List<FantasyMatchPlayer> metadataPlayersToParse = await _fantasyMatchFacade.GetMatchPlayerNotGcDetailParsed(500);

            if (metadataPlayersToParse.Count > 0)
            {
                foreach (FantasyMatchPlayer metadataPlayerToParse in metadataPlayersToParse)
                {
                    var matchMetadataLookup = await _dbContext.GcMatchMetadata
                        .Where(md => md.MatchId == metadataPlayerToParse.FantasyMatchId)
                        .SelectMany(md => md.Teams,
                        (left, right) => new { Match = left, Team = right })
                        .SelectMany(mdt => mdt.Team.Players,
                        (left, right) => new { Match = left.Match, Team = left.Team, Player = right })
                        .Where(mdp => mdp.Match.MatchId == metadataPlayerToParse.FantasyMatchId && mdp.Player.PlayerSlot == metadataPlayerToParse.PlayerSlot)
                        .Select(result => result.Player)
                        .FirstAsync();

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

                _dbContext.FantasyMatchPlayers.UpdateRange(metadataPlayersToParse);
                await _dbContext.SaveChangesAsync();

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
