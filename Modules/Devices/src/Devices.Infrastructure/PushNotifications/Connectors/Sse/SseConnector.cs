using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Responses;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Sse;

public class SseConnector : IPnsConnector
{
    private readonly SseServerClient _sseServerClient;
    private readonly ILogger<SseConnector> _logger;

    public SseConnector(SseServerClient sseServerClient, ILogger<SseConnector> logger)
    {
        _sseServerClient = sseServerClient;
        _logger = logger;
    }

    public async Task<SendResults> Send(IEnumerable<PnsRegistration> registrations, IdentityAddress recipient, IPushNotification notification)
    {
        var sendResults = new SendResults();

        foreach (var registration in registrations)
        {
            try
            {
                await _sseServerClient.SendEvent(recipient, notification.GetEventName());
            }
            catch (SseClientNotRegisteredException)
            {
                sendResults.AddFailure(registration.DeviceId, ErrorReason.InvalidHandle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while sending the event.");
                sendResults.AddFailure(registration.DeviceId, ErrorReason.Unexpected);
            }

            sendResults.AddSuccess(registration.DeviceId);
        }

        return sendResults;
    }

    public void ValidateRegistration(PnsRegistration registration)
    {
    }
}
