using Moq;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using csharp_ef_webapi.Services;


namespace csharp_ef_webapi.UnitTests.Data;

public class SqliteInMemoryDiscordTests : IDisposable
{
    // private readonly ILogger<FantasyRepository> loggerMock = new Mock<ILogger<FantasyRepository>>();
    private readonly DbConnection _connection;
    private readonly DbContextOptions<AghanimsFantasyContext> _contextOptions;


    #region ConstructorAndDispose
    public SqliteInMemoryDiscordTests()
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

        context.SaveChanges();
    }

    AghanimsFantasyContext CreateContext() => new AghanimsFantasyContext(_contextOptions);

    public void Dispose() => _connection.Dispose();
    #endregion

    #region Discord
    // [Fact]
    // public async void GetAllTeams()
    // {
    //     using var context = CreateContext();
    //     var loggerMock = new Mock<ILogger<FantasyRepository>>();
    //     var repository = new FantasyRepository(loggerMock.Object, context);

    //     var teams = await repository.GetTeamsAsync();

    //     Assert.Equal(2, teams.Count());
    //     Assert.IsAssignableFrom<IEnumerable<Team>>(teams);
    // }

    [Fact]
    public async Task GetUserData_BadToken_ReturnsUnauthorized()
    {
        // Arrange
        using var context = CreateContext();
        var httpClient = new HttpClient(new MockHttpMessageHandler(HttpStatusCode.Unauthorized));
        var loggerMock = new Mock<ILogger<DiscordWebApiService>>();
        var discordApiService = new DiscordWebApiService(loggerMock.Object, context, httpClient);

        // Act
        await discordApiService.GetDiscordByIdAsync(1234L);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>  v.ToString() == "401 Unauthorized error on Discord call"),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)
                ), 
            Times.Once
        );
    }

    [Fact]
    public async Task GetUserData_MissingUsernameField_LogsError()
    {
        // Arrange
        using var context = CreateContext();
        var responseBody = "{\"id\": \"123\"}"; // Missing "id" field
        var httpClient = new HttpClient(new MockHttpMessageHandler(HttpStatusCode.OK, responseBody));
        var loggerMock = new Mock<ILogger<DiscordWebApiService>>();
        var discordApiService = new DiscordWebApiService(loggerMock.Object, context, httpClient);

        // Act
        await discordApiService.GetDiscordByIdAsync(1234L);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>  v.ToString() == "Malformed or missing Discord response for user ID 1234"),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)
                ), 
            Times.Once
        );
    }

    [Fact]
    public async Task GetUserData_ValidateSuccessfulInsertInDb()
    {
        // Arrange
        using var context = CreateContext();
        var responseBody = "{\"id\": \"123\", \"username\": \"pants\"}";
        var httpClient = new HttpClient(new MockHttpMessageHandler(HttpStatusCode.OK, responseBody));
        var loggerMock = new Mock<ILogger<DiscordWebApiService>>();
        var discordApiService = new DiscordWebApiService(loggerMock.Object, context, httpClient);

        // Act
        await discordApiService.GetDiscordByIdAsync(123L);

        // Assert
        Assert.True(context.DiscordIds.Where(di => di.DiscordId == 123).Count() == 1);
        Assert.True(context.DiscordIds.Where(di => di.DiscordName == "pants").Count() == 1);
    }

    // Helper class for mocking HTTP responses
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string? _responseBody;

        public MockHttpMessageHandler(HttpStatusCode statusCode, string? responseBody = null)
        {
            _statusCode = statusCode;
            _responseBody = responseBody;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(_statusCode);
            if (_responseBody != null)
            {
                response.Content = new StringContent(_responseBody);
            }
            return await Task.FromResult(response);
        }
    }

    #endregion
}