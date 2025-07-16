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

    // The `notificationText` parameter is not used in this implementation, so we make it optional. This simplifies the tests.
    public async Task<SendResult> Send(PnsRegistration registration, IPushNotification notification, NotificationText? notificationText = null)
    {
        try
        {
            var eventName = notification.GetEventName();

            _logger.Sending(eventName);

            await _sseServerClient.SendEvent(registration.IdentityAddress, eventName);
            return SendResult.Success(registration.DeviceId);
        }
        catch (SseClientNotRegisteredException)
        {
            return SendResult.Failure(registration.DeviceId, ErrorReason.InvalidHandle);
        }
        catch (Exception ex)
        {
            _logger.ErrorOnSend(ex);
            return SendResult.Failure(registration.DeviceId, ErrorReason.Unexpected);
        }
    }

    public Task<SendResult> SendTextOnly(PnsRegistration registration, NotificationText notificationText, string notificationId)
    {
        // we currently don't want to send text-only notifications via SSE
        return Task.FromResult(SendResult.Success(registration.DeviceId));
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
        Message = "Sending push notification (type '{eventName}').")]
    public static partial void Sending(this ILogger logger, string eventName);

    [LoggerMessage(
        EventId = 707295,
        EventName = "ServerSentEventsConnector.ErrorOnSend",
        Level = LogLevel.Error,
        Message = "An unexpected error occurred while sending the event.")]
    public static partial void ErrorOnSend(this ILogger logger, Exception exception);
}
