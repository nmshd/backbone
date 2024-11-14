using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.BuildingBlocks.Application.PushNotifications;

public record SendPushNotificationFilter
{
    private SendPushNotificationFilter()
    {
    }

    public List<DeviceId> ExcludedDevices { get; set; } = [];
    public List<IdentityAddress> IncludedIdentities { get; set; } = [];

    public static SendPushNotificationFilter AllDevicesOf(IdentityAddress address)
    {
        return new SendPushNotificationFilter
        {
            IncludedIdentities = [address]
        };
    }

    public static SendPushNotificationFilter AllDevicesOfExcept(IdentityAddress address, params DeviceId[] deviceIds)
    {
        return new SendPushNotificationFilter
        {
            IncludedIdentities = [address],
            ExcludedDevices = [.. deviceIds]
        };
    }
}
