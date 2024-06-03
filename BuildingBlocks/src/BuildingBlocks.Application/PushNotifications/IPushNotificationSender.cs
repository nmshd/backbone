using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.BuildingBlocks.Application.PushNotifications;

public interface IPushNotificationSender
{
    Task SendNotification(IdentityAddress recipient, IPushNotification notification, CancellationToken cancellationToken);
}

