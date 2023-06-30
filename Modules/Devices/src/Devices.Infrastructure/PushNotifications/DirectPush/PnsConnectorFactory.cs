using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
public abstract class PnsConnectorFactory
{
    public static IPnsConnector CreateFor(PushNotificationPlatform platform)
    {
        throw new NotImplementedException("PnsConnectorFactory and PnsConnectorFactoryImpl are not yet implemented.");
    }
}
