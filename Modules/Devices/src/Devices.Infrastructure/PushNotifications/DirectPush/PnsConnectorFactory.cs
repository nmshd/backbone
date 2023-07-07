using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.FirebaseCloudMessaging;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
public abstract class PnsConnectorFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PnsConnectorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IPnsConnector CreateFor(PushNotificationPlatform platform)
    {
        switch (platform)
        {
            case PushNotificationPlatform.Fcm:
                return _serviceProvider.GetRequiredService<FirebaseCloudMessagingConnector>();
            case PushNotificationPlatform.Apns:
                break;
        }
        throw new NotImplementedException($"There is currently no {nameof(IPnsConnector)} for the platform '{platform}'.");
    }
}
