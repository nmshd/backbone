using Backbone.BuildingBlocks.Application.PushNotifications;

namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.Datawallet;

public record DatawalletModificationsCreatedPushNotification(string CreatedByDevice) : IPushNotification;
