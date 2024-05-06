namespace TeamCon2024.Core.Retries;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Net.Sockets;

using Microsoft.Extensions.Logging;

using Polly;
using Polly.Contrib.WaitAndRetry;

public sealed class PolicyFactory
{
    private static PolicyBuilder TransientPolicyBuilder
        => Policy
            .Handle<TimeoutException>()
            .Or<SocketException>()
            .Or<DbException>(ex => ex.IsTransient);

    public static IAsyncPolicy CreateTransientDbRetryAsyncPolicy(RetrySettings retrySettings, ILogger logger)
    {
        return TransientPolicyBuilder.WaitAndRetryAsync(
            GetSleepDurations(retrySettings),
            (ex, ts, attempt, _) => logger.LogWarning(ex, "The transient database error has occured. Next retry ({RetryAttempt}) in {RetryTimeSpan}.", attempt, ts));
    }

    private static IEnumerable<TimeSpan> GetSleepDurations(RetrySettings retrySettings)
    {
        return Backoff.DecorrelatedJitterBackoffV2(retrySettings.Delay, retrySettings.Times);
    }
}
