namespace csharp_ef_data_loader.Services;

using DataAccessLibrary.Data;
using DataAccessLibrary.Models.ProMetadata;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

internal class HeroesContext : DotaOperationContext
{
    private static readonly string AppendedApiPath = "GetHeroes/v1";
    private readonly ILogger<HeroesContext> _logger;
    private readonly HttpClient _httpClient;
    private readonly ProMetadataRepository _proMetadataRepository;

    public HeroesContext(
            ILogger<HeroesContext> logger,
            HttpClient httpClient,
            ProMetadataRepository proMetadataRepository,
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

        JToken responseRawJToken = JToken.Parse(await response.Content.ReadAsStringAsync());

        // Read and deserialize the matches from the json response
        JToken responseObject = responseRawJToken["result"] ?? "{}";
        JToken heroesJson = responseObject["heroes"] ?? "[]";

        List<Hero> heroesResponse = JsonConvert.DeserializeObject<List<Hero>>(heroesJson.ToString()) ?? new List<Hero>();

        return heroesResponse;
    }
}
