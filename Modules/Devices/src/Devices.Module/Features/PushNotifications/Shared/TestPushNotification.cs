using Backbone.BuildingBlocks.Application.PushNotifications;

namespace Backbone.Modules.Devices.Module.Features.PushNotifications.Shared;

public record TestPushNotification : IPushNotification
{
    public object? Data { get; set; }
}
