using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.ApplePushNotificationService;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.FirebaseCloudMessaging;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
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
}
