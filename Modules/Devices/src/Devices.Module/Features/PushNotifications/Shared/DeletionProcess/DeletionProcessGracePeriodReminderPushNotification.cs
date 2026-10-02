using Backbone.BuildingBlocks.Application.PushNotifications;

namespace Backbone.Modules.Devices.Module.Features.PushNotifications.Shared.DeletionProcess;

public record DeletionProcessGracePeriodReminderPushNotification(int DaysUntilDeletion) : IPushNotification;
