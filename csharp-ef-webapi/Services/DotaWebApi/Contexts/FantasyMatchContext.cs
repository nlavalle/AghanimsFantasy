using csharp_ef_webapi.Data;
using csharp_ef_webapi.Models.GameCoordinator;
using csharp_ef_webapi.Models.Fantasy;
using csharp_ef_webapi.Models.WebApi;
using SteamKit2.GC.Dota.Internal;
using Microsoft.EntityFrameworkCore;
using csharp_ef_webapi.Models.ProMetadata;

namespace csharp_ef_webapi.Services;
internal class FantasyMatchContext : DotaOperationContext
{
    private readonly AghanimsFantasyContext _dbContext;
    private readonly ILogger<FantasyMatchContext> _logger;

    public FantasyMatchContext(ILogger<FantasyMatchContext> logger, IServiceScope scope, Config config)
        : base(scope, config)
    {
        _dbContext = scope.ServiceProvider.GetRequiredService<AghanimsFantasyContext>();
        _logger = logger;
    }

    protected override async Task OperateAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Load new match histories not in the FantasyMatch table (limit 50)
            List<MatchHistory> newMatchHistories = _dbContext.MatchHistory.Where(
                mh => !_dbContext.FantasyMatches.Any(fm => fm.MatchId == mh.MatchId))
                .Take(50)
                .ToList();

