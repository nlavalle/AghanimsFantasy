using Moq;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


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
            select distinct
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
                (
                    dmdp.kills * dflw.kills_weight +
                    dmdp.deaths * dflw.deaths_weight + 
                    dmdp.assists * dflw.assists_weight + 
                    dmdp.last_hits * dflw.last_hits_weight +
                    dmdp.gold_per_min * dflw.gold_per_min_weight + 
                    dmdp.xp_per_min * dflw.xp_per_min_weight
                ) as total_match_fantasy_points	
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
            ;";
            viewCommand.ExecuteNonQuery();
        }

        Team team = new Team {
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
                        Players = new List<MatchHistoryPlayer>()
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
                        Players = new List<MatchHistoryPlayer>()
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
                        TowerStatusRadiant = 0,
                        MatchMetadata = new GcMatchMetadata {
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
                        MatchMetadata = new GcMatchMetadata {
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
                        }
                    },
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
            new FantasyDraftPlayer() { FantasyPlayer = context.FantasyPlayers.First(fp => fp.Id == 1) ?? new FantasyPlayer(), DraftOrder = 1 }
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

        var fantasyPlayers = await repository.FantasyPlayersByFantasyLeagueAsync(1);
        Assert.Equal(3, fantasyPlayers.Count());
        Assert.IsAssignableFrom<IEnumerable<FantasyPlayer>>(fantasyPlayers);
    }

    [Fact]
    public async void GetAllLeagueFantasyPlayers()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<FantasyRepository>>();
        var repository = new FantasyRepository(loggerMock.Object, context);

        var fantasyPlayers = await repository.FantasyPlayersByFantasyLeagueAsync(null);
        Assert.Equal(4, fantasyPlayers.Count());
        Assert.IsAssignableFrom<IEnumerable<FantasyPlayer>>(fantasyPlayers);
    }

    [Fact]
    public async void GetEmptyLeagueFantasyPlayers()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<FantasyRepository>>();
        var repository = new FantasyRepository(loggerMock.Object, context);

        var fantasyPlayers = await repository.FantasyPlayersByFantasyLeagueAsync(3);
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