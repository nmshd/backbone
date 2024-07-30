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

    public Task<SendResults> Send(IEnumerable<PnsRegistration> registrations, IPushNotification notification)
    {
        _logger.Sending(notification.GetEventName());

        var sendResults = new SendResults();
        foreach (var registration in registrations)
        {
            sendResults.AddSuccess(registration.DeviceId);
        }

        return Task.FromResult(sendResults);
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
