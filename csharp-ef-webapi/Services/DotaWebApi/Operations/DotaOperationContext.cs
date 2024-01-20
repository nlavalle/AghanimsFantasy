namespace csharp_ef_webapi.Services;

// This class represents the shared operations on contexts
// It stores and disposes the scope that was created for the context by the launcher
// It also ensures that only one copy of the operation is ever run within the context
// It provides no guarantees that more than one context will exist (see: DotaOperationLauncher)
// The property LastKnownWindow will track the approximate time that the operation
// will complete its outstanding API requests and can be used as a status indicator
internal abstract class DotaOperationContext : IDisposable
{
    private readonly IServiceScope _scope;
    protected readonly Config _config;

    private readonly CancellationTokenSource _cts;

    // Task 
    private TaskCompletionSource? _tcs = null;
    private long _startTicks = 0;
    private long _stopTicks = 0;
    // Facts:
    // | Condition                          | Operation     | Value
    // | _stopTicks < _startTicks           | Started       |
    // | _stopTicks > _startTicks           | Done          | Run Time
    // | _startTicks == 0                   | Unstarted     |
    // | _startTicks != 0                   | Started       | Start Time
    // | _stopTicks == 0                    | Uncompleted   |
    // | _stopTicks != 0                    | Completed     | Completion Time

    public long StartTicks => Volatile.Read(ref _startTicks);
    public long StopTicks => Volatile.Read(ref _stopTicks);
    public bool IsStarted => StartTicks != 0;
    public bool IsCompleted => StopTicks != 0;

    // Scheduling Status
    private long _lastKnownWindow = 0;

    protected long LastKnownWindow => Volatile.Read(ref _lastKnownWindow);

    protected DotaOperationContext(IServiceScope scope, Config config)
    {
        _scope = scope;
        _config = config;
        _cts = CancellationTokenSource.CreateLinkedTokenSource(config.StoppingToken);
    }

    // Note that this method is not async, it just passes through a Task
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        CancellationToken localToken = _cts.Token;
        bool startTask = false;

        // Using Task to ensure only one thread ever runs the task
        var current = Volatile.Read(ref _tcs);
        if (current == null)
        {
            // Here, a task does not exist yet, so we'll try creating one
            var temp = new TaskCompletionSource();
            current = Interlocked.CompareExchange(ref _tcs, temp, null);
            if (current == null)
            {
                startTask = true;
                current = temp;
            }
        }

        if (startTask)
        {
            Volatile.Write(ref _startTicks, DateTimeOffset.UtcNow.Ticks);

            _ = Task.Run(async () =>
            {
                try
                {
                    await OperateAsync(localToken);
                }
                finally
                {
                    Volatile.Write(ref _stopTicks, DateTimeOffset.UtcNow.Ticks);
                    current.SetResult();
                }
            }, localToken);
        }

        if (cancellationToken == _config.StoppingToken
            || cancellationToken == CancellationToken.None
            || cancellationToken == localToken)
        {
            await current.Task;
        }
        else
        {
            await current.Task.ContinueWith((task) =>
            {
                if (task.Exception != null)
                {
                    throw task.Exception;
                }
            }, cancellationToken);
        }
    }

    protected abstract Task OperateAsync(CancellationToken cancellationToken);

    protected async Task WaitNextTaskScheduleAsync(CancellationToken cancellationToken)
    {
        var window = _config.RateLimiter.ClaimSchedulingWindow();

        var lastWindow = Volatile.Read(ref _lastKnownWindow);
        if (window > lastWindow)
        {
            var comparand = lastWindow;
            lastWindow = Interlocked.CompareExchange(ref _lastKnownWindow, window, comparand);

            while (window > lastWindow)
            {
                Thread.SpinWait(1);
                comparand = lastWindow;
                lastWindow = Interlocked.CompareExchange(ref _lastKnownWindow, window, comparand);
            }

        }

        var currentTimeTicks = DateTimeOffset.UtcNow.Ticks;

        if (window > currentTimeTicks)
            await Task.Delay(new TimeSpan(window - currentTimeTicks), cancellationToken);
    }

    public void Cancel()
    {
        _cts.Cancel();
    }

    public void Dispose()
    {
        _scope.Dispose();
        _cts.Dispose();
    }

    public sealed class Config
    {
        private readonly Dictionary<string, string> _baseQuery;

        public IEnumerable<KeyValuePair<string, string>> BaseQuery => _baseQuery;
        public Uri BaseUri { get; }
        public RateLimiter RateLimiter { get; }
        public CancellationToken StoppingToken { get; }

        public Config(Uri baseUri, Dictionary<string, string> baseQuery, RateLimiter rateLimiter, CancellationToken stoppingToken)
        {
            BaseUri = baseUri;
            _baseQuery = baseQuery;
            RateLimiter = rateLimiter;
            StoppingToken = stoppingToken;
        }
    }
}
