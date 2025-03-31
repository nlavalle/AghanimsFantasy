namespace DataAccessLibrary.IntegrationTests.Data;

using Moq;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DataAccessLibrary.Models.ProMetadata;

public class SqliteInMemoryTeamTests : IDisposable
{
    // private readonly ILogger<FantasyRepository> loggerMock = new Mock<ILogger<FantasyRepository>>();
    private readonly DbConnection _connection;
    private readonly DbContextOptions<AghanimsFantasyContext> _contextOptions;


    #region ConstructorAndDispose
    public SqliteInMemoryTeamTests()
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

        context.Teams.AddRange(
            new Team
            {
                Id = 1,
                TimeCreated = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                GamesPlayed = 0,
                Player0AccountId = 0,
                Player1AccountId = 0,
                Player2AccountId = 0,
                Player3AccountId = 0,
                Player4AccountId = 0,
                Player5AccountId = 0,
                AdminAccountId = 0
            },
            new Team
            {
                Id = 2,
                Name = "Test Team",
                TimeCreated = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                GamesPlayed = 0,
                Player0AccountId = 0,
                Player1AccountId = 0,
                Player2AccountId = 0,
                Player3AccountId = 0,
                Player4AccountId = 0,
                Player5AccountId = 0,
                AdminAccountId = 0
            }
        );

        context.SaveChanges();
    }

    AghanimsFantasyContext CreateContext() => new AghanimsFantasyContext(_contextOptions);

    public void Dispose() => _connection.Dispose();
    #endregion

    #region Teams
    [Fact]
    public async void GetAllTeams()
    {
        using var context = CreateContext();
        var teams = await context.Teams.ToListAsync();

        Assert.Equal(2, teams.Count());
        Assert.IsAssignableFrom<IEnumerable<Team>>(teams);
    }
    #endregion
}