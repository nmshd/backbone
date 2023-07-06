using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
public abstract class PnsConnectorFactory
{
    public IPnsConnector CreateFor(PushNotificationPlatform platform)
    {
        throw new NotImplementedException($"There is currently no {nameof(IPnsConnector)} for the platform '{platform}'.");
    }
}
