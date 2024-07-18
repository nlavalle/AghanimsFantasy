using Microsoft.Extensions.DependencyInjection;

namespace csharp_ef_data_loader.Services;

// This class represents the shared operations for launching contexts
// It creates and passes the scope that the context needs to do DI
// It also ensures that only one context of its type is ever run concurrently from the launcher
// It provides no guarantees that more than one launcher will exist (the API service does this)
internal class DotaSteamClientOperationLauncher<T> where T : DotaOperationContext
{
    // Dependencies
    private readonly IServiceProvider _serviceProvider;

    // Configuration
    internal long CoolDownTicks { get; }
    private readonly DotaOperationContext.Config _contextConfig;

    // Operation
    private readonly object _lockObject = new object();
    private T? _currentOperation;

    public DotaSteamClientOperationLauncher(IServiceProvider serviceProvider, TimeSpan coolDown, RateLimiter rateLimiter, CancellationToken stoppingToken)
    {
        _serviceProvider = serviceProvider;
        CoolDownTicks = coolDown.Ticks;

        _contextConfig = new DotaOperationContext.Config(rateLimiter, stoppingToken);
    }

    public T GetInstance(bool overrideCooldown = false)
    {
        T? old = default,
            current = default;

        lock (_lockObject)
        {
            current = _currentOperation;
            if (current == default)
            {
                // Here the launcher has never created a context, we'll create one
                return CreateInstance();
            }

            // Here we need to check the context that exists to see its status
            var stopTicks = current.StopTicks;
            if (stopTicks != 0)
            {
                // Here the context is completed
                if (overrideCooldown || DateTimeOffset.UtcNow.Ticks - stopTicks >= CoolDownTicks)
                {
                    // Here the context needs to be guaranteed or was completed long enough ago that we can start another
                    old = current;
                    current = CreateInstance();
                }
            }
        }

        if (old != default)
        {
            // Here we are getting rid of the last context, doesn't need to be inside the lock
            old.Dispose();
        }

        return current;

        // Creating the instance, this can only be called inside the lock
        T CreateInstance()
        {
            var scope = _serviceProvider.CreateScope();
            var operation = ActivatorUtilities.CreateInstance<T>(scope.ServiceProvider, scope, _contextConfig);
            _currentOperation = operation;

            return operation;
        }
    }

    public async Task StartOperation(CancellationToken cancellationToken, bool overrideCooldown = false)
    {
        var current = GetInstance(overrideCooldown);

        await current.ExecuteAsync(cancellationToken);
    }
}
