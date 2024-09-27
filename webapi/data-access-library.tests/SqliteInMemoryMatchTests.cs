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

        // Create team and accounts to be used later
        for (int i = 1; i < 11; i++)
        {
            context.Accounts.Add(DbSeeder.NewAccount(i));
            context.SaveChanges();
        }

        Team team0 = DbSeeder.NewTeam(1);
        context.Teams.Add(team0);

        League populatedLeague1 = DbSeeder.FullPopulatedMatch(1);

        populatedLeague1.MatchHistories.Add(
            // Outdated match test
            new MatchHistory
            {
                MatchId = 4,
                LeagueId = populatedLeague1.Id,
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

        context.Leagues.Add(populatedLeague1);

        context.SaveChanges();

        // Empty league
        League emptyLeague2 = DbSeeder.NewLeague(2);
        emptyLeague2.FantasyLeagues.Add(new FantasyLeague
        {
            LeagueId = emptyLeague2.Id
        });

        context.Leagues.Add(emptyLeague2);
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
        Assert.Single(matchHistories);
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

        var fantasyLeague = await fantasyLeagueRepository.GetByIdAsync(1);
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
        Assert.Single(matchDetails);
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

        var league = await context.Leagues.FindAsync(1);
        var matchDetailPlayers = await repository.GetByLeagueAsync(league!);
        Assert.Equal(10, matchDetailPlayers.Count());
        Assert.IsAssignableFrom<IEnumerable<MatchDetailsPlayer>>(matchDetailPlayers);
    }

    [Fact]
    public async void GetMatchDetailPlayersAllLeagues()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<MatchDetailRepository>>();
        var repository = new MatchDetailRepository(loggerMock.Object, context);

        var matchDetailPlayers = await repository.GetByLeagueAsync(null);
        Assert.Equal(10, matchDetailPlayers.Count());
        Assert.IsAssignableFrom<IEnumerable<MatchDetailsPlayer>>(matchDetailPlayers);
    }

    #endregion
}