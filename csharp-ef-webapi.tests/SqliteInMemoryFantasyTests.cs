using Moq;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using csharp_ef_webapi.Models.ProMetadata;
using csharp_ef_webapi.Models.Fantasy;
using csharp_ef_webapi.Models.WebApi;
using csharp_ef_webapi.Models.GameCoordinator;


namespace csharp_ef_webapi.UnitTests.Data;

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
                dmdp.id as match_details_player_id,
                dmdp.kills as kills,
                dmdp.kills * dflw.kills_weight as kills_points,
                dmdp.deaths as deaths,
                dmdp.deaths * dflw.deaths_weight as deaths_points,
                dmdp.assists as assists,
                dmdp.assists * dflw.assists_weight as assists_points,
                dmdp.last_hits as last_hits,
                dmdp.last_hits * dflw.last_hits_weight as last_hits_points,
                dmdp.gold_per_min as gold_per_min,
                dmdp.gold_per_min * dflw.gold_per_min_weight as gold_per_min_points,
                dmdp.xp_per_min as xp_per_min,
                dmdp.xp_per_min * dflw.xp_per_min_weight as xp_per_min_points,
                dmdp.net_worth as networth,
                dmdp.net_worth * dflw.networth_weight as networth_points,
                dmdp.hero_damage as hero_damage,
                dmdp.hero_damage * dflw.hero_damage_weight as hero_damage_points,
                dmdp.tower_damage as tower_damage,
                dmdp.tower_damage * dflw.tower_damage_weight as tower_damage_points,
                dmdp.hero_healing as hero_healing,
                dmdp.hero_healing * dflw.hero_healing_weight as hero_healing_points,
                dmdp.gold as gold,
                dmdp.gold * dflw.gold_weight as gold_points,
                dgmmp.fight_score as fight_score,
                dgmmp.fight_score * dflw.fight_score_weight as fight_score_points,
                dgmmp.farm_score as farm_score,
                dgmmp.farm_score * dflw.farm_score_weight as farm_score_points,
                dgmmp.support_score as support_score,
                dgmmp.support_score * dflw.support_score_weight as support_score_points,
                dgmmp.push_score as push_score,
                dgmmp.push_score * dflw.push_score_weight as push_score_points,
                dgmmp.hero_xp as hero_xp,
                dgmmp.hero_xp * dflw.hero_xp_weight as hero_xp_points,
                dgmmp.camps_stacked as camps_stacked,
                dgmmp.camps_stacked * dflw.camps_stacked_weight as camps_stacked_points,
                dgmmp.rampages as rampages,
                dgmmp.rampages * dflw.rampages_weight as rampages_points,
                dgmmp.triple_kills as triple_kills,
                dgmmp.triple_kills * dflw.triple_kills_weight as triple_kills_points,
                dgmmp.aegis_snatched as aegis_snatched,
                dgmmp.aegis_snatched * dflw.aegis_snatched_weight as aegis_snatched_points,	
                dgmmp.rapiers_purchased as rapiers_purchased,
                dgmmp.rapiers_purchased * dflw.rapiers_purchased_weight as rapiers_purchased_points,
                dgmmp.couriers_killed as couriers_killed,
                dgmmp.couriers_killed * dflw.couriers_killed_weight as couriers_killed_points,
                dgmmp.support_gold_spent as support_gold_spent,
                dgmmp.support_gold_spent * dflw.support_gold_spent_weight as support_gold_spent_points,
                dgmmp.observer_wards_placed as observer_wards_placed,
                dgmmp.observer_wards_placed * dflw.observer_wards_placed_weight as observer_wards_placed_points,
                dgmmp.sentry_wards_placed as sentry_wards_placed,
                dgmmp.sentry_wards_placed * dflw.sentry_wards_placed_weight as sentry_wards_placed_points,
                dgmmp.wards_dewarded as wards_dewarded,
                dgmmp.wards_dewarded * dflw.wards_dewarded_weight as wards_dewarded_points,
                dgmmp.stun_duration as stun_duration,
                dgmmp.stun_duration * dflw.stun_duration_weight as stun_duration_points,	
                cast(
                    dmdp.kills * dflw.kills_weight +
                    dmdp.deaths * dflw.deaths_weight + 
                    dmdp.assists * dflw.assists_weight + 
                    dmdp.last_hits * dflw.last_hits_weight +
                    dmdp.gold_per_min * dflw.gold_per_min_weight + 
                    dmdp.xp_per_min * dflw.xp_per_min_weight + 
                    dmdp.net_worth * dflw.networth_weight +
                    dmdp.hero_damage * dflw.hero_damage_weight +
                    dmdp.tower_damage * dflw.tower_damage_weight +
                    dmdp.hero_healing * dflw.hero_healing_weight +
                    dmdp.gold * dflw.gold_weight +
                    dgmmp.fight_score * dflw.fight_score_weight +
                    dgmmp.farm_score * dflw.farm_score_weight + 
                    dgmmp.support_score * dflw.support_score_weight +
                    dgmmp.push_score * dflw.push_score_weight +
                    dgmmp.hero_xp * dflw.hero_xp_weight +
                    dgmmp.camps_stacked * dflw.camps_stacked_weight +
                    dgmmp.rampages * dflw.rampages_weight +
                    dgmmp.triple_kills * dflw.triple_kills_weight +
                    dgmmp.aegis_snatched * dflw.aegis_snatched_weight +	
                    dgmmp.rapiers_purchased * dflw.rapiers_purchased_weight +
                    dgmmp.couriers_killed * dflw.couriers_killed_weight +
                    dgmmp.support_gold_spent * dflw.support_gold_spent_weight +
                    dgmmp.observer_wards_placed * dflw.observer_wards_placed_weight +
                    dgmmp.sentry_wards_placed * dflw.sentry_wards_placed_weight +
                    dgmmp.wards_dewarded * dflw.wards_dewarded_weight +
                    dgmmp.stun_duration * dflw.stun_duration_weight	
                as double) as total_match_fantasy_points	
            from dota_fantasy_leagues dfl
                join dota_fantasy_league_weights dflw 
                    on dfl.id = dflw.fantasy_league_id 
                join dota_fantasy_players dfp
                    on dfl.id = dfp.fantasy_league_id
                left join dota_match_details dmd 
                    on dfl.league_id = dmd.league_id
                        and dfl.league_start_time <= dmd.start_time 
                        and dfl.league_end_time >= dmd.start_time
                left join dota_match_details_players dmdp 
                    on dmd.match_id = dmdp.match_id and dmdp.account_id = dfp.dota_account_id
                left join dota_gc_match_metadata dgmm 
                    on dmd.match_id = dgmm.match_id
                left join dota_gc_match_metadata_team dgmmt 
                    on dgmmt.""GcMatchMetadataId"" = dgmm.id
                left join dota_gc_match_metadata_player dgmmp 
                    on dgmmp.""GcMatchMetadataTeamId"" = dgmmt.id 
                        and dgmmp.player_slot = dmdp.player_slot
            where dgmmt.id is null or 
                (dgmmt.id is not null and dgmmp.id is not null)
            ;
            
            create view fantasy_player_point_totals as
            SELECT 
                fantasy_league_id, 
                fantasy_player_id, 
                count(distinct match_details_player_id) as matches,
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

        context.Teams.Add(team);

        context.SaveChanges();

        context.Leagues.AddRange(
            new League
            {
                Id = 100,
                Name = "test league 1",
                IsActive = true,
                FantasyLeagues = new List<FantasyLeague>
                {
                    new FantasyLeague
                    {
                        Id = 1,
                        LeagueId = 100,
                        Name = "test league 100",
                        IsActive = true,
                        FantasyDraftLocked = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
                        LeagueStartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
                        LeagueEndTime = new DateTimeOffset(new DateTime(2024, 12, 31)).ToUnixTimeSeconds(),
                        FantasyPlayers = new List<FantasyPlayer>
                        {
                            new FantasyPlayer
                            {
                                Id = 1,
                                DotaAccount = new Account {
                                    Id = 1,
                                },
                                Team = new Team {
                                    Id = 10
                                },
                            },
                            new FantasyPlayer
                            {
                                Id = 2,
                                DotaAccount = new Account {
                                    Id = 2,
                                },
                                Team = new Team {
                                    Id = 11
                                }
                            },
                            new FantasyPlayer
                            {
                                Id = 3,
                                DotaAccount = new Account {
                                    Id = 3,
                                },
                                Team = new Team {
                                    Id = 12
                                }
                            },
                        },
                        FantasyLeagueWeight = new FantasyLeagueWeight
                        {
                            FantasyLeagueId = 1,
                            KillsWeight = 0.03M,
                            DeathsWeight = -0.03M,
                            AssistsWeight = 0.15M,
                            LastHitsWeight = 0.003M,
                            GoldPerMinWeight = 0.002M,
                            XpPerMinWeight = 0.002M
                        }
                    },
                },
                MatchHistories = [
                    new MatchHistory
                    {
                        MatchId = 1,
                        LeagueId = 100,
                        DireTeamId = 2,
                        RadiantTeamId = 3,
                        LobbyType = 0,
                        MatchSeqNum = 0,
                        StartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
                        SeriesId = 0,
                        SeriesType = 0,
                        Players = new List<MatchHistoryPlayer>{
                            new MatchHistoryPlayer{
                                MatchId = 1,
                                AccountId = 1,
                                PlayerSlot = 0,
                                HeroId = 1,
                                TeamNumber = 0,
                                TeamSlot = 0,
                                Id = 1
                            }
                        }
                    },
                    new MatchHistory
                    {
                        MatchId = 2,
                        LeagueId = 100,
                        DireTeamId = 2,
                        RadiantTeamId = 3,
                        LobbyType = 0,
                        MatchSeqNum = 0,
                        StartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
                        SeriesId = 0,
                        SeriesType = 0,
                        Players = new List<MatchHistoryPlayer>{
                            new MatchHistoryPlayer{
                                MatchId = 2,
                                AccountId = 2,
                                PlayerSlot = 0,
                                HeroId = 1,
                                TeamNumber = 0,
                                TeamSlot = 0,
                                Id = 2
                            }
                        }
                    },
                ],
                MatchDetails = [
                    new MatchDetail
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
                        Players = [
                            new MatchDetailsPlayer
                            {
                                Id = 1,
                                MatchId = 1,
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
                                MatchId = 1,
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
                        ],
                        PreGameDuration = 0,
                        RadiantScore = 0,
                        RadiantWin = true,
                        StartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
                        TowerStatusDire = 0,
                        TowerStatusRadiant = 0
                    },
                    new MatchDetail
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
                    },
                    new MatchDetail
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
                    },
                ],
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
            },
            new League
            {
                Id = 200,
                Name = "test league 2",
                IsActive = false,
                FantasyLeagues = [
                    new FantasyLeague
                    {
                        Id = 2,
                        LeagueId = 200,
                        Name = "test league 2",
                        IsActive = false,
                        FantasyDraftLocked = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                        LeagueStartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
                        LeagueEndTime = new DateTimeOffset(new DateTime(2024, 12, 31)).ToUnixTimeSeconds(),
                        FantasyPlayers = [
                            new FantasyPlayer
                            {
                                Id = 4,
                                DotaAccount = new Account {
                                    Id = 4,
                                },
                                Team = new Team {
                                    Id = 13
                                }
                            }
                        ]
                    }
                ],
                MatchHistories = [
                    new MatchHistory
                    {
                        MatchId = 3,
                        LeagueId = 2,
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
                        LeagueId = 2,
                        DireTeamId = 2,
                        RadiantTeamId = 3,
                        LobbyType = 0,
                        MatchSeqNum = 0,
                        StartTime = new DateTimeOffset(new DateTime(2023, 1, 1)).ToUnixTimeSeconds(),
                        SeriesId = 0,
                        SeriesType = 0,
                        Players = new List<MatchHistoryPlayer>()
                    }
                ],
                MatchDetails = [
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
                ]
            }
        );

        context.SaveChanges();

        // Fantasy draft with existing data

        var fantasyDraft = new FantasyDraft
        {
            Id = 1,
            DiscordAccountId = 1,
            DraftCreated = DateTime.UtcNow,
            DraftLastUpdated = DateTime.UtcNow,
            FantasyLeagueId = 1,
            DraftPickPlayers = new List<FantasyDraftPlayer>()
        };

        fantasyDraft.DraftPickPlayers.Add(
            new FantasyDraftPlayer() { FantasyPlayer = context.FantasyPlayers.First(fp => fp.Id == 1) ?? new FantasyPlayer() { Team = new Team(), DotaAccount = new Account() }, DraftOrder = 1 }
        );

        context.FantasyDrafts.AddRange(
            fantasyDraft
        );

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
        var loggerMock = new Mock<ILogger<FantasyRepository>>();
        var repository = new FantasyRepository(loggerMock.Object, context);

        var fantasyPlayers = await repository.GetFantasyPlayersAsync(1);
        Assert.Equal(3, fantasyPlayers.Count());
        Assert.IsAssignableFrom<IEnumerable<FantasyPlayer>>(fantasyPlayers);
    }

    [Fact]
    public async void GetAllLeagueFantasyPlayers()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<FantasyRepository>>();
        var repository = new FantasyRepository(loggerMock.Object, context);

        var fantasyPlayers = await repository.GetFantasyPlayersAsync(null);
        Assert.Equal(4, fantasyPlayers.Count());
        Assert.IsAssignableFrom<IEnumerable<FantasyPlayer>>(fantasyPlayers);
    }

    [Fact]
    public async void GetEmptyLeagueFantasyPlayers()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<FantasyRepository>>();
        var repository = new FantasyRepository(loggerMock.Object, context);

        var fantasyPlayers = await repository.GetFantasyPlayersAsync(3);
        Assert.Empty(fantasyPlayers);
    }

    [Fact]
    public async void GetFantasyPlayerPointsLeague()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<FantasyRepository>>();
        var repository = new FantasyRepository(loggerMock.Object, context);

        var fantasyPlayerPoints = await repository.FantasyPlayerPointsByFantasyLeagueAsync(1);
        // We want to return fantasy player points even if rows are null, so this should have 2 players with a match
        //  and all 3 players should return a null row for scenarios when a new league has no games yet
        Assert.Equal(2, fantasyPlayerPoints.Where(fpp => fpp.Match != null).Count());
        Assert.Equal(3, fantasyPlayerPoints.Where(fpp => fpp.Match == null).Count());
        Assert.Equal(5, fantasyPlayerPoints.Count());
        Assert.IsAssignableFrom<IEnumerable<FantasyPlayerPoints>>(fantasyPlayerPoints);
    }

    [Fact]
    public async void GetFantasyPlayerPointTotalsLeague()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<FantasyRepository>>();
        var repository = new FantasyRepository(loggerMock.Object, context);

        var fantasyPlayerPoints = await repository.FantasyPlayerPointTotalsByFantasyLeagueAsync(1);
        // We want to return fantasy player points even if rows are null, so this should have all 3 players
        // even though 1 has no match
        Assert.Equal(3, fantasyPlayerPoints.Count());
        Assert.IsAssignableFrom<IEnumerable<FantasyPlayerPointTotals>>(fantasyPlayerPoints);
    }

    [Fact]
    public async void GetFantasyPlayerPointsEmptyLeague()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<FantasyRepository>>();
        var repository = new FantasyRepository(loggerMock.Object, context);

        var fantasyPlayerPoints = await repository.FantasyPlayerPointsByFantasyLeagueAsync(4);

        Assert.Empty(fantasyPlayerPoints);
    }

    [Fact]
    public async void GetFantasyDraftsByUserLeague()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<FantasyRepository>>();
        var repository = new FantasyRepository(loggerMock.Object, context);

        var fantasyPlayerPoints = await repository.FantasyDraftsByUserLeagueAsync(1, 1);

        Assert.Single(fantasyPlayerPoints);
    }

    [Fact]
    public async void GetFantasyDraftsEmptyLeague()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<FantasyRepository>>();
        var repository = new FantasyRepository(loggerMock.Object, context);

        var fantasyPlayerPoints = await repository.FantasyDraftsByUserLeagueAsync(1, 4);

        Assert.Empty(fantasyPlayerPoints);
    }

    [Fact]
    public async void GetFantasyDraftsUnknownUser()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<FantasyRepository>>();
        var repository = new FantasyRepository(loggerMock.Object, context);

        var fantasyPlayerPoints = await repository.FantasyDraftsByUserLeagueAsync(2, 1);

        Assert.Empty(fantasyPlayerPoints);
    }

    // Task<IEnumerable<FantasyPlayerPoints>> FantasyDraftPointsByLeagueAsync(int LeagueId);
    // Task<IEnumerable<FantasyPlayerPoints>> FantasyDraftPointsByUserLeagueAsync(long UserDiscordAccountId, int LeagueId);
    // IEnumerable<FantasyPlayerPointTotals> AggregateFantasyPlayerPoints(IEnumerable<FantasyPlayerPoints> fantasyPlayerPoints);
    // IEnumerable<FantasyDraftPointTotals> AggregateFantasyDraftPoints(IEnumerable<FantasyPlayerPoints> fantasyPlayerPoints);
    // Task ClearUserFantasyPlayersAsync(long UserDiscordAccountId, int LeagueId);
    // Task<FantasyDraft> AddNewUserFantasyPlayerAsync(long UserDiscordAccountId, int LeagueId, long? FantasyPlayerId, int DraftOrder);

    #endregion
}