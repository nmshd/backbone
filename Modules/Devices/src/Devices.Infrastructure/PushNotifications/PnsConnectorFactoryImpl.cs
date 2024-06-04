using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Apns;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Dummy;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Fcm;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications;

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
}
