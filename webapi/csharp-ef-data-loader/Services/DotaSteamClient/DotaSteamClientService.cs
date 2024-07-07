namespace csharp_ef_data_loader.Services;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

public class DotaSteamClientService : BackgroundService
{
    private TaskCompletionSource _tcsReadyForRequests;
    private readonly ILogger<DotaWebApiService> _logger;
    internal CancellationToken StoppingToken { get; private set; }
    internal RateLimiter RateLimiter { get; } = new RateLimiter(_delayBetweenRequests);
    private const long _delayBetweenRequests = 10_000_000; // Constant for ticks in a second
    IServiceProvider _serviceProvider;
    Dictionary<Type, object> _launchers;

    public DotaSteamClientService(ILogger<DotaWebApiService> logger, IServiceProvider serviceProvider)
    {
        _tcsReadyForRequests = new TaskCompletionSource();
        _launchers = new Dictionary<Type, object>();

        _logger = logger;
        _serviceProvider = serviceProvider;

        _logger.LogInformation("Dota Steam Client Service constructed");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        StoppingToken = stoppingToken;

        // Build our Launchers

        var commonCooldown = TimeSpan.FromMinutes(1);

        CreateLauncher<SteamClientMatchDetailsContext>(commonCooldown);

        // Ready
        _tcsReadyForRequests.SetResult();
        _logger.LogInformation("Dota Steam Client Service started");

        Task[] tasks =
        [
            LoopOperation<SteamClientMatchDetailsContext>(TimeSpan.FromMinutes(10), stoppingToken)
        ];
        await Task.WhenAll(tasks);
    }

    private void CreateLauncher<T>(TimeSpan cooldown) where T : DotaOperationContext
    {
        Debug.Assert(!_tcsReadyForRequests.Task.IsCompleted);

        _launchers.Add(typeof(T), new DotaSteamClientOperationLauncher<T>(_serviceProvider, cooldown, RateLimiter, StoppingToken));
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

    private bool TryGetLauncher<T>([NotNullWhen(true)] out DotaSteamClientOperationLauncher<T>? launcher) where T : DotaOperationContext
    {
        Debug.Assert(_tcsReadyForRequests.Task.IsCompleted);

        if (_launchers.TryGetValue(typeof(T), out var value))
        {
            launcher = (DotaSteamClientOperationLauncher<T>)value;
            return true;
        }

        launcher = null;
        return false;
    }
}