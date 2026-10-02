using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Abstractions.PushNotifications;

namespace Backbone.Modules.Devices.Module.Features.PushNotifications.Shared.Datawallet;

[NotificationId("DatawalletModified")]
public record DatawalletModificationsCreatedPushNotification(string CreatedByDevice) : IPushNotification;
