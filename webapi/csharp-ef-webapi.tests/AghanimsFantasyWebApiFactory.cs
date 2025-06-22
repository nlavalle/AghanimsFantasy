using System.Data.Common;
using DataAccessLibrary.Data.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace csharp_ef_webapi.IntegrationTests;

public class AghanimsFantasyWebApiFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly SqliteConnection _connection;
    public AghanimsFantasyWebApiFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<AghanimsFantasyContext>));

            if (dbContextDescriptor != null) services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbConnection));

            if (dbConnectionDescriptor != null) services.Remove(dbConnectionDescriptor);

            services.AddDbContext<AghanimsFantasyContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AghanimsFantasyContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                SeedTestUsers(db);

                db.SaveChanges();
            }

        });

        builder.UseEnvironment("Development");

        // Add env variables
        Environment.SetEnvironmentVariable("DISCORD_APP_ID", "discord_app_id");
        Environment.SetEnvironmentVariable("DISCORD_APP_SECRET", "discord_app_secret");
        Environment.SetEnvironmentVariable("DISCORD_BOT_TOKEN", "discord_bot_token");
        Environment.SetEnvironmentVariable("GOOGLE_CLIENT_ID", "google_client_id");
        Environment.SetEnvironmentVariable("GOOGLE_CLIENT_SECRET", "google_client_secret");
        Environment.SetEnvironmentVariable("MAILGUN_DOMAIN", "mailgun_domain");
        Environment.SetEnvironmentVariable("MAILGUN_KEY", "mailgun_key");
    }

    public HttpClient CreateClientWithAuth()
    {
        var client = WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(defaultScheme: "TestScheme")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        "TestScheme", options => { });

                services.PostConfigure<AuthenticationOptions>(options =>
                {
                    options.DefaultAuthenticateScheme = "TestScheme";
                    options.DefaultChallengeScheme = "TestScheme";
                    options.DefaultScheme = "TestScheme";
                });
            });
        })
        .CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "TestScheme");
        return client;
    }

    private void SeedTestUsers(AghanimsFantasyContext database)
    {
        database.Users.Add(new AghanimsFantasyUser
        {
            Id = "test-user-id",
            UserName = "testuser@example.com",
            Email = "testuser@example.com",
            EmailConfirmed = true
        });
    }
}