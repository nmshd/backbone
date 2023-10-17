using System.Diagnostics;
using Backbone.Tooling.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Logging;

public static class ILoggerExtensions
{
    public static async Task TraceTime(this ILogger logger, Func<Task> action, string? actionName = null)
    {
        if (!EnvironmentVariables.DEBUG_PERFORMANCE)
        {
            await action();
            return;
        }

        var watch = Stopwatch.StartNew();
        try
        {
            await action();
        }
        finally
        {
            watch.Stop();
            logger.LogDebug(LogEventIds.EXECUTION_TIME, "Executed '{action}' in {elapsedMilliseconds}ms.", actionName ?? "Action", watch.ElapsedMilliseconds);
        }
    }

    public static async Task<T> TraceTime<T>(this ILogger logger, Func<Task<T>> action, string? actionName = null)
    {
        if (!EnvironmentVariables.DEBUG_PERFORMANCE)
            return await action();

        var watch = Stopwatch.StartNew();
        try
        {
            return await action();
        }
        finally
        {
            watch.Stop();
            logger.LogDebug(LogEventIds.EXECUTION_TIME, "Executed '{action}' in {elapsedMilliseconds}ms.", actionName ?? "Action", watch.ElapsedMilliseconds);
        }
    }
}
