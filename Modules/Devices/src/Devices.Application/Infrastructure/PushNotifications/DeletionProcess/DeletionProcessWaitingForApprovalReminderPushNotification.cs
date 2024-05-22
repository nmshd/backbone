using Backbone.BuildingBlocks.Domain.PushNotifications;

namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;

public record DeletionProcessWaitingForApprovalReminderPushNotification(int DaysUntilApprovalPeriodEnds) : IPushNotification;
