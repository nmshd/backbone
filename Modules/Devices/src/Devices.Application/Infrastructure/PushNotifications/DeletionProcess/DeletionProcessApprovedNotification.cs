using Backbone.BuildingBlocks.Domain.PushNotifications;

namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;

public record DeletionProcessApprovedNotification(int DaysUntilDeletion) : IPushNotification
{
    internal DeletionProcessApprovedNotification() : this(0) { }
}
