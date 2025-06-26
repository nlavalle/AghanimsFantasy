using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;

namespace csharp_ef_webapi.IntegrationTests;

public class AuthTests :
    IClassFixture<AghanimsFantasyWebApiFactory<Program>>
{
    private readonly AghanimsFantasyWebApiFactory<Program>
        _factory;

    public AuthTests(
        AghanimsFantasyWebApiFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_SecurePageRedirectsAnUnauthenticatedUser()
    {
        // Arrange
        var client = _factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

        // Act
        var response = await client.GetAsync("https://localhost:5001/api/auth/authorization");

        // Assert
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("https://localhost:5001/Account/Login?ReturnUrl=%2Fapi%2Fauth%2Fauthorization",
            response?.Headers?.Location?.OriginalString ?? "fail");
    }

    [Fact]
    public async Task Get_SecurePageIsReturnedForAnAuthenticatedUser()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth();

        //Act
        var response = await client.GetAsync("https://localhost:5001/api/auth/authorization");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}