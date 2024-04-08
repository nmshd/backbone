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
            logger.ExecutedAction(actionName ?? "Action", watch.ElapsedMilliseconds);
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
            logger.ExecutedAction(actionName ?? "Action", watch.ElapsedMilliseconds);
        }
    }
}

internal static partial class SaveChangesTimeInterceptorLogs
{
    [LoggerMessage(
        EventId = 293800,
        EventName = "ExecutionTime",
        Level = LogLevel.Information,
        Message = "Executed '{actionName}' in {elapsedMilliseconds}ms.")]
    public static partial void ExecutedAction(this ILogger logger, string actionName, long elapsedMilliseconds);
}
