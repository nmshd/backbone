using Backbone.BuildingBlocks.Domain.PushNotifications;

namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;

public record DeletionProcessGracePeriodReminderPushNotification(int DaysUntilDeletion) : IPushNotification;
