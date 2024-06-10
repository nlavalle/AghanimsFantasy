using Moq;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using csharp_ef_webapi.Models.ProMetadata;
using csharp_ef_webapi.Models.Fantasy;
using csharp_ef_webapi.Models.WebApi;


namespace csharp_ef_webapi.UnitTests.Data;

public class SqliteInMemoryMatchTests : IDisposable
{
    // private readonly ILogger<FantasyRepository> loggerMock = new Mock<ILogger<FantasyRepository>>();
    private readonly DbConnection _connection;
    private readonly DbContextOptions<AghanimsFantasyContext> _contextOptions;


    #region ConstructorAndDispose
    public SqliteInMemoryMatchTests()
    {
        // Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
        // at the end of the test (see Dispose below).
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        // These options will be used by the context instances in this test suite, including the connection opened above.
        _contextOptions = new DbContextOptionsBuilder<AghanimsFantasyContext>()
            .UseSqlite(_connection)
            .EnableSensitiveDataLogging()
            .Options;

        // Create the schema and seed some data
        using var context = new AghanimsFantasyContext(_contextOptions);

        if (context.Database.EnsureCreated())
        {
            // using var viewCommand = context.Database.GetDbConnection().CreateCommand();
            // viewCommand.CommandText = @"
            // CREATE VIEW AllResources AS
            // SELECT Url
            // FROM Blogs;";
            // viewCommand.ExecuteNonQuery();
        }

        context.Leagues.AddRange(
            new League
            {
                Id = 100,
                Name = "test league 1",
                IsActive = true,
                FantasyLeagues = [
                    new FantasyLeague
                    {
                        Id = 1,
                        LeagueId = 1,
                        Name = "test league 1",
                        IsActive = true,
                        FantasyDraftLocked = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
                        LeagueStartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
                        LeagueEndTime = new DateTimeOffset(new DateTime(2024, 12, 31)).ToUnixTimeSeconds()
                    }
                ],
                MatchHistories = [
                    new MatchHistory
                    {
                        MatchId = 1,
                        LeagueId = 1,
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
                        LeagueId = 1,
                        DireTeamId = 2,
                        RadiantTeamId = 3,
                        LobbyType = 0,
                        MatchSeqNum = 0,
                        StartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
                        SeriesId = 0,
                        SeriesType = 0,
                        Players = new List<MatchHistoryPlayer>()
                    }
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
                        LeagueId = 1,
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
                        LeagueId = 1,
                        LobbyType = 0,
                        MatchSeqNum = 0,
                        PicksBans = new List<MatchDetailsPicksBans>(),
                        Players = [
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
                    }
                ],
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
                        LeagueId = 2,
                        Name = "test league 2",
                        IsActive = false,
                        FantasyDraftLocked = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                        LeagueStartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
                        LeagueEndTime = new DateTimeOffset(new DateTime(2024, 12, 31)).ToUnixTimeSeconds()
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
                        StartTime = new DateTimeOffset(new DateTime(2023, 1, 1)).ToUnixTimeSeconds(),
                        TowerStatusDire = 0,
                        TowerStatusRadiant = 0
                    }
                ]
            }
        );

        context.SaveChanges();
    }

    AghanimsFantasyContext CreateContext() => new AghanimsFantasyContext(_contextOptions);

    public void Dispose()
    {
        _connection.Dispose();
    }
    #endregion

    #region Matches
    [Fact]
    public async void GetLeagueMatches()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<WebApiRepository>>();
        var repository = new WebApiRepository(loggerMock.Object, context);

        var matchHistories = await repository.GetMatchHistoryByFantasyLeagueAsync(1);
        Assert.Equal(2, matchHistories.Count());
        Assert.IsAssignableFrom<IEnumerable<MatchHistory>>(matchHistories);
    }

    [Fact]
    public async void GetSingleMatch()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<WebApiRepository>>();
        var repository = new WebApiRepository(loggerMock.Object, context);

        var matchDetail = await repository.GetMatchDetailAsync(1);
        Assert.NotNull(matchDetail);
        Assert.Equal(1, matchDetail.MatchId);
        Assert.IsAssignableFrom<MatchDetail>(matchDetail);
    }

    [Fact]
    public async void GetUnknownMatch()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<WebApiRepository>>();
        var repository = new WebApiRepository(loggerMock.Object, context);

        var matchDetail = await repository.GetMatchDetailAsync(5);
        Assert.Null(matchDetail);
    }

    [Fact]
    public async void GetOutdatedMatch()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<WebApiRepository>>();
        var repository = new WebApiRepository(loggerMock.Object, context);

        var matchDetail = await repository.GetMatchDetailsByFantasyLeagueAsync(2);
        // Match Detail 4 exists but is older than league start time
        Assert.Null(matchDetail.Where(md => md.MatchId == 4).FirstOrDefault());
    }

    [Fact]
    public async void GetMatchDetails()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<WebApiRepository>>();
        var repository = new WebApiRepository(loggerMock.Object, context);

        var matchDetails = await repository.GetMatchDetailsByFantasyLeagueAsync(1);
        Assert.Equal(2, matchDetails.Count());
        Assert.IsAssignableFrom<IEnumerable<MatchDetail>>(matchDetails);
    }

    [Fact]
    public async void GetEmptyLeagueMatchDetails()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<WebApiRepository>>();
        var repository = new WebApiRepository(loggerMock.Object, context);

        var matchDetails = await repository.GetMatchDetailsByFantasyLeagueAsync(3);
        Assert.Empty(matchDetails);
    }


    [Fact]
    public async void GetMatchDetailPlayers()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<WebApiRepository>>();
        var repository = new WebApiRepository(loggerMock.Object, context);

        var matchDetailPlayers = await repository.GetMatchDetailPlayersByLeagueAsync(100);
        Assert.Equal(2, matchDetailPlayers.Count());
        Assert.IsAssignableFrom<IEnumerable<MatchDetailsPlayer>>(matchDetailPlayers);
    }

    [Fact]
    public async void GetMatchDetailPlayersAllLeagues()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<WebApiRepository>>();
        var repository = new WebApiRepository(loggerMock.Object, context);

        var matchDetailPlayers = await repository.GetMatchDetailPlayersByLeagueAsync(null);
        Assert.Equal(2, matchDetailPlayers.Count());
        Assert.IsAssignableFrom<IEnumerable<MatchDetailsPlayer>>(matchDetailPlayers);
    }

    #endregion
}