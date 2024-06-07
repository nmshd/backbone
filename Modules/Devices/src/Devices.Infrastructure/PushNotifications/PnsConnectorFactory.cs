using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications;

public abstract class PnsConnectorFactory
{
    public IPnsConnector CreateFor(PushNotificationPlatform platform)
    {
        switch (platform)
        {
            case PushNotificationPlatform.Fcm:
                return CreateForFirebaseCloudMessaging();
            case PushNotificationPlatform.Apns:
                return CreateForApplePushNotificationService();
            case PushNotificationPlatform.Dummy:
                return CreateForDummy();
            case PushNotificationPlatform.Sse:
                return CreateForSse();
        }

        throw new NotImplementedException($"There is currently no {nameof(IPnsConnector)} for the platform '{platform}'.");
    }

    protected abstract IPnsConnector CreateForFirebaseCloudMessaging();

    protected abstract IPnsConnector CreateForApplePushNotificationService();

    protected abstract IPnsConnector CreateForDummy();

    protected abstract IPnsConnector CreateForSse();
}
