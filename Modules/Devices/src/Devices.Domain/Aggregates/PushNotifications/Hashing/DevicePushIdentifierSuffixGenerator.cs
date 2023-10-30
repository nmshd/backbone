using System.Diagnostics;

namespace Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Hashing;

public static class DevicePushIdentifierSuffixGenerator
{
    private static readonly ThreadLocal<Func<IDevicePushIdentifierSuffixGenerator>> GET_SUFFIX_GENERATOR = new(() => () => new DevicePushIdentifierSuffixGeneratorImpl());

    public static void SetHasher(IDevicePushIdentifierSuffixGenerator suffixGenerator)
    {
        var stackTrace = new StackTrace();
        var callerType = stackTrace.GetFrame(1)!.GetMethod()!.DeclaringType;

        if (callerType is { Namespace: not null } && !callerType.Namespace.Contains("Test"))
        {
            throw new NotSupportedException("You can't call this method from a Non-Test-class");
        }

        GET_SUFFIX_GENERATOR.Value = () => suffixGenerator;
    }

    public static string GenerateSuffixUtf8(string input)
    {
        return GET_SUFFIX_GENERATOR.Value!().GenerateSuffixUtf8(input);
    }
}
