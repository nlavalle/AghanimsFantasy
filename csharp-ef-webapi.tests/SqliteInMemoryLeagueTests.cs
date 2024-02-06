using Moq;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace csharp_ef_webapi.UnitTests.Data;

public class SqliteInMemoryLeagueTests : IDisposable
{
    // private readonly ILogger<FantasyRepository> loggerMock = new Mock<ILogger<FantasyRepository>>();
    private readonly DbConnection _connection;
    private readonly DbContextOptions<AghanimsFantasyContext> _contextOptions;


    #region ConstructorAndDispose
    public SqliteInMemoryLeagueTests()
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
                FantasyDraftLocked = new DateTimeOffset(new DateTime(2024,1,1)).ToUnixTimeSeconds(),
                LeagueStartTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                LeagueEndTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()
            },
            new League
            {
                Id = 2,
                LeagueId = 2,
                Name = "test league 2",
                IsActive = false,
                FantasyDraftLocked = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                LeagueStartTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                LeagueEndTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()
            }
        );
        context.SaveChanges();
    }

    AghanimsFantasyContext CreateContext() => new AghanimsFantasyContext(_contextOptions);

    public void Dispose() => _connection.Dispose();
    #endregion

    #region Leagues
    [Fact]
    public async void GetAllLeaguesTest()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<FantasyRepository>>();
        var repository = new FantasyRepository(loggerMock.Object, context);

        var leagues = await repository.GetLeaguesAsync(null);

        Assert.Collection(
            leagues,
            l => Assert.Equal("test league 1", l.Name),
            l => Assert.Equal("test league 2", l.Name));
    }

    [Fact]
    public async void GetActiveLeaguesTest()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<FantasyRepository>>();
        var repository = new FantasyRepository(loggerMock.Object, context);

        var leagues = await repository.GetLeaguesAsync(true);

        Assert.Collection(
            leagues,
            l => Assert.Equal("test league 1", l.Name)
        );
    }

    [Fact]
    public async void GetLeagueLockedDateTest()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<FantasyRepository>>();
        var repository = new FantasyRepository(loggerMock.Object, context);

        var league = await repository.GetLeagueLockedDate(1);

        Assert.Equal(league, new DateTimeOffset(new DateTime(2024,1,1)).UtcDateTime);
    }
    #endregion
}