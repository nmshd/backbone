using Backbone.BuildingBlocks.Application.PushNotifications;

namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;

public record DeletionProcessApprovedNotification(int DaysUntilDeletion) : IPushNotification;
