using Backbone.BuildingBlocks.Application.PushNotifications;

namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.Datawallet;

[NotificationId("DatawalletModified")]
public record DatawalletModificationsCreatedPushNotification(string CreatedByDevice) : IPushNotification;
