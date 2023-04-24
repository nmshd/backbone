using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Dummy;

public static class IServiceCollectionExtensions
{
    public static void AddDummyPushNotifications(this IServiceCollection services)
    {
        services.AddTransient<IPushService, DummyPushService>();
    }
}
