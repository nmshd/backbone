using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Dummy;

public static class IServiceCollectionExtensions
{
    public static void AddDummyPushNotifications(this IServiceCollection services)
    {
        services.AddTransient<IPushService, DummyPushService>();
    }
}
