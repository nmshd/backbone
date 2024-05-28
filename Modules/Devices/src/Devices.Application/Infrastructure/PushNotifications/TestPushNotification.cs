using Backbone.BuildingBlocks.Domain.PushNotifications;

namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;

public record TestPushNotification : IPushNotification
{
    public object? Data { get; set; }
}
