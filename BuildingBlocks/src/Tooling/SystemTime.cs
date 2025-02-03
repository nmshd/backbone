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
    private static readonly ThreadLocal<Stack<Func<DateTime>>> HISTORY = new(() => new Stack<Func<DateTime>>());
    private static readonly ThreadLocal<Func<DateTime>> GET_TIME = new(() => () => DateTime.UtcNow);

    /// <inheritdoc cref="DateTime.Today"/>
    public static DateTime UtcToday => GET_TIME.Value == null
        ? throw new Exception("Time function is null")
        : GET_TIME.Value().ToUniversalTime().Date;

    /// <inheritdoc cref="DateTime.UtcNow"/>
    public static DateTime UtcNow => GET_TIME.Value == null
        ? throw new Exception("Time function is null")
        : GET_TIME.Value().ToUniversalTime();

    public static void Set(string dateTimeString)
    {
        EnsureIsCalledFromTest();
        SetInternal(DateTime.Parse(dateTimeString));
    }

    /// <summary>
    /// Sets a fixed (deterministic) time for the current thread to return by <see cref="SystemTime"/>.
    /// </summary>
    public static void Set(DateTime dateTime)
    {
        EnsureIsCalledFromTest();
        SetInternal(dateTime);
    }

    private static void EnsureIsCalledFromTest()
    {
        var stackTrace = new StackTrace();
        var callerType = stackTrace.GetFrame(2)!.GetMethod()!.DeclaringType;

        if (callerType is { Namespace: not null } && !callerType.Namespace.Contains("Test") && !callerType.FullName!.Contains("Tests"))
        {
            throw new NotSupportedException("You can't call this method from a Non-Test-class");
        }
    }

    private static void SetInternal(DateTime dateTime)
    {
        if (dateTime.Kind != DateTimeKind.Local)
            dateTime = dateTime.ToLocalTime();

        HISTORY.Value!.Push(GET_TIME.Value!);

        GET_TIME.Value = () => dateTime;
    }

    public static void UndoSet()
    {
        if (HISTORY.Value!.Count == 0)
            GET_TIME.Value = () => DateTime.UtcNow;

        GET_TIME.Value = HISTORY.Value!.Pop();
    }

    /// <summary>
    /// Resets <see cref="SystemTime"/> to return the current <see cref="DateTime.UtcNow"/>.
    /// </summary>
    public static void Reset()
    {
        GET_TIME.Value = () => DateTime.UtcNow;
    }
}
