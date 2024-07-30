using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Responses;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Sse;

public class ServerSentEventsConnector : IPnsConnector
{
    private readonly ISseServerClient _sseServerClient;
    private readonly ILogger<ServerSentEventsConnector> _logger;

    public ServerSentEventsConnector(ISseServerClient sseServerClient, ILogger<ServerSentEventsConnector> logger)
    {
        _sseServerClient = sseServerClient;
        _logger = logger;
    }

    public async Task<SendResults> Send(IEnumerable<PnsRegistration> registrations, IPushNotification notification)
    {
        var sendResults = new SendResults();

        foreach (var registration in registrations)
        {
            try
            {
                var eventName = notification.GetEventName();

                _logger.Sending(eventName, registration.IdentityAddress);

                await _sseServerClient.SendEvent(registration.IdentityAddress, eventName);
                sendResults.AddSuccess(registration.DeviceId);
            }
            catch (SseClientNotRegisteredException)
            {
                sendResults.AddFailure(registration.DeviceId, ErrorReason.InvalidHandle);
            }
            catch (Exception ex)
            {
                _logger.ErrorOnSend(ex);
                sendResults.AddFailure(registration.DeviceId, ErrorReason.Unexpected);
            }
        }

        return sendResults;
    }

    public void ValidateRegistration(PnsRegistration registration)
    {
        // There is nothing to validate here
    }
}

internal static partial class ServerSentEventsConnectorLogs
{
    [LoggerMessage(
        EventId = 433411,
        EventName = "ServerSentEventsConnector.Sending",
        Level = LogLevel.Debug,
        Message = "Sending push notification (type '{eventName}') to '{address}'.")]
    public static partial void Sending(this ILogger logger, string eventName, string address);

    [LoggerMessage(
        EventId = 707295,
        EventName = "ServerSentEventsConnector.ErrorOnSend",
        Level = LogLevel.Error,
        Message = "An unexpected error occurred while sending the event.")]
    public static partial void ErrorOnSend(this ILogger logger, Exception exception);
}
