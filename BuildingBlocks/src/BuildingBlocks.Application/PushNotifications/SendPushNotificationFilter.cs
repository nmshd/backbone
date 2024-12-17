using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.BuildingBlocks.Application.PushNotifications;

public record SendPushNotificationFilter
{
    private SendPushNotificationFilter()
    {
    }

    public List<DeviceId> ExcludedDevices { get; set; } = [];
    public List<IdentityAddress> IncludedIdentities { get; set; } = [];

    public static SendPushNotificationFilter AllDevicesOf(params IdentityAddress[] recipientAddresses)
    {
        return new SendPushNotificationFilter
        {
            IncludedIdentities = [.. recipientAddresses]
        };
    }

    public static SendPushNotificationFilter AllDevicesOfExcept(IdentityAddress recipientAddress, params DeviceId[] deviceIds)
    {
        return new SendPushNotificationFilter
        {
            IncludedIdentities = [recipientAddress],
            ExcludedDevices = [.. deviceIds]
        };
    }

    public static SendPushNotificationFilter AllDevicesOfAllIdentities()
    {
        return new SendPushNotificationFilter
        {
            IncludedIdentities = [],
            ExcludedDevices = []
        };
    }
}
