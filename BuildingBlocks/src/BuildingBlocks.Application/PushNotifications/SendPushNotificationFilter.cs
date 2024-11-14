using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.BuildingBlocks.Application.PushNotifications;

public record SendPushNotificationFilter
{
    public List<DeviceId> ExcludedDevices { get; set; } = [];
    public List<IdentityAddress> IncludedIdentities { get; set; } = [];
}
