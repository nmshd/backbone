using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Dummy;

public static class IServiceCollectionExtensions
{
    public static void AddDummyPushNotifications(this IServiceCollection services)
    {
        services.AddTransient<IPushNotificationRegistrationService, DummyPushService>();
        services.AddTransient<IPushNotificationSender, DummyPushService>();
    }
}
