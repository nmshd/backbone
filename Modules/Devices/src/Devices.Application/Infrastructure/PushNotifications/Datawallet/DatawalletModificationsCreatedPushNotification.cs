using Backbone.BuildingBlocks.Domain.PushNotifications;

namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.Datawallet;

public record DatawalletModificationsCreatedPushNotification(string CreatedByDevice) : IPushNotification
{
    internal DatawalletModificationsCreatedPushNotification() : this("") { }
}
