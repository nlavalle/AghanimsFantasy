namespace csharp_ef_webapi.Services;
internal class RateLimiter
{
    private static long _taskDelayTicker = 0;

    // internal static async Task WaitNextTaskScheduleAsync(long delayBetweenRequests)
    // {
    //     // Get the time
    //     long currentTimeTicks = DateTimeOffset.UtcNow.Ticks;

    //     // Get last scheduled time and set the next time to it
    //     // this will always be our most current known value in the scheduler
    //     long updatedSchedulerTicks = Volatile.Read(ref _taskDelayTicker);

    //     // This will always be our comparand
    //     long lastSchedulerTicks = updatedSchedulerTicks;

    //     // Calculate the next scheduler window: add the delay to the current scheduler value
    //     // This will be our exchange value after checking the current time
    //     long nextSchedulerTicks = updatedSchedulerTicks + delayBetweenRequests;

    //     // Special case: the current scheduler value is long in the past or is 0
    //     // If the expected value is less than or equal to the current time,
    //     // then the next window is more than half a second in the past, try to schedule now
    //     if (nextSchedulerTicks <= currentTimeTicks)
    //     {
    //         // Attempt to change the scheduler to the current time
    //         updatedSchedulerTicks = Interlocked.CompareExchange(ref _taskDelayTicker, currentTimeTicks, lastSchedulerTicks);

    //         // If that succeeded, we're done, let the task run
    //         if (updatedSchedulerTicks == lastSchedulerTicks)
    //             return;

    //         // Here we failed to acquire the next schedule because another thread got it first,
    //         // update to new value and add the delay to fit the optimized case below
    //         lastSchedulerTicks = updatedSchedulerTicks;
    //         nextSchedulerTicks = updatedSchedulerTicks + delayBetweenRequests;
    //     }

    //     // From here forward, we are delaying the task into the future

    //     // Optimized case: we already have the future value as part of the calculations above
    //     updatedSchedulerTicks = Interlocked.CompareExchange(ref _taskDelayTicker, nextSchedulerTicks, lastSchedulerTicks);

    //     while (updatedSchedulerTicks != lastSchedulerTicks)
    //     {
    //         // Common non-optimized case: We fight for the next value
    //         Thread.SpinWait(1);

    //         // Update to new value, add the delay
    //         lastSchedulerTicks = updatedSchedulerTicks;
    //         nextSchedulerTicks = updatedSchedulerTicks + delayBetweenRequests;

    //         updatedSchedulerTicks = Interlocked.CompareExchange(ref _taskDelayTicker, nextSchedulerTicks, lastSchedulerTicks);
    //     }

    //     // We're going to reacquire the current time as our last action to try to be as accurate as possible
    //     currentTimeTicks = DateTimeOffset.UtcNow.Ticks;

    //     // Going to check this here for the unlikely, but not impossible, case that we are scheduling right at a half second border
    //     if (nextSchedulerTicks > currentTimeTicks)
    //         await Task.Delay(new TimeSpan(nextSchedulerTicks - currentTimeTicks));
        
    //     return;
    // }

    // Non-static rate limiter, I would remove the guts of this class above here
    private long _localDelayTicker = 0;
    private readonly long _localDelayBetweenRequests;

    public RateLimiter(long delayBetweenRequests)
    {
        _localDelayBetweenRequests = delayBetweenRequests;
    }

    internal long ClaimSchedulingWindow()
    {
        long currentTimeTicks = DateTimeOffset.UtcNow.Ticks;

        // Get last scheduled time and set the next time to it
        // this will always be our most current known value in the scheduler
        long updatedSchedulerTicks = Volatile.Read(ref _localDelayTicker);

        // This will always be our comparand
        long lastSchedulerTicks = updatedSchedulerTicks;

        // Calculate the next scheduler window: add the delay to the current scheduler value
        // This will be our exchange value after checking the current time
        long nextSchedulerTicks = updatedSchedulerTicks + _localDelayBetweenRequests;

        // Special case: the current scheduler value is long in the past or is 0
        // If the expected value is less than or equal to the current time,
        // then the next window is more than half a second in the past, try to schedule now
        if (nextSchedulerTicks <= currentTimeTicks)
        {
            // Attempt to change the scheduler to the current time
            updatedSchedulerTicks = Interlocked.CompareExchange(ref _localDelayTicker, currentTimeTicks, lastSchedulerTicks);

            // If that succeeded, we're done, let the task run
            if (updatedSchedulerTicks == lastSchedulerTicks)
                return currentTimeTicks;

            // Here we failed to acquire the next schedule because another thread got it first,
            // update to new value and add the delay to fit the optimized case below
            lastSchedulerTicks = updatedSchedulerTicks;
            nextSchedulerTicks = updatedSchedulerTicks + _localDelayBetweenRequests;
        }

        // From here forward, we are delaying the task into the future

        // Optimized case: we already have the future value as part of the calculations above
        updatedSchedulerTicks = Interlocked.CompareExchange(ref _localDelayTicker, nextSchedulerTicks, lastSchedulerTicks);

        while (updatedSchedulerTicks != lastSchedulerTicks)
        {
            // Common non-optimized case: We fight for the next value
            Thread.SpinWait(1);

            // Update to new value, add the delay
            lastSchedulerTicks = updatedSchedulerTicks;
            nextSchedulerTicks = updatedSchedulerTicks + _localDelayBetweenRequests;

            updatedSchedulerTicks = Interlocked.CompareExchange(ref _taskDelayTicker, nextSchedulerTicks, lastSchedulerTicks);
        }

        return nextSchedulerTicks;
    }

    public async Task WaitNextTaskScheduleAsync(CancellationToken cancellationToken)
    {
        var window = ClaimSchedulingWindow();
        var currentTimeTicks = DateTimeOffset.UtcNow.Ticks;

        // Going to check this here for the unlikely, but not impossible, case that we are scheduling right at a half second border
        if (window > currentTimeTicks)
            await Task.Delay(new TimeSpan(window - currentTimeTicks));
    }

}
