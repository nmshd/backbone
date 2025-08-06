using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Responses;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors;

public interface IPnsConnector
{
    Task<SendResult> Send(PnsRegistration registration, IPushNotification notification, NotificationText notificationText);
    Task<SendResult> Send(PnsRegistration registration, NotificationText notificationText, string notificationId);
    void ValidateRegistration(PnsRegistration registration);
}