            if (newMatchHistories.Count > 0)
            {
                List<FantasyMatch> newHistoryFantasyMatches = new List<FantasyMatch>();
                foreach (MatchHistory newMatchHistory in newMatchHistories)
                {
                    FantasyMatch newFantasyMatch = new FantasyMatch()
                    {
                        MatchId = newMatchHistory.MatchId,
                        DireScore = null,
                        DireTeam = _dbContext.Teams.First(t => t.Id == newMatchHistory.DireTeamId),
                        Duration = null,
                        FirstBloodTime = null,
                        GameMode = null,
                        GcMetadataParsed = false,
                        LeagueId = newMatchHistory.LeagueId,
                        LobbyType = newMatchHistory.LobbyType,
                        MatchDetailParsed = false,
                        MatchHistoryParsed = true,
                        PreGameDuration = null,
                        RadiantScore = null,
                        RadiantTeam = _dbContext.Teams.First(t => t.Id == newMatchHistory.RadiantTeamId),
                        RadiantWin = null,
                        StartTime = newMatchHistory.StartTime
                    };

                    newHistoryFantasyMatches.Add(newFantasyMatch);
                }

                await _dbContext.FantasyMatches.AddRangeAsync(newHistoryFantasyMatches);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"{newMatchHistories.Count} new fantasy matches added to table");
            }


            // Check for gc match details and update if exists
            List<FantasyMatch> gcMatchDetailsToParse = _dbContext.FantasyMatches.Where(
                fm => !fm.MatchDetailParsed && _dbContext.GcDotaMatches.Any(md => (long)md.match_id == fm.MatchId))
                .Take(50)
                .ToList();

            if (gcMatchDetailsToParse.Count > 0)
            {
                foreach (FantasyMatch gcMatchDetailToParse in gcMatchDetailsToParse)
                {
                    CMsgDOTAMatch gcMatchDetailLookup = _dbContext.GcDotaMatches.First(md => (long)md.match_id == gcMatchDetailToParse.MatchId);
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
            List<FantasyMatch> matchDetailsToParse = _dbContext.FantasyMatches.Where(
                fm => !fm.MatchDetailParsed && _dbContext.MatchDetails.Any(md => md.MatchId == fm.MatchId))
                .Take(50)
                .ToList();

            if (matchDetailsToParse.Count > 0)
            {
                foreach (FantasyMatch matchDetailToParse in matchDetailsToParse)
                {
                    MatchDetail matchDetailLookup = _dbContext.MatchDetails.First(md => md.MatchId == matchDetailToParse.MatchId);
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
            var gcMatchPlayers = _dbContext.GcDotaMatches
                .SelectMany(match => match.players,
                (left, right) => new { Match = left, MatchPlayer = right })
                .Where(gcmp =>
                    !_dbContext.FantasyMatchPlayers.Any(fmp => fmp.MatchId == (long)gcmp.Match.match_id && fmp.Account != null && fmp.Account.Id == gcmp.MatchPlayer.account_id)
                    && (gcmp.Match.match_outcome == EMatchOutcome.k_EMatchOutcome_RadVictory || gcmp.Match.match_outcome == EMatchOutcome.k_EMatchOutcome_DireVictory) // Filter out cancelled games
                )
                .Take(500) // 10 players per match and 50 matches per pull so this feels equivalent
                .ToList();

            if (gcMatchPlayers.Count() > 0)
            {
                foreach (var gcMatchDetailPlayer in gcMatchPlayers)
                {
                    // We don't want to insert anything with nulls, so flag errors so I can catch them in the logging and add them in
                    Account? accountLookup = _dbContext.Accounts.Find((long)gcMatchDetailPlayer.MatchPlayer.account_id);
                    if (accountLookup == null)
                    {
                        _logger.LogError($"Missing Account {gcMatchDetailPlayer.MatchPlayer.account_id}, need to add it to nadcl.dota_accounts");
                        continue;
                    }

                    Hero? heroLookup = _dbContext.Heroes.Find((long)gcMatchDetailPlayer.MatchPlayer.hero_id);
                    if (heroLookup == null)
                    {
                        _logger.LogError($"Missing Hero {gcMatchDetailPlayer.MatchPlayer.hero_id}, need to add it to nadcl.dota_heroes");
                        continue;
                    }

                    long teamId = gcMatchDetailPlayer.MatchPlayer.team_number == DOTA_GC_TEAM.DOTA_GC_TEAM_BAD_GUYS ? (long)gcMatchDetailPlayer.Match.dire_team_id : (long)gcMatchDetailPlayer.Match.radiant_team_id;
                    Team? teamLookup = _dbContext.Teams.Find(teamId);
                    if (teamLookup == null)
                    {
                        _logger.LogError($"Missing Team {teamId}, need to add it to nadcl.dota_teams");
                        continue;
                    }

                    FantasyMatch? fantasyMatch = _dbContext.FantasyMatches.Find((long)gcMatchDetailPlayer.Match.match_id);
                    if (fantasyMatch == null)
                    {
                        _logger.LogInformation($"Missing Match parent {gcMatchDetailPlayer.Match.match_id}, skipping and will retry.");
                        continue;
                    }


                    FantasyMatchPlayer newPlayer = new FantasyMatchPlayer()
                    {
                        Account = accountLookup,
                        Assists = (int)gcMatchDetailPlayer.MatchPlayer.assists,
                        // CampsStacked = ,
                        Deaths = (int)gcMatchDetailPlayer.MatchPlayer.deaths,
                        Denies = (int)gcMatchDetailPlayer.MatchPlayer.denies,
                        // Dewards = ,
                        DotaTeamSide = (int)gcMatchDetailPlayer.MatchPlayer.team_number == 1,
                        Gold = (int)gcMatchDetailPlayer.MatchPlayer.gold,
                        GoldPerMin = (int)gcMatchDetailPlayer.MatchPlayer.gold_per_min,
                        Hero = heroLookup,
                        HeroDamage = (int)gcMatchDetailPlayer.MatchPlayer.hero_damage,
                        HeroHealing = (int)gcMatchDetailPlayer.MatchPlayer.hero_healing,
                        Kills = (int)gcMatchDetailPlayer.MatchPlayer.kills,
                        LastHits = (int)gcMatchDetailPlayer.MatchPlayer.last_hits,
                        Level = (int)gcMatchDetailPlayer.MatchPlayer.level,
                        MatchDetailPlayerParsed = true,
                        MatchId = (long)gcMatchDetailPlayer.Match.match_id,
                        Networth = (int)gcMatchDetailPlayer.MatchPlayer.net_worth,
                        // ObserverWardsPlaced = ,
                        PlayerSlot = (int)gcMatchDetailPlayer.MatchPlayer.player_slot,
                        // SentyWardsPlaced = ,
                        // StunDuration = ,
                        SupportGoldSpent = (int)gcMatchDetailPlayer.MatchPlayer.support_gold,
                        Team = teamLookup,
                        TowerDamage = (int)gcMatchDetailPlayer.MatchPlayer.tower_damage,
                        XpPerMin = (int)gcMatchDetailPlayer.MatchPlayer.xp_per_min,
                    };

                    _dbContext.FantasyMatchPlayers.Add(newPlayer);
                }

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"{gcMatchPlayers.Count} new fantasy match players added to table");
            }
            // Check for match metadata player and update match player if exists
            List<FantasyMatchPlayer> metadataPlayersToParse = _dbContext.FantasyMatchPlayers.Where(
                fm => !fm.GcMetadataPlayerParsed && _dbContext.GcMatchMetadata.Any(md => md.MatchId == fm.MatchId))
                .Take(50)
                .ToList();

            if (metadataPlayersToParse.Count > 0)
            {
                foreach (FantasyMatchPlayer metadataPlayerToParse in metadataPlayersToParse)
                {
                    var matchMetadataLookup = await _dbContext.GcMatchMetadata
                        .Where(md => md.MatchId == metadataPlayerToParse.MatchId)
                        .SelectMany(md => md.Teams,
                        (left, right) => new { Match = left, Team = right })
                        .SelectMany(mdt => mdt.Team.Players,
                        (left, right) => new { Match = left.Match, Team = left.Team, Player = right })
                        .Where(mdp => mdp.Match.MatchId == metadataPlayerToParse.MatchId && mdp.Player.PlayerSlot == metadataPlayerToParse.PlayerSlot)
                        .FirstAsync();

                    metadataPlayerToParse.CampsStacked = (int)matchMetadataLookup.Player.CampsStacked;
                    metadataPlayerToParse.Dewards = (int)matchMetadataLookup.Player.WardsDewarded;
                    metadataPlayerToParse.GcMetadataPlayerParsed = true;
                    metadataPlayerToParse.ObserverWardsPlaced = (int)matchMetadataLookup.Player.ObserverWardsPlaced;
                    metadataPlayerToParse.SentyWardsPlaced = (int)matchMetadataLookup.Player.SentryWardsPlaced;
                    metadataPlayerToParse.StunDuration = (int)matchMetadataLookup.Player.StunDuration;
                    metadataPlayerToParse.FightScore = matchMetadataLookup.Player.FightScore;
                    metadataPlayerToParse.FarmScore = matchMetadataLookup.Player.FarmScore;
                    metadataPlayerToParse.SupportScore = matchMetadataLookup.Player.SupportScore;
                    metadataPlayerToParse.PushScore = matchMetadataLookup.Player.PushScore;
                    metadataPlayerToParse.HeroXp = matchMetadataLookup.Player.HeroXp;
                    metadataPlayerToParse.Rampages = matchMetadataLookup.Player.Rampages;
                    metadataPlayerToParse.TripleKills = matchMetadataLookup.Player.TripleKills;
                    metadataPlayerToParse.AegisSnatched = matchMetadataLookup.Player.AegisSnatched;
                    metadataPlayerToParse.RapiersPurchased = matchMetadataLookup.Player.RapiersPurchased;
                    metadataPlayerToParse.CouriersKilled = matchMetadataLookup.Player.CouriersKilled;
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
