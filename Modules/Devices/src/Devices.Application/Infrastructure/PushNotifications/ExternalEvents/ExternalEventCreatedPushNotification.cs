using Backbone.BuildingBlocks.Application.PushNotifications;

namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.ExternalEvents;

[NotificationId("ExternalEventCreated")]
public record ExternalEventCreatedPushNotification : IPushNotification;
