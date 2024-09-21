using Moq;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DataAccessLibrary.Models.ProMetadata;
using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.WebApi;
using DataAccessLibrary.Models.GameCoordinator;
using DataAccessLibrary.Models.Discord;


namespace DataAccessLibrary.UnitTests.Data;

public class SqliteInMemoryFantasyTests : IDisposable
{
    // private readonly ILogger<FantasyRepository> loggerMock = new Mock<ILogger<FantasyRepository>>();
    private readonly DbConnection _connection;
    private readonly DbContextOptions<AghanimsFantasyContext> _contextOptions;


    #region ConstructorAndDispose
    public SqliteInMemoryFantasyTests()
    {
        // Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
        // at the end of the test (see Dispose below).
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        // These options will be used by the context instances in this test suite, including the connection opened above.
        _contextOptions = new DbContextOptionsBuilder<AghanimsFantasyContext>()
            .UseSqlite(_connection)
            .Options;

        // Create the schema and seed some data
        using var context = new AghanimsFantasyContext(_contextOptions);

        if (context.Database.EnsureCreated())
        {
            using var viewCommand = context.Database.GetDbConnection().CreateCommand();
            viewCommand.CommandText = @"
            create view fantasy_player_points as
            select
                dfl.id as fantasy_league_id,
                dfp.id as fantasy_player_id,
                fmp.id as fantasy_match_player_id,
                fmp.kills as kills,
                fmp.kills * dflw.kills_weight as kills_points,
                fmp.deaths as deaths,
                fmp.deaths * dflw.deaths_weight as deaths_points,
                fmp.assists as assists,
                fmp.assists * dflw.assists_weight as assists_points,
                fmp.last_hits as last_hits,
                fmp.last_hits * dflw.last_hits_weight as last_hits_points,
                fmp.gold_per_min as gold_per_min,
                fmp.gold_per_min * dflw.gold_per_min_weight as gold_per_min_points,
                fmp.xp_per_min as xp_per_min,
                fmp.xp_per_min * dflw.xp_per_min_weight as xp_per_min_points,
                fmp.net_worth as networth,
                fmp.net_worth * dflw.networth_weight as networth_points,
                fmp.hero_damage as hero_damage,
                fmp.hero_damage * dflw.hero_damage_weight as hero_damage_points,
                fmp.tower_damage as tower_damage,
                fmp.tower_damage * dflw.tower_damage_weight as tower_damage_points,
                fmp.hero_healing as hero_healing,
                fmp.hero_healing * dflw.hero_healing_weight as hero_healing_points,
                fmp.gold as gold,
                fmp.gold * dflw.gold_weight as gold_points,
                fmp.fight_score as fight_score,
                fmp.fight_score * dflw.fight_score_weight as fight_score_points,
                fmp.farm_score as farm_score,
                fmp.farm_score * dflw.farm_score_weight as farm_score_points,
                fmp.support_score as support_score,
                fmp.support_score * dflw.support_score_weight as support_score_points,
                fmp.push_score as push_score,
                fmp.push_score * dflw.push_score_weight as push_score_points,
                fmp.hero_xp as hero_xp,
                fmp.hero_xp * dflw.hero_xp_weight as hero_xp_points,
                fmp.camps_stacked as camps_stacked,
                fmp.camps_stacked * dflw.camps_stacked_weight as camps_stacked_points,
                fmp.rampages as rampages,
                fmp.rampages * dflw.rampages_weight as rampages_points,
                fmp.triple_kills as triple_kills,
                fmp.triple_kills * dflw.triple_kills_weight as triple_kills_points,
                fmp.aegis_snatched as aegis_snatched,
                fmp.aegis_snatched * dflw.aegis_snatched_weight as aegis_snatched_points,	
                fmp.rapiers_purchased as rapiers_purchased,
                fmp.rapiers_purchased * dflw.rapiers_purchased_weight as rapiers_purchased_points,
                fmp.couriers_killed as couriers_killed,
                fmp.couriers_killed * dflw.couriers_killed_weight as couriers_killed_points,
                fmp.support_gold_spent as support_gold_spent,
                fmp.support_gold_spent * dflw.support_gold_spent_weight as support_gold_spent_points,
                fmp.observer_wards_placed as observer_wards_placed,
                fmp.observer_wards_placed * dflw.observer_wards_placed_weight as observer_wards_placed_points,
                fmp.sentry_wards_placed as sentry_wards_placed,
                fmp.sentry_wards_placed * dflw.sentry_wards_placed_weight as sentry_wards_placed_points,
                fmp.dewards as wards_dewarded,
                fmp.dewards * dflw.wards_dewarded_weight as wards_dewarded_points,
                fmp.stun_duration as stun_duration,
                fmp.stun_duration * dflw.stun_duration_weight as stun_duration_points,	
                cast(
                    coalesce(fmp.kills * dflw.kills_weight, 0) +
                    coalesce(fmp.deaths * dflw.deaths_weight, 0) + 
                    coalesce(fmp.assists * dflw.assists_weight, 0) + 
                    coalesce(fmp.last_hits * dflw.last_hits_weight, 0) +
                    coalesce(fmp.gold_per_min * dflw.gold_per_min_weight, 0) + 
                    coalesce(fmp.xp_per_min * dflw.xp_per_min_weight, 0) + 
                    coalesce(fmp.net_worth * dflw.networth_weight, 0) +
                    coalesce(fmp.hero_damage * dflw.hero_damage_weight, 0) +
                    coalesce(fmp.tower_damage * dflw.tower_damage_weight, 0) +
                    coalesce(fmp.hero_healing * dflw.hero_healing_weight, 0) +
                    coalesce(fmp.gold * dflw.gold_weight, 0) +
                    coalesce(fmp.fight_score * dflw.fight_score_weight, 0) +
                    coalesce(fmp.farm_score * dflw.farm_score_weight, 0) + 
                    coalesce(fmp.support_score * dflw.support_score_weight, 0) +
                    coalesce(fmp.push_score * dflw.push_score_weight, 0) +
                    coalesce(fmp.hero_xp * dflw.hero_xp_weight, 0) +
                    coalesce(fmp.camps_stacked * dflw.camps_stacked_weight, 0) +
                    coalesce(fmp.rampages * dflw.rampages_weight, 0) +
                    coalesce(fmp.triple_kills * dflw.triple_kills_weight, 0) +
                    coalesce(fmp.aegis_snatched * dflw.aegis_snatched_weight, 0) +	
                    coalesce(fmp.rapiers_purchased * dflw.rapiers_purchased_weight, 0) +
                    coalesce(fmp.couriers_killed * dflw.couriers_killed_weight, 0) +
                    coalesce(fmp.support_gold_spent * dflw.support_gold_spent_weight, 0) +
                    coalesce(fmp.observer_wards_placed * dflw.observer_wards_placed_weight, 0) +
                    coalesce(fmp.sentry_wards_placed * dflw.sentry_wards_placed_weight, 0) +
                    coalesce(fmp.dewards * dflw.wards_dewarded_weight, 0) +
                    coalesce(fmp.stun_duration * dflw.stun_duration_weight, 0)	
                as double) as total_match_fantasy_points	
            from dota_fantasy_leagues dfl
                join dota_fantasy_league_weights dflw 
                    on dfl.id = dflw.fantasy_league_id
                join dota_fantasy_players dfp
                    on dfl.id = dfp.fantasy_league_id
                left join fantasy_match fm
                    on dfl.league_id = fm.league_id
                        and dfl.league_start_time <= fm.start_time 
                        and dfl.league_end_time >= fm.start_time
                left join fantasy_match_player fmp 
                    on fmp.match_id = fm.match_id and fmp.account_id = dfp.dota_account_id
            ;

            create view fantasy_player_point_totals as
            SELECT 
                fantasy_league_id, 
                fantasy_player_id, 
                count(distinct fantasy_match_player_id) as matches,
                coalesce(sum(kills),0) as kills,
                coalesce(sum(kills_points),0) as kills_points,
                coalesce(sum(deaths),0) as deaths,
                coalesce(sum(deaths_points),0) as deaths_points,
                coalesce(sum(assists),0) as assists,
                coalesce(sum(assists_points),0) as assists_points,
                coalesce(sum(last_hits),0) as last_hits,
                coalesce(sum(last_hits_points),0) as last_hits_points,
                coalesce(avg(gold_per_min),0) as gold_per_min,
                coalesce(sum(gold_per_min_points),0) as gold_per_min_points,
                coalesce(avg(xp_per_min),0) as xp_per_min,
                coalesce(sum(xp_per_min_points),0) as xp_per_min_points,
                coalesce(sum(networth),0) as networth,
                coalesce(sum(networth_points),0) as networth_points,
                coalesce(sum(hero_damage),0) as hero_damage,
                coalesce(sum(hero_damage_points),0) as hero_damage_points,
                coalesce(sum(tower_damage),0) as tower_damage,
                coalesce(sum(tower_damage_points),0) as tower_damage_points,
                coalesce(sum(hero_healing),0) as hero_healing,
                coalesce(sum(hero_healing_points),0) as hero_healing_points,
                coalesce(sum(gold),0) as gold,
                coalesce(sum(gold_points),0) as gold_points,
                coalesce(sum(fight_score),0) as fight_score,
                coalesce(sum(fight_score_points),0) as fight_score_points,
                coalesce(sum(farm_score),0) as farm_score,
                coalesce(sum(farm_score_points),0) as farm_score_points,
                coalesce(sum(support_score),0) as support_score,
                coalesce(sum(support_score_points),0) as support_score_points,
                coalesce(sum(push_score),0) as push_score,
                coalesce(sum(push_score_points),0) as push_score_points,
                coalesce(sum(hero_xp),0) as hero_xp,
                coalesce(sum(hero_xp_points),0) as hero_xp_points,
                coalesce(sum(camps_stacked),0) as camps_stacked,
                coalesce(sum(camps_stacked_points),0) as camps_stacked_points,
                coalesce(sum(rampages),0) as rampages,
                coalesce(sum(rampages_points),0) as rampages_points,
                coalesce(sum(triple_kills),0) as triple_kills,
                coalesce(sum(triple_kills_points),0) as triple_kills_points,
                coalesce(sum(aegis_snatched),0) as aegis_snatched,
                coalesce(sum(aegis_snatched_points),0) as aegis_snatched_points,
                coalesce(sum(rapiers_purchased),0) as rapiers_purchased,
                coalesce(sum(rapiers_purchased_points),0) as rapiers_purchased_points,
                coalesce(sum(couriers_killed),0) as couriers_killed,
                coalesce(sum(couriers_killed_points),0) as couriers_killed_points,
                coalesce(sum(support_gold_spent),0) as support_gold_spent,
                coalesce(sum(support_gold_spent_points),0) as support_gold_spent_points,
                coalesce(sum(observer_wards_placed),0) as observer_wards_placed,
                coalesce(sum(observer_wards_placed_points),0) as observer_wards_placed_points,
                coalesce(sum(sentry_wards_placed),0) as sentry_wards_placed,
                coalesce(sum(sentry_wards_placed_points),0) as sentry_wards_placed_points,
                coalesce(sum(wards_dewarded),0) as wards_dewarded,
                coalesce(sum(wards_dewarded_points),0) as wards_dewarded_points,
                coalesce(sum(stun_duration),0) as stun_duration,
                coalesce(sum(stun_duration_points),0) as stun_duration_points,
                coalesce(sum(total_match_fantasy_points),0) as total_match_fantasy_points
            FROM fantasy_player_points
            group by fantasy_league_id, fantasy_player_id
            order by total_match_fantasy_points desc
            ;
            ";
            viewCommand.ExecuteNonQuery();
        }

        Team team = new Team
        {
            Id = 1
        };

        Account Player1 = new Account
        {
            Id = 1
        };

        Account Player2 = new Account
        {
            Id = 2
        };

        Account Player3 = new Account
        {
            Id = 3
        };

        context.Teams.Add(team);

        context.SaveChanges();

        League league100 = new League
        {
            Id = 100,
            Name = "test league 1",
            IsActive = true,
            MatchMetadatas = [
                       new GcMatchMetadata {
                        Id = 20,
                        MatchId = 2,
                        Teams = [
                            new GcMatchMetadataTeam {
                                Players = [
                                    new GcMatchMetadataPlayer {
                                        Id = 200
                                    }
                                ]
                            }
                        ]
                    },
                    new GcMatchMetadata {
                        Id = 10,
                        MatchId = 1,
                        Teams = [
                            new GcMatchMetadataTeam {
                                Players = [
                                    new GcMatchMetadataPlayer {
                                        Id = 100
                                    }
                                ]
                            }
                        ]
                    }
            ]
        };

        FantasyLeague fantasyLeague1 = new FantasyLeague
        {
            Id = 1,
            League = league100,
            Name = "test league 100",
            IsActive = true,
            FantasyDraftLocked = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
            LeagueStartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
            LeagueEndTime = new DateTimeOffset(new DateTime(2024, 12, 31)).ToUnixTimeSeconds(),
        };

        FantasyPlayer fantasyPlayer1 = new FantasyPlayer
        {
            Id = 1,
            FantasyLeague = fantasyLeague1,
            DotaAccount = Player1,
            Team = new Team
            {
                Id = 10
            },
        };

        FantasyPlayer fantasyPlayer2 = new FantasyPlayer
        {
            Id = 2,
            FantasyLeague = fantasyLeague1,
            DotaAccount = Player2,
            Team = new Team
            {
                Id = 11
            }
        };

        FantasyPlayer fantasyPlayer3 = new FantasyPlayer
        {
            Id = 3,
            FantasyLeague = fantasyLeague1,
            DotaAccount = Player3,
            Team = new Team
            {
                Id = 12
            }
        };

        fantasyLeague1.FantasyPlayers.Add(fantasyPlayer1);
        fantasyLeague1.FantasyPlayers.Add(fantasyPlayer2);
        fantasyLeague1.FantasyPlayers.Add(fantasyPlayer3);

        fantasyLeague1.FantasyLeagueWeight = new FantasyLeagueWeight
        {
            FantasyLeague = fantasyLeague1,
            KillsWeight = 0.03M,
            DeathsWeight = -0.03M,
            AssistsWeight = 0.15M,
            LastHitsWeight = 0.003M,
            GoldPerMinWeight = 0.002M,
            XpPerMinWeight = 0.002M
        };

        league100.FantasyLeagues.Add(
            fantasyLeague1
        );

        MatchHistory matchHistory1 = new MatchHistory
        {
            MatchId = 1,
            League = league100,
            DireTeamId = 2,
            RadiantTeamId = 3,
            LobbyType = 0,
            MatchSeqNum = 0,
            StartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
            SeriesId = 0,
            SeriesType = 0,
            Players = new List<MatchHistoryPlayer>
            {

            }
        };

        matchHistory1.Players.Add(
            new MatchHistoryPlayer
            {
                Match = matchHistory1,
                AccountId = 1,
                PlayerSlot = 0,
                HeroId = 1,
                TeamNumber = 0,
                TeamSlot = 0,
                Id = 1
            }
        );

        MatchHistory matchHistory2 = new MatchHistory
        {
            MatchId = 2,
            League = league100,
            DireTeamId = 2,
            RadiantTeamId = 3,
            LobbyType = 0,
            MatchSeqNum = 0,
            StartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
            SeriesId = 0,
            SeriesType = 0,
            Players = new List<MatchHistoryPlayer>
            {

            }
        };

        matchHistory2.Players.Add(
            new MatchHistoryPlayer
            {
                Match = matchHistory2,
                AccountId = 2,
                PlayerSlot = 0,
                HeroId = 1,
                TeamNumber = 0,
                TeamSlot = 0,
                Id = 2
            }
        );

        league100.MatchHistories.AddRange([
            matchHistory1
        ]);

        MatchDetail matchDetail1 = new MatchDetail
        {
            MatchId = 1,
            BarracksStatusDire = 0,
            BarracksStatusRadiant = 0,
            Cluster = 0,
            DireScore = 0,
            Duration = 0,
            Engine = 0,
            FirstBloodTime = 0,
            Flags = 0,
            GameMode = 0,
            HumanPlayers = 10,
            LeagueId = 100,
            LobbyType = 0,
            MatchSeqNum = 0,
            PicksBans = new List<MatchDetailsPicksBans>(),
            Players = [],
            PreGameDuration = 0,
            RadiantScore = 0,
            RadiantWin = true,
            StartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
            TowerStatusDire = 0,
            TowerStatusRadiant = 0
        };

        matchDetail1.Players.AddRange([
            new MatchDetailsPlayer
                {
                    Id = 1,
                    Match = matchDetail1,
                    AccountId = 1,
                    AbilityUpgrades = new List<MatchDetailsPlayersAbilityUpgrade>(),
                    AghanimsScepter = 0,
                    AghanimsShard = 0,
                    Assists = 0,
                    Backpack0 = 0,
                    Backpack1 = 0,
                    Backpack2 = 0,
                    Deaths = 0,
                    Denies = 0,
                    Gold = 0,
                    GoldPerMin = 0,
                    GoldSpent = 0,
                    HeroDamage = 0,
                    HeroHealing = 0,
                    HeroId = 0,
                    Item0 = 0,
                    Item1 = 0,
                    Item2 = 0,
                    Item3 = 0,
                    Item4 = 0,
                    Item5 = 0,
                    ItemNeutral = 0,
                    Kills = 0,
                    LastHits = 0,
                    LeaverStatus = 0,
                    Level = 0,
                    Moonshard = 0,
                    Networth = 0,
                    PlayerSlot = 0,
                    ScaledHeroDamage = 0,
                    ScaledHeroHealing = 0,
                    ScaledTowerDamage = 0,
                    TeamNumber = 0,
                    TeamSlot = 0,
                    TowerDamage = 0,
                    XpPerMin = 0
                },
                new MatchDetailsPlayer
                {
                    Id = 2,
                    Match = matchDetail1,
                    AccountId = 2,
                    AbilityUpgrades = new List<MatchDetailsPlayersAbilityUpgrade>(),
                    AghanimsScepter = 0,
                    AghanimsShard = 0,
                    Assists = 0,
                    Backpack0 = 0,
                    Backpack1 = 0,
                    Backpack2 = 0,
                    Deaths = 0,
                    Denies = 0,
                    Gold = 0,
                    GoldPerMin = 0,
                    GoldSpent = 0,
                    HeroDamage = 0,
                    HeroHealing = 0,
                    HeroId = 0,
                    Item0 = 0,
                    Item1 = 0,
                    Item2 = 0,
                    Item3 = 0,
                    Item4 = 0,
                    Item5 = 0,
                    ItemNeutral = 0,
                    Kills = 0,
                    LastHits = 0,
                    LeaverStatus = 0,
                    Level = 0,
                    Moonshard = 0,
                    Networth = 0,
                    PlayerSlot = 0,
                    ScaledHeroDamage = 0,
                    ScaledHeroHealing = 0,
                    ScaledTowerDamage = 0,
                    TeamNumber = 0,
                    TeamSlot = 0,
                    TowerDamage = 0,
                    XpPerMin = 0
                }
        ]);

        MatchDetail matchDetail2 = new MatchDetail
        {
            MatchId = 2,
            BarracksStatusDire = 0,
            BarracksStatusRadiant = 0,
            Cluster = 0,
            DireScore = 0,
            Duration = 0,
            Engine = 0,
            FirstBloodTime = 0,
            Flags = 0,
            GameMode = 0,
            HumanPlayers = 10,
            LeagueId = 100,
            LobbyType = 0,
            MatchSeqNum = 0,
            PicksBans = new List<MatchDetailsPicksBans>(),
            Players = new List<MatchDetailsPlayer>(),
            PreGameDuration = 0,
            RadiantScore = 0,
            RadiantWin = true,
            StartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
            TowerStatusDire = 0,
            TowerStatusRadiant = 0,
        };

        MatchDetail matchDetail10 = new MatchDetail
        {
            MatchId = 10,
            BarracksStatusDire = 0,
            BarracksStatusRadiant = 0,
            Cluster = 0,
            DireScore = 0,
            Duration = 0,
            Engine = 0,
            FirstBloodTime = 0,
            Flags = 0,
            GameMode = 0,
            HumanPlayers = 10,
            LeagueId = 100,
            LobbyType = 0,
            MatchSeqNum = 0,
            PicksBans = new List<MatchDetailsPicksBans>(),
            Players = new List<MatchDetailsPlayer>(),
            PreGameDuration = 0,
            RadiantScore = 0,
            RadiantWin = true,
            StartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
            TowerStatusDire = 0,
            TowerStatusRadiant = 0
        };

        league100.MatchDetails.AddRange([
            matchDetail1,
            matchDetail2,
            matchDetail10
        ]);


        context.FantasyLeagues.Add(fantasyLeague1);

        League league200 = new League
        {
            Id = 200,
            Name = "test league 2",
            IsActive = false,
        };

        FantasyLeague fantasyLeague2 = new FantasyLeague
        {
            Id = 2,
            League = league200,
            Name = "test league 2",
            IsActive = false,
            FantasyDraftLocked = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
            LeagueStartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
            LeagueEndTime = new DateTimeOffset(new DateTime(2024, 12, 31)).ToUnixTimeSeconds(),
        };

        fantasyLeague2.FantasyPlayers.Add(
            new FantasyPlayer
            {
                Id = 4,
                FantasyLeague = fantasyLeague2,
                DotaAccount = new Account
                {
                    Id = 4,
                },
                Team = new Team
                {
                    Id = 13
                }
            }
        );

        league200.FantasyLeagues.Add(fantasyLeague2);

        league200.MatchHistories.AddRange([
            new MatchHistory
            {
                MatchId = 3,
                League = league200,
                DireTeamId = 2,
                RadiantTeamId = 3,
                LobbyType = 0,
                MatchSeqNum = 0,
                StartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
                SeriesId = 0,
                SeriesType = 0,
                Players = new List<MatchHistoryPlayer>()
            },
            new MatchHistory
            {
                MatchId = 4,
                League = league200,
                DireTeamId = 2,
                RadiantTeamId = 3,
                LobbyType = 0,
                MatchSeqNum = 0,
                StartTime = new DateTimeOffset(new DateTime(2023, 1, 1)).ToUnixTimeSeconds(),
                SeriesId = 0,
                SeriesType = 0,
                Players = new List<MatchHistoryPlayer>()
            }
        ]);

        league200.MatchDetails.AddRange([
            new MatchDetail
            {
                MatchId = 3,
                BarracksStatusDire = 0,
                BarracksStatusRadiant = 0,
                Cluster = 0,
                DireScore = 0,
                Duration = 0,
                Engine = 0,
                FirstBloodTime = 0,
                Flags = 0,
                GameMode = 0,
                HumanPlayers = 10,
                LeagueId = 2,
                LobbyType = 0,
                MatchSeqNum = 0,
                PicksBans = new List<MatchDetailsPicksBans>(),
                Players = new List<MatchDetailsPlayer>(),
                PreGameDuration = 0,
                RadiantScore = 0,
                RadiantWin = true,
                StartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
                TowerStatusDire = 0,
                TowerStatusRadiant = 0
            },
            new MatchDetail
            {
                MatchId = 4,
                BarracksStatusDire = 0,
                BarracksStatusRadiant = 0,
                Cluster = 0,
                DireScore = 0,
                Duration = 0,
                Engine = 0,
                FirstBloodTime = 0,
                Flags = 0,
                GameMode = 0,
                HumanPlayers = 10,
                LeagueId = 2,
                LobbyType = 0,
                MatchSeqNum = 0,
                PicksBans = new List<MatchDetailsPicksBans>(),
                Players = new List<MatchDetailsPlayer>(),
                PreGameDuration = 0,
                RadiantScore = 0,
                RadiantWin = true,
                StartTime = new DateTimeOffset(new DateTime(2023, 1, 1)).ToUnixTimeSeconds(),
                TowerStatusDire = 0,
                TowerStatusRadiant = 0
            }
        ]);
        context.Leagues.AddRange(
               league100,
               league200
        );

        league200.FantasyLeagues.Add(new FantasyLeague
        {
            Id = 3,
            League = league200,
            Name = "test league 3",
            IsActive = false,
            FantasyDraftLocked = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
            LeagueStartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
            LeagueEndTime = new DateTimeOffset(new DateTime(2024, 12, 31)).ToUnixTimeSeconds(),
        });

        context.SaveChanges();

        // Fantasy draft with existing data

        var discordUser = new DiscordUser
        {
            Id = 1,
            Username = "test"
        };

        context.DiscordUsers.Add(discordUser);

        var fantasyDraft = new FantasyDraft
        {
            Id = 1,
            DiscordAccountId = 1,
            DraftCreated = DateTime.UtcNow,
            DraftLastUpdated = DateTime.UtcNow,
            FantasyLeague = fantasyLeague1,
            DraftPickPlayers = new List<FantasyDraftPlayer>()
        };

        fantasyDraft.DraftPickPlayers.Add(
            new FantasyDraftPlayer() { FantasyPlayer = context.FantasyPlayers.First(fp => fp.Id == 1) ?? new FantasyPlayer() { FantasyLeague = fantasyLeague1, Team = new Team(), DotaAccount = new Account() }, DraftOrder = 1 }
        );

        context.FantasyDrafts.AddRange(
            fantasyDraft
        );

        context.SaveChanges();

        FantasyMatch fantasyMatch1 = new FantasyMatch
        {
            League = league100,
            MatchId = 1,
            StartTime = new DateTimeOffset(new DateTime(2024, 1, 2)).ToUnixTimeSeconds()
        };

        fantasyMatch1.Players.Add(
            new FantasyMatchPlayer
            {
                Id = 1,
                Match = fantasyMatch1,
                Account = Player1,
                GcMetadataPlayerParsed = true,
                MatchDetailPlayerParsed = true
            }
        );

        fantasyMatch1.Players.Add(
            new FantasyMatchPlayer
            {
                Id = 2,
                Match = fantasyMatch1,
                Account = Player2,
                GcMetadataPlayerParsed = true,
                MatchDetailPlayerParsed = true
            }
        );

        // Fantasy matches for scoring
        context.FantasyMatches.AddRange([
            fantasyMatch1
        ]);

        context.SaveChanges();
    }

