using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Abstractions.PushNotifications;

namespace Backbone.Modules.Devices.Module.Features.PushNotifications.Shared.ExternalEvents;

[NotificationId("ExternalEventCreated")]
public record ExternalEventCreatedPushNotification : IPushNotification;
