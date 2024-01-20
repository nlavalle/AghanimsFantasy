using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace csharp_ef_webapi.Services;
public class DotaWebApiService : BackgroundService
{
    private readonly string _baseApiUrl;
    private readonly string _econApiUrl;
    private readonly string _steamKey;
    private readonly HttpClient _httpClient;
    private TaskCompletionSource _tcsReadyForRequests;
    private readonly ILogger<DotaWebApiService> _logger;
    internal CancellationToken StoppingToken { get; private set; }
    internal RateLimiter RateLimiter { get; } = new RateLimiter(_delayBetweenRequests);
    private const long _delayBetweenRequests = 10_000_000; // Constant for ticks in a second
    IConfiguration _configuration;
    IServiceProvider _serviceProvider;
    Dictionary<Type, object> _launchers;

    public DotaWebApiService(ILogger<DotaWebApiService> logger, IConfiguration configuration, IServiceProvider serviceProvider, HttpClient httpClient)
    {
        _tcsReadyForRequests = new TaskCompletionSource();
        _launchers = new Dictionary<Type, object>();

        _logger = logger;
        _configuration = configuration;
        _serviceProvider = serviceProvider;

        _baseApiUrl = _configuration.GetSection("DotaWebApi").GetValue<string>("BaseUrl");
        _econApiUrl = _configuration.GetSection("DotaWebApi").GetValue<string>("EconUrl");
        _steamKey = Environment.GetEnvironmentVariable("STEAM_KEY") ?? "";

        _httpClient = httpClient;
        _logger.LogInformation("Dota WebApi Service started");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        StoppingToken = stoppingToken;

        // Build our Launchers

        // Build the common base query object
        // All API calls use a steam key, if possible
        var baseQuery = new Dictionary<string, string>();
        if (_steamKey != null)
        {
            baseQuery["key"] = _steamKey;
        }

        var baseApiUri = new Uri(_baseApiUrl);
        var econApiUri = new Uri(_econApiUrl);

        var commonCooldown = TimeSpan.FromMinutes(1);

        CreateLauncher<HeroesContext>(commonCooldown, econApiUri, baseQuery);
        CreateLauncher<LeagueHistoryContext>(commonCooldown, baseApiUri, baseQuery);
        CreateLauncher<MatchDetailsContext>(commonCooldown, baseApiUri, baseQuery);
        CreateLauncher<TeamsContext>(commonCooldown, baseApiUri, baseQuery);

        // Ready
        _tcsReadyForRequests.SetResult();
        _logger.LogInformation("Dota WebApi Service started");

        Task[] tasks = new Task[4];

        tasks[0] = LoopOperation<HeroesContext>(TimeSpan.FromDays(1), stoppingToken);
        tasks[1] = LoopOperation<LeagueHistoryContext>(TimeSpan.FromMinutes(30), stoppingToken);
        tasks[2] = LoopOperation<MatchDetailsContext>(TimeSpan.FromMinutes(5), stoppingToken);
        tasks[3] = LoopOperation<TeamsContext>(TimeSpan.FromDays(1), stoppingToken);


        await Task.WhenAll(tasks);
    }

    //     internal async Task StartOperation<T>(CancellationToken cancellationToken, bool ignoreCooldown = false) where T : DotaOperationContext
    //     {
    //         await _tcsReadyForRequests.Task;

    //         if (!TryGetLauncher<T>(out var launcher))
    //         {
    //             // TODO: Specify
    //             throw new InvalidOperationException();
    //         }

    //         await launcher.StartOperation(cancellationToken, ignoreCooldown);
    //     }

    private void CreateLauncher<T>(TimeSpan cooldown, Uri baseApiUri, Dictionary<string, string> baseQuery) where T : DotaOperationContext
    {
        Debug.Assert(!_tcsReadyForRequests.Task.IsCompleted);

        _launchers.Add(typeof(T), new DotaOperationLauncher<T>(_serviceProvider, cooldown, baseApiUri, baseQuery, RateLimiter, StoppingToken));
    }

    private async Task LoopOperation<T>(TimeSpan delay, CancellationToken cancellationToken) where T : DotaOperationContext
    {
        Debug.Assert(_tcsReadyForRequests.Task.IsCompleted);

        if (!TryGetLauncher<T>(out var launcher))
        {
            // TODO: Specify
            throw new InvalidOperationException();
        }

        var argTicks = delay.Ticks;
        if (argTicks < launcher.CoolDownTicks)
            throw new ArgumentOutOfRangeException(nameof(delay), "Provided delay is less than the cooldown of this call.");

        T? old = null;
        while (!cancellationToken.IsCancellationRequested)
        {
            var current = launcher.GetInstance(false);

            if (current != old)
            {
                old = current;
                await current.ExecuteAsync(cancellationToken);
            }

            var now = DateTimeOffset.UtcNow.Ticks;
            var stop = current.StopTicks;
            Debug.Assert(stop < now);
            Debug.Assert(stop != 0);

            var wait = now - stop + argTicks;
            if (wait > 0)
            {
                await Task.Delay(new TimeSpan(wait), cancellationToken);
            }
        }
    }

    private bool TryGetLauncher<T>([NotNullWhen(true)] out DotaOperationLauncher<T>? launcher) where T : DotaOperationContext
    {
        Debug.Assert(_tcsReadyForRequests.Task.IsCompleted);

        if (_launchers.TryGetValue(typeof(T), out var value))
        {
            launcher = (DotaOperationLauncher<T>)value;
            return true;
        }

        launcher = null;
        return false;
    }
}