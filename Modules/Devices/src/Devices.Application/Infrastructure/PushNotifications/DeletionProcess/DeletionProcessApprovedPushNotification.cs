using Backbone.BuildingBlocks.Application.PushNotifications;

namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;

public record DeletionProcessApprovedPushNotification(int DaysUntilDeletion) : IPushNotification;
