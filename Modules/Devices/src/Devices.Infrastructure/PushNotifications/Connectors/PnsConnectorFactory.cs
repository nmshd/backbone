using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors;

public abstract class PnsConnectorFactory
{
    public IPnsConnector CreateFor(PushNotificationPlatform platform)
    {
        return platform switch
        {
            PushNotificationPlatform.Fcm => CreateForFirebaseCloudMessaging(),
            PushNotificationPlatform.Apns => CreateForApplePushNotificationService(),
            PushNotificationPlatform.Dummy => CreateForDummy(),
            PushNotificationPlatform.Sse => CreateForSse(),
            _ => throw new NotSupportedException($"There is currently no {nameof(IPnsConnector)} for the platform '{platform}'.")
        };
    }

    protected abstract IPnsConnector CreateForFirebaseCloudMessaging();

    protected abstract IPnsConnector CreateForApplePushNotificationService();

    protected abstract IPnsConnector CreateForDummy();

    protected abstract IPnsConnector CreateForSse();
}