    AghanimsFantasyContext CreateContext() => new AghanimsFantasyContext(_contextOptions);

    public void Dispose() => _connection.Dispose();
    #endregion

    #region Fantasy
    [Fact]
    public async void GetLeagueFantasyPlayers()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<FantasyPlayerRepository>>();
        var repository = new FantasyPlayerRepository(loggerMock.Object, context);

        var fantasyLeague = await context.FantasyLeagues.FindAsync(1);
        var fantasyPlayers = await repository.GetFantasyLeaguePlayersAsync(fantasyLeague!);
        Assert.Equal(3, fantasyPlayers.Count());
        Assert.IsAssignableFrom<IEnumerable<FantasyPlayer>>(fantasyPlayers);
    }

    [Fact]
    public async void GetAllLeagueFantasyPlayers()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<FantasyPlayerRepository>>();
        var repository = new FantasyPlayerRepository(loggerMock.Object, context);

        var fantasyPlayers = await repository.GetAllAsync();
        Assert.Equal(4, fantasyPlayers.Count());
        Assert.IsAssignableFrom<IEnumerable<FantasyPlayer>>(fantasyPlayers);
    }

    [Fact]
    public async void GetEmptyLeagueFantasyPlayers()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<FantasyPlayerRepository>>();
        var repository = new FantasyPlayerRepository(loggerMock.Object, context);

        var fantasyLeague = await context.FantasyLeagues.FindAsync(3);
        var fantasyPlayers = await repository.GetFantasyLeaguePlayersAsync(fantasyLeague!);
        Assert.Empty(fantasyPlayers);
    }

    [Fact]
    public async void GetFantasyPlayerPointsLeague()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<FantasyDraftRepository>>();
        var repository = new FantasyDraftRepository(loggerMock.Object, context);

        var fantasyLeague = await context.FantasyLeagues.FindAsync(1);
        var fantasyPlayerPoints = await repository.FantasyPlayerPointTotalsByFantasyLeagueAsync(fantasyLeague!);
        // // We want to return fantasy player points even if rows are null, so this should have 2 players with a match
        // //  and all 3 players should return a null row for scenarios when a new league has no games yet
        Assert.Equal(2, fantasyPlayerPoints.Where(fpp => fpp.TotalMatches > 0).Count());
        Assert.Single(fantasyPlayerPoints.Where(fpp => fpp.TotalMatches == 0));
        Assert.Equal(3, fantasyPlayerPoints.Count());
        Assert.IsAssignableFrom<IEnumerable<FantasyPlayerPointTotals>>(fantasyPlayerPoints);
    }

    [Fact]
    public async void GetFantasyPlayerPointTotalsLeague()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<FantasyDraftRepository>>();
        var repository = new FantasyDraftRepository(loggerMock.Object, context);

        var fantasyLeague = await context.FantasyLeagues.FindAsync(1);
        var fantasyPlayerPoints = await repository.FantasyPlayerPointTotalsByFantasyLeagueAsync(fantasyLeague!);
        // We want to return fantasy player points even if rows are null, so this should have all 3 players
        // even though 1 has no match
        Assert.Equal(3, fantasyPlayerPoints.Count());
        Assert.IsAssignableFrom<IEnumerable<FantasyPlayerPointTotals>>(fantasyPlayerPoints);
    }

    [Fact]
    public async void GetFantasyDraftsByUserLeague()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<FantasyDraftRepository>>();
        var repository = new FantasyDraftRepository(loggerMock.Object, context);

        var fantasyLeague = await context.FantasyLeagues.FindAsync(1);
        var discordUser = await context.DiscordUsers.FindAsync(1L);
        var fantasyDraft = await repository.GetByUserFantasyLeague(fantasyLeague!, discordUser!);

        Assert.NotNull(fantasyDraft);
        Assert.IsAssignableFrom<FantasyDraft>(fantasyDraft);
    }

    [Fact]
    public async void GetFantasyDraftsEmptyLeague()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<FantasyDraftRepository>>();
        var repository = new FantasyDraftRepository(loggerMock.Object, context);

        var fantasyLeague = await context.FantasyLeagues.FindAsync(2);
        var discordUser = await context.DiscordUsers.FindAsync(1L);
        var fantasyDraft = await repository.GetByUserFantasyLeague(fantasyLeague!, discordUser!);

        Assert.Null(fantasyDraft);
    }

    // Task<IEnumerable<FantasyPlayerPoints>> FantasyDraftPointsByLeagueAsync(int LeagueId);
    // Task<IEnumerable<FantasyPlayerPoints>> FantasyDraftPointsByUserLeagueAsync(long UserDiscordAccountId, int LeagueId);
    // IEnumerable<FantasyPlayerPointTotals> AggregateFantasyPlayerPoints(IEnumerable<FantasyPlayerPoints> fantasyPlayerPoints);
    // IEnumerable<FantasyDraftPointTotals> AggregateFantasyDraftPoints(IEnumerable<FantasyPlayerPoints> fantasyPlayerPoints);
    // Task ClearUserFantasyPlayersAsync(long UserDiscordAccountId, int LeagueId);
    // Task<FantasyDraft> AddNewUserFantasyPlayerAsync(long UserDiscordAccountId, int LeagueId, long? FantasyPlayerId, int DraftOrder);

    #endregion
}