using Moq;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using csharp_ef_webapi.Models.ProMetadata;


namespace csharp_ef_webapi.UnitTests.Data;

public class SqliteInMemoryHeroTests : IDisposable
{
    // private readonly ILogger<FantasyRepository> loggerMock = new Mock<ILogger<FantasyRepository>>();
    private readonly DbConnection _connection;
    private readonly DbContextOptions<AghanimsFantasyContext> _contextOptions;


    #region ConstructorAndDispose
    public SqliteInMemoryHeroTests()
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

        context.Heroes.AddRange(
            new Hero
            {
                Id = 1,
                Name = "hero 1"
            },
            new Hero
            {
                Id = 2,
                Name = "hero 2"
            }
        );

        context.SaveChanges();
    }

    AghanimsFantasyContext CreateContext() => new AghanimsFantasyContext(_contextOptions);

    public void Dispose() => _connection.Dispose();
    #endregion

    #region Heroes
    [Fact]
    public async void GetAllHeroes()
    {
        using var context = CreateContext();
        var loggerMock = new Mock<ILogger<ProMetadataRepository>>();
        var repository = new ProMetadataRepository(loggerMock.Object, context);

        var heroes = await repository.GetHeroesAsync();

        Assert.Equal(2, heroes.Count());
        Assert.IsAssignableFrom<IEnumerable<Hero>>(heroes);
    }
    #endregion
}