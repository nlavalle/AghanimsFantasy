using csharp_ef_webapi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace csharp_ef_webapi.Services;
internal class HeroesContext : DotaOperationContext
{
    private static readonly string AppendedApiPath = "GetHeroes/v1";
    // private readonly DotaWebApiService _apiService;
    private readonly AghanimsFantasyContext _dbContext;
    private readonly ILogger<HeroesContext> _logger;
    private readonly HttpClient _httpClient;

    public HeroesContext(ILogger<HeroesContext> logger, HttpClient httpClient, IServiceScope scope, Config config)
        : base(scope, config)
    {
        _dbContext = scope.ServiceProvider.GetRequiredService<AghanimsFantasyContext>();
        _logger = logger;
        _httpClient = httpClient;
    }

    protected override async Task OperateAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Fetching heroes");

        List<Hero> heroes = new List<Hero>();
        heroes = await GetHeroesAsync(cancellationToken);

        foreach (Hero hero in heroes)
        {
            if (_dbContext.Heroes.FirstOrDefault(h => h.Id == hero.Id) == null)
            {
                _dbContext.Heroes.Add(hero);
            }
            else
            {
                Hero updateHero = _dbContext.Heroes.First(h => h.Id == hero.Id);
                updateHero.Name = hero.Name;
                _dbContext.Heroes.Update(updateHero);
            }
        }
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"Hero fetch done");
    }

    private async Task<List<Hero>> GetHeroesAsync(CancellationToken cancellationToken)
    {
        UriBuilder uriBuilder = new UriBuilder(_config.BaseUri);
        uriBuilder.AppendPath(AppendedApiPath);

        Dictionary<string, string> query = new Dictionary<string, string>(_config.BaseQuery);

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
