namespace DataAccessLibrary.UnitTests.Data;

using Moq;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DataAccessLibrary.Models.ProMetadata;
using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.WebApi;

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

        League league100 = new League
        {
            Id = 100,
            Name = "test league 1",
            IsActive = true
        };

        league100.FantasyLeagues.Add(new FantasyLeague
        {
            Id = 1,
            Name = "test league 1",
            League = league100,
            IsActive = true,
            FantasyDraftLocked = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
            LeagueStartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
            LeagueEndTime = new DateTimeOffset(new DateTime(2024, 12, 31)).ToUnixTimeSeconds()
        });

        league100.MatchHistories.AddRange([
            new MatchHistory
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
                Players = new List<MatchHistoryPlayer>()
            },
            new MatchHistory
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
                Players = new List<MatchHistoryPlayer>()
            }
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
            LeagueId = 1,
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

        matchDetail1.Players.Add(
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
            }
        );

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
            LeagueId = 1,
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

        matchDetail2.Players.Add(
            new MatchDetailsPlayer
            {
                Id = 2,
                Match = matchDetail2,
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
        );


        league100.MatchDetails.AddRange([
            matchDetail1,
            matchDetail2
        ]);

        League league200 = new League
        {
            Id = 200,
            Name = "test league 2",
            IsActive = false
        };

        league200.FantasyLeagues.Add(
            new FantasyLeague
            {
                Id = 2,
                League = league200,
                Name = "test league 2",
                IsActive = false,
                FantasyDraftLocked = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                LeagueStartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
                LeagueEndTime = new DateTimeOffset(new DateTime(2024, 12, 31)).ToUnixTimeSeconds()
            }
        );

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

        league200.MatchDetails.Add(
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
        );

        context.Leagues.AddRange(
            league100,
            league200
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
        var loggerMock = new Mock<ILogger<MatchHistoryRepository>>();
        var fantasyLeagueLoggerMock = new Mock<ILogger<FantasyLeagueRepository>>();
        var repository = new MatchHistoryRepository(loggerMock.Object, context);
        var fantasyLeagueRepository = new FantasyLeagueRepository(fantasyLeagueLoggerMock.Object, context);

        var fantasyLeague = await fantasyLeagueRepository.GetByIdAsync(1);
        var matchHistories = await repository.GetByFantasyLeagueAsync(fantasyLeague!);
        Assert.Equal(2, matchHistories.Count());
        Assert.IsAssignableFrom<IEnumerable<MatchHistory>>(matchHistories);
    }

    [Fact]
    public async void GetSingleMatch()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<MatchDetailRepository>>();
        var repository = new MatchDetailRepository(loggerMock.Object, context);

        var matchDetail = await repository.GetByIdAsync(1);
        Assert.NotNull(matchDetail);
        Assert.Equal(1, matchDetail.MatchId);
        Assert.IsAssignableFrom<MatchDetail>(matchDetail);
    }

    [Fact]
    public async void GetUnknownMatch()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<MatchDetailRepository>>();
        var repository = new MatchDetailRepository(loggerMock.Object, context);

        var matchDetail = await repository.GetByIdAsync(5);
        Assert.Null(matchDetail);
    }

    [Fact]
    public async void GetOutdatedMatch()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<MatchDetailRepository>>();
        var fantasyLeagueLoggerMock = new Mock<ILogger<FantasyLeagueRepository>>();
        var repository = new MatchDetailRepository(loggerMock.Object, context);
        var fantasyLeagueRepository = new FantasyLeagueRepository(fantasyLeagueLoggerMock.Object, context);

        var fantasyLeague = await fantasyLeagueRepository.GetByIdAsync(2);
        var matchDetail = await repository.GetByFantasyLeagueAsync(fantasyLeague!);
        // Match Detail 4 exists but is older than league start time
        Assert.Null(matchDetail.Where(md => md.MatchId == 4).FirstOrDefault());
    }

    [Fact]
    public async void GetMatchDetails()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<MatchDetailRepository>>();
        var fantasyLeagueLoggerMock = new Mock<ILogger<FantasyLeagueRepository>>();
        var repository = new MatchDetailRepository(loggerMock.Object, context);
        var fantasyLeagueRepository = new FantasyLeagueRepository(fantasyLeagueLoggerMock.Object, context);

        var fantasyLeague = await fantasyLeagueRepository.GetByIdAsync(1);
        var matchDetails = await repository.GetByFantasyLeagueAsync(fantasyLeague!);
        Assert.Equal(2, matchDetails.Count());
        Assert.IsAssignableFrom<IEnumerable<MatchDetail>>(matchDetails);
    }

    [Fact]
    public async void GetEmptyLeagueMatchDetails()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<MatchDetailRepository>>();
        var fantasyLeagueLoggerMock = new Mock<ILogger<FantasyLeagueRepository>>();
        var repository = new MatchDetailRepository(loggerMock.Object, context);
        var fantasyLeagueRepository = new FantasyLeagueRepository(fantasyLeagueLoggerMock.Object, context);

        var fantasyLeague = await fantasyLeagueRepository.GetByIdAsync(2);
        var matchDetails = await repository.GetByFantasyLeagueAsync(fantasyLeague!);
        Assert.Empty(matchDetails);
    }


    [Fact]
    public async void GetMatchDetailPlayers()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<MatchDetailRepository>>();
        var repository = new MatchDetailRepository(loggerMock.Object, context);

        var league = await context.Leagues.FindAsync(100);
        var matchDetailPlayers = await repository.GetByLeagueAsync(league!);
        Assert.Equal(2, matchDetailPlayers.Count());
        Assert.IsAssignableFrom<IEnumerable<MatchDetailsPlayer>>(matchDetailPlayers);
    }

    [Fact]
    public async void GetMatchDetailPlayersAllLeagues()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<MatchDetailRepository>>();
        var repository = new MatchDetailRepository(loggerMock.Object, context);

        var matchDetailPlayers = await repository.GetByLeagueAsync(null);
        Assert.Equal(2, matchDetailPlayers.Count());
        Assert.IsAssignableFrom<IEnumerable<MatchDetailsPlayer>>(matchDetailPlayers);
    }

    #endregion
}