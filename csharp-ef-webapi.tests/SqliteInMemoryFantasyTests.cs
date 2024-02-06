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
                Id = 1,
                LeagueId = 1,
                Name = "test league 1",
                IsActive = true,
                FantasyDraftLocked = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
                LeagueStartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
                LeagueEndTime = new DateTimeOffset(new DateTime(2024, 12, 31)).ToUnixTimeSeconds()
            },
            new League
            {
                Id = 2,
                LeagueId = 2,
                Name = "test league 2",
                IsActive = false,
                FantasyDraftLocked = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                LeagueStartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
                LeagueEndTime = new DateTimeOffset(new DateTime(2024, 12, 31)).ToUnixTimeSeconds()
            }
        );

        context.MatchHistory.AddRange(
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
            },
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
        );

        context.MatchDetails.AddRange(
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
        );

        context.MatchDetailsPlayers.AddRange(
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
        );

        context.FantasyPlayers.AddRange(
            new FantasyPlayer
            {
                Id = 1,
                LeagueId = 1,
                DotaAccountId = 1,
                TeamId = 1,
                DotaAccount = new Account(),
                Team = new Team()
            },
            new FantasyPlayer
            {
                Id = 2,
                LeagueId = 1,
                DotaAccountId = 2,
                TeamId = 2,
                DotaAccount = new Account(),
                Team = new Team()
            },
            new FantasyPlayer
            {
                Id = 3,
                LeagueId = 2,
                DotaAccountId = 3,
                TeamId = 3,
                DotaAccount = new Account(),
                Team = new Team()
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
            LeagueId = 1,
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

        var fantasyPlayers = await repository.FantasyPlayersByLeagueAsync(1);
        Assert.Equal(2, fantasyPlayers.Count());
        Assert.IsAssignableFrom<IEnumerable<FantasyPlayer>>(fantasyPlayers);
    }

    [Fact]
    public async void GetAllLeagueFantasyPlayers()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<FantasyRepository>>();
        var repository = new FantasyRepository(loggerMock.Object, context);

        var fantasyPlayers = await repository.FantasyPlayersByLeagueAsync(null);
        Assert.Equal(3, fantasyPlayers.Count());
        Assert.IsAssignableFrom<IEnumerable<FantasyPlayer>>(fantasyPlayers);
    }

    [Fact]
    public async void GetEmptyLeagueFantasyPlayers()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<FantasyRepository>>();
        var repository = new FantasyRepository(loggerMock.Object, context);

        var fantasyPlayers = await repository.FantasyPlayersByLeagueAsync(3);
        Assert.Empty(fantasyPlayers);
    }

    [Fact]
    public async void GetFantasyPlayerPointsLeague()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<FantasyRepository>>();
        var repository = new FantasyRepository(loggerMock.Object, context);

        var fantasyPlayerPoints = await repository.FantasyPlayerPointsByLeagueAsync(1);
        // This is going to be 2 fantasy players, and 2 null rows with the left join so 0's show if there were no matches
        Assert.Equal(4, fantasyPlayerPoints.Count());
        Assert.Equal(2, fantasyPlayerPoints.Where(fpp => fpp.Match != null).Count());
        Assert.Equal(2, fantasyPlayerPoints.Where(fpp => fpp.Match == null).Count());
        Assert.IsAssignableFrom<IEnumerable<FantasyPlayerPoints>>(fantasyPlayerPoints);
    }

    [Fact]
    public async void GetFantasyPlayerPointsEmptyLeague()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<FantasyRepository>>();
        var repository = new FantasyRepository(loggerMock.Object, context);

        var fantasyPlayerPoints = await repository.FantasyPlayerPointsByLeagueAsync(4);

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