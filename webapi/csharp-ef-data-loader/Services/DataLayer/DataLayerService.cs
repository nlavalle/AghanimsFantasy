namespace csharp_ef_data_loader.Services;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class DataLayerService : BackgroundService
{
    private TaskCompletionSource _tcsReadyForRequests;
    private readonly ILogger<DataLayerService> _logger;
    internal CancellationToken StoppingToken { get; private set; }
    internal RateLimiter RateLimiter { get; } = new RateLimiter(_delayBetweenRequests * 2);
    private const long _delayBetweenRequests = 10_000_000; // Constant for ticks in a second\
    IServiceProvider _serviceProvider;
    Dictionary<Type, object> _launchers;

    public DataLayerService(ILogger<DataLayerService> logger, IServiceProvider serviceProvider)
    {
        _tcsReadyForRequests = new TaskCompletionSource();
        _launchers = new Dictionary<Type, object>();

        _logger = logger;
        _serviceProvider = serviceProvider;

        _logger.LogInformation("Dota WebApi Service constructed");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        StoppingToken = stoppingToken;

        // Build our Launchers
        var commonCooldown = TimeSpan.FromMinutes(1);

        CreateLauncher<FantasyNormalizedAveragesContext>(commonCooldown);
        CreateLauncher<FantasyPlayerBudgetProbabilityContext>(commonCooldown);
        CreateLauncher<FantasyMatchContext>(commonCooldown);
        CreateLauncher<FantasyLedgerContext>(commonCooldown);

        // Ready
        _tcsReadyForRequests.SetResult();
        _logger.LogInformation("Data Layer Service started");

        Task[] tasks =
        [
            LoopOperation<FantasyNormalizedAveragesContext>(TimeSpan.FromHours(1), stoppingToken),
            LoopOperation<FantasyPlayerBudgetProbabilityContext>(TimeSpan.FromDays(1), stoppingToken),
            LoopOperation<FantasyMatchContext>(TimeSpan.FromMinutes(5), stoppingToken),
            LoopOperation<FantasyLedgerContext>(TimeSpan.FromHours(1), stoppingToken),
        ];
        await Task.WhenAll(tasks);
    }

    private void CreateLauncher<T>(TimeSpan cooldown) where T : DotaOperationContext
    {
        Debug.Assert(!_tcsReadyForRequests.Task.IsCompleted);

        _launchers.Add(typeof(T), new DataLayerOperationLauncher<T>(_serviceProvider, cooldown, RateLimiter, StoppingToken));
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

    private bool TryGetLauncher<T>([NotNullWhen(true)] out DataLayerOperationLauncher<T>? launcher) where T : DotaOperationContext
    {
        Debug.Assert(_tcsReadyForRequests.Task.IsCompleted);

        if (_launchers.TryGetValue(typeof(T), out var value))
        {
            launcher = (DataLayerOperationLauncher<T>)value;
            return true;
        }

        launcher = null;
        return false;
    }
}