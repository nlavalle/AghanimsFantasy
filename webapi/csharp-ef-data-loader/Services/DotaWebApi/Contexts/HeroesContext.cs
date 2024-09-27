namespace csharp_ef_data_loader.Services;

using DataAccessLibrary.Data;
using DataAccessLibrary.Models.ProMetadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

internal class HeroesContext : DotaOperationContext
{
    private static readonly string AppendedApiPath = "GetHeroes/v1";
    private readonly ILogger<HeroesContext> _logger;
    private readonly HttpClient _httpClient;
    private readonly IProMetadataRepository _proMetadataRepository;

    public HeroesContext(
            ILogger<HeroesContext> logger,
            HttpClient httpClient,
            IProMetadataRepository proMetadataRepository,
            IServiceScope scope,
            Config config
        )
        : base(scope, config)
    {
        _logger = logger;
        _httpClient = httpClient;
        _proMetadataRepository = proMetadataRepository;
    }

    protected override async Task OperateAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Fetching heroes");

        List<Hero> heroes = new List<Hero>();
        heroes = await GetHeroesAsync(cancellationToken);

        foreach (Hero hero in heroes)
        {
            await _proMetadataRepository.UpsertHeroAsync(hero);
        }

        _logger.LogInformation($"Hero fetch done");
    }

    private async Task<List<Hero>> GetHeroesAsync(CancellationToken cancellationToken)
    {
        UriBuilder uriBuilder = new UriBuilder(_config.BaseUri);
        uriBuilder.AppendPath(AppendedApiPath);

        Dictionary<string, string> query = new Dictionary<string, string>(_config.ConfigSettings);

        uriBuilder.SetQuery(query);

        HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

        await WaitNextTaskScheduleAsync(cancellationToken);

        HttpResponseMessage response = await _httpClient.SendAsync(httpRequest);
        _logger.LogInformation($"Request submitted at {DateTime.Now.Ticks}");
        response.EnsureSuccessStatusCode();

        JsonDocument responseRawJDocument = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        // Read and deserialize the matches from the json response
        JsonElement resultElement;
        if (!responseRawJDocument.RootElement.TryGetProperty("result", out resultElement)) throw new Exception("Result element not found in response");
        JsonElement heroesElement;
        if (!resultElement.TryGetProperty("heroes", out heroesElement)) throw new Exception("Heroes element not found in result response");

        List<Hero> heroesResponse = JsonSerializer.Deserialize<List<Hero>>(heroesElement.GetRawText()) ?? throw new Exception("Unable to deserialize response json to Hero");

        return heroesResponse;
    }
}
