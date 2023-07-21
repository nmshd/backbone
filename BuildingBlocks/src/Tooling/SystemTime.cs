using System.Diagnostics;

namespace Enmeshed.Tooling;

/// <summary>
/// Provides access to system time while allowing it to be set to a fixed <see cref="DateTime"/> value.
/// </summary>
/// <remarks>
/// </remarks>
public static class SystemTime
{
    private static Func<DateTime> _getTime = () => DateTime.UtcNow;

    /// <inheritdoc cref="DateTime.Today"/>
    public static DateTime UtcToday => _getTime == null
        ? throw new Exception("Time function is null")
        : _getTime().ToUniversalTime().Date;

    /// <inheritdoc cref="DateTime.UtcNow"/>
    public static DateTime UtcNow => _getTime == null
        ? throw new Exception("Time function is null")
        : _getTime().ToUniversalTime();

    /// <summary>
    /// Sets a fixed (deterministic) time for the current thread to return by <see cref="SystemTime"/>.
    /// </summary>
    public static void Set(DateTime time)
    {
        var stackTrace = new StackTrace();
        var callerType = stackTrace.GetFrame(1)!.GetMethod()!.DeclaringType;

        if (callerType != null && callerType.Namespace != null && !callerType.Namespace.Contains("Test"))
        {
            throw new NotSupportedException("You can't call this method from a Non-Test-class");
        }

        if (time.Kind != DateTimeKind.Local)
            time = time.ToLocalTime();

        _getTime = () => time;
    }

    /// <summary>
    /// Resets <see cref="SystemTime"/> to return the current <see cref="DateTime.Now"/>.
    /// </summary>
    public static void Reset()
    {
        _getTime = () => DateTime.UtcNow;
    }
}
