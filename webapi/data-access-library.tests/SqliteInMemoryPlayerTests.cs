namespace DataAccessLibrary.IntegrationTests.Data;

using Moq;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DataAccessLibrary.Models.ProMetadata;

public class SqliteInMemoryPlayerTests : IDisposable
{
    // private readonly ILogger<FantasyRepository> loggerMock = new Mock<ILogger<FantasyRepository>>();
    private readonly DbConnection _connection;
    private readonly DbContextOptions<AghanimsFantasyContext> _contextOptions;


    #region ConstructorAndDispose
    public SqliteInMemoryPlayerTests()
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

        context.Accounts.AddRange(
            new Account
            {
                Id = 1,
                Name = "account 1",
                SteamProfilePicture = "profile pic 1"
            },
            new Account
            {
                Id = 2,
                Name = "account 2",
                SteamProfilePicture = "profile pic 2"
            }
        );

        context.SaveChanges();
    }

    AghanimsFantasyContext CreateContext() => new AghanimsFantasyContext(_contextOptions);

    public void Dispose() => _connection.Dispose();
    #endregion

    #region Players
    [Fact]
    public async void GetAllPlayers()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<ProMetadataRepository>>();
        var repository = new ProMetadataRepository(loggerMock.Object, context);

        var players = await repository.GetPlayerAccounts();

        Assert.Equal(2, players.Count());
        Assert.IsAssignableFrom<IEnumerable<Account>>(players);
    }
    #endregion
}