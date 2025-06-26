using System.Net;
using System.Text.Json;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace csharp_ef_webapi.IntegrationTests;

public class PrizeTests :
    IClassFixture<AghanimsFantasyWebApiFactory<Program>>
{
    private readonly AghanimsFantasyWebApiFactory<Program>
        _factory;

    public PrizeTests(
        AghanimsFantasyWebApiFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_Prizes()
    {
        // Arrange
        var client = _factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

        // Act
        var response = await client.GetAsync("https://localhost:5001/api/prize");
        var responseString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var prizes = JsonSerializer.Deserialize<string[]>(responseString);
        Assert.Equal(["KILL_STREAK_FLAMES", "STICKER", "LEADERBOARD_BADGE"], prizes);
    }

    [Fact]
    public async Task Post_PurchasePrize_Fail_NotEnoughMoney()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth();

        //Act
        var response = await client.PostAsync("https://localhost:5001/api/prize?Prize=KILL_STREAK_FLAMES", null);
        var responseString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Unable to afford prize", responseString);
    }

    [Fact]
    public async Task Post_PurchasePrize_Fail_NotValidPrize()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth();

        //Act
        var response = await client.PostAsync("https://localhost:5001/api/prize?Prize=UNICORNS", null);
        var responseString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Not a valid prize", responseString);
    }

    [Fact]
    public async Task Post_PurchasePrize_Success()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth();
        // Add ledger shards so prize can be afforded
        AddPrizeCostToLedger();

        //Act
        var response = await client.PostAsync("https://localhost:5001/api/prize?Prize=KILL_STREAK_FLAMES", null);
        var responseString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var prizes = JsonSerializer.Deserialize<string[]>(responseString);
        Assert.Equal(["KILL_STREAK_FLAMES", "STICKER", "LEADERBOARD_BADGE"], prizes);
    }

    [Fact]
    public async Task Post_GetUnlockedPrizes_Success()
    {
        // Arrange
        var client = _factory.CreateClientWithAuth();

        //Act - Check Empty Response
        var emptyResponse = await client.GetAsync("https://localhost:5001/api/prize/unlocked");
        var emptyResponseString = await emptyResponse.Content.ReadAsStringAsync();

        // Assert - No Prizes
        Assert.Equal(HttpStatusCode.OK, emptyResponse.StatusCode);
        Assert.Equal("[]", emptyResponseString);

        // Add ledger shards so prize can be afforded
        AddPrizeCostToLedger();

        //Act - Unlock Prize
        var unlockResponse = await client.PostAsync("https://localhost:5001/api/prize?Prize=KILL_STREAK_FLAMES", null);
        var unlockResponseString = await unlockResponse.Content.ReadAsStringAsync();

        // Assert - Prize Unlocked Successfully
        Assert.Equal(HttpStatusCode.OK, unlockResponse.StatusCode);
        var prizes = JsonSerializer.Deserialize<string[]>(unlockResponseString);
        Assert.Equal(["KILL_STREAK_FLAMES", "STICKER", "LEADERBOARD_BADGE"], prizes);

        //Act - Get Prizes expecting unlocked
        var unlockedResponse = await client.GetAsync("https://localhost:5001/api/prize/unlocked");
        var unlockedResponseString = await unlockedResponse.Content.ReadAsStringAsync();

        // Assert - Unlocked prizes in response
        Assert.Equal(HttpStatusCode.OK, emptyResponse.StatusCode);
        var fantasyPrize = JsonSerializer.Deserialize<IEnumerable<FantasyPrize>>(unlockedResponseString);
        Assert.NotNull(fantasyPrize);
        Assert.Single(fantasyPrize);
        var fantasyPrizeSingle = fantasyPrize.First();
        Assert.Equal("test-user-id", fantasyPrizeSingle.UserId);
        Assert.Equal(FantasyPrizeOption.KILL_STREAK_FLAMES, fantasyPrizeSingle.FantasyPrizeOption);
        Assert.True(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 60 < fantasyPrizeSingle.PrizeTimestamp);
    }

    private void AddPrizeCostToLedger()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AghanimsFantasyContext>();
        dbContext.FantasyLedger.Add(new FantasyLedger
        {
            UserId = "test-user-id",
            Amount = FantasyPrizeOption.KILL_STREAK_FLAMES.GetCost(),
            DiscordId = 0,
            LedgerRecordedTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            SourceId = 1,
            SourceType = "Test"
        });
        dbContext.SaveChanges();
    }
}