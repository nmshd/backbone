using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Responses;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Dummy;

public class DummyConnector : IPnsConnector
{
    private readonly ILogger<DummyConnector> _logger;

    public DummyConnector(ILogger<DummyConnector> logger)
    {
        _logger = logger;
    }

    public Task<SendResult> Send(PnsRegistration registration, IPushNotification notification, NotificationText notificationText)
    {
        _logger.Sending(notification.GetEventName());

        return Task.FromResult(SendResult.Success(registration.DeviceId));
    }

    public Task<SendResult> Send(PnsRegistration registration, NotificationText notificationText, string notificationId)
    {
        _logger.Sending("no-content");

        return Task.FromResult(SendResult.Success(registration.DeviceId));
    }

    public void ValidateRegistration(PnsRegistration registration)
    {
    }
}

internal static partial class DummyConnectorLogs
{
    [LoggerMessage(
        EventId = 860591,
        EventName = "DummyConnectorLogs.Sending",
        Level = LogLevel.Debug,
        Message = "Sending push notification (type '{eventName}').")]
    public static partial void Sending(this ILogger logger, string eventName);
}
