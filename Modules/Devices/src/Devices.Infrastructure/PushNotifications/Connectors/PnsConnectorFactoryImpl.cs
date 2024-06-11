using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Apns;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Dummy;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Fcm;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Sse;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors;

public class PnsConnectorFactoryImpl : PnsConnectorFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PnsConnectorFactoryImpl(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override IPnsConnector CreateForFirebaseCloudMessaging()
    {
        return _serviceProvider.GetRequiredService<FirebaseCloudMessagingConnector>();
    }

    protected override IPnsConnector CreateForApplePushNotificationService()
    {
        return _serviceProvider.GetRequiredService<ApplePushNotificationServiceConnector>();
    }

    protected override IPnsConnector CreateForDummy()
    {
        return _serviceProvider.GetRequiredService<DummyConnector>();
    }

    protected override IPnsConnector CreateForSse()
    {
        return _serviceProvider.GetRequiredService<SseConnector>();
    }
}
