using System.Diagnostics;

namespace Backbone.Tooling;

/// <summary>
/// Provides access to system time while allowing it to be set to a fixed <see cref="DateTime"/> value.
/// </summary>
/// <remarks>
/// This class is thread safe.
/// </remarks>
public static class SystemTime
{
    private static readonly ThreadLocal<Func<DateTime>> GET_TIME = new(() => () => DateTime.Now);

    /// <inheritdoc cref="DateTime.Today"/>
    public static DateTime UtcToday => GET_TIME.Value == null
        ? throw new Exception("Time function is null")
        : GET_TIME.Value().ToUniversalTime().Date;

    /// <inheritdoc cref="DateTime.UtcNow"/>
    public static DateTime UtcNow => GET_TIME.Value == null
        ? throw new Exception("Time function is null")
        : GET_TIME.Value().ToUniversalTime();

    /// <summary>
    /// Sets a fixed (deterministic) time for the current thread to return by <see cref="SystemTime"/>.
    /// </summary>
    public static void Set(DateTime time)
    {
        var stackTrace = new StackTrace();
        var callerType = stackTrace.GetFrame(1)!.GetMethod()!.DeclaringType;

        if (callerType is { Namespace: not null } && !callerType.Namespace.Contains("Test"))
        {
            throw new NotSupportedException("You can't call this method from a Non-Test-class");
        }

        if (time.Kind != DateTimeKind.Local)
            time = time.ToLocalTime();

        GET_TIME.Value = () => time;
    }

    /// <summary>
    /// Resets <see cref="SystemTime"/> to return the current <see cref="DateTime.Now"/>.
    /// </summary>
    public static void Reset()
    {
        GET_TIME.Value = () => DateTime.Now;
    }
}
