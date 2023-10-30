using System.Diagnostics;

namespace Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Hashing;

public static class DevicePushIdentifierHasher
{
    private static readonly ThreadLocal<Func<IDevicePushIdentifierHasher>> GET_HASHER = new(() => () => new DevicePushIdentifierHasherImpl());

    public static void SetHasher(IDevicePushIdentifierHasher hasher)
    {
        var stackTrace = new StackTrace();
        var callerType = stackTrace.GetFrame(1)!.GetMethod()!.DeclaringType;

        if (callerType is { Namespace: not null } && !callerType.Namespace.Contains("Test"))
        {
            throw new NotSupportedException("You can't call this method from a Non-Test-class");
        }

        GET_HASHER.Value = () => hasher;
    }

    public static string HashUtf8(string input)
    {
        return GET_HASHER.Value!().HashUtf8(input);
    }
}
