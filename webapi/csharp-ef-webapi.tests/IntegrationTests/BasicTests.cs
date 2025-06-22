namespace csharp_ef_webapi.IntegrationTests;

public class BasicTests
    : IClassFixture<AghanimsFantasyWebApiFactory<Program>>
{
    private readonly AghanimsFantasyWebApiFactory<Program> _factory;

    public BasicTests(AghanimsFantasyWebApiFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("/api/auth/authenticated")]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal("application/json; charset=utf-8",
            response?.Content?.Headers?.ContentType?.ToString() ?? "fail");
    }
}