using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;

public class DirectPushService : IPushService
{
    private readonly IPnsRegistrationRepository _pnsRegistrationRepository;
    private readonly ILogger<DirectPushService> _logger;
    private readonly PnsConnectorFactory _pnsConnectorFactory;
    private readonly IPnsRegistrationRepository _registrationRepository;

    public DirectPushService(IPnsRegistrationRepository pnsRegistrationRepository, PnsConnectorFactory pnsConnectorFactory, ILogger<DirectPushService> logger, IPnsRegistrationRepository registrationRepository)
    {
        _pnsRegistrationRepository = pnsRegistrationRepository;
        _pnsConnectorFactory = pnsConnectorFactory;
        _logger = logger;
        _registrationRepository = registrationRepository;
    }

    public async Task SendNotification(IdentityAddress recipient, object notification, CancellationToken cancellationToken)
    {
        var registrations = await _pnsRegistrationRepository.FindWithAddress(recipient, cancellationToken);

        var groups = registrations.GroupBy(registration => registration.Handle.Platform);

        foreach (var group in groups)
        {
            var platform = group.Key;

            var pnsConnector = _pnsConnectorFactory.CreateFor(platform);

            var sendResults = await pnsConnector.Send(group, recipient, notification);
            await HandleNotificationResponses(sendResults);
        }
    }

    private async Task HandleNotificationResponses(SendResults sendResults)
    {
        var devicesIdsToDelete = new List<DeviceId>();
        foreach (var sendResult in sendResults.Failures)
        {
            switch (sendResult.Error.Reason)
            {
                case SendResult.ErrorReason.InvalidHandle:
                    _logger.LogInformation("Deleting device {deviceId} since handle is no longer valid.", sendResult.DeviceId);
                    devicesIdsToDelete.Add(sendResult.DeviceId);

                    break;
                case SendResult.ErrorReason.Unexpected:
                    _logger.LogError(
                        "The following error occurred while trying to send the notification for deviceId '{deviceId}': '{error}'",
                        sendResult.DeviceId, sendResult.Error.Message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Reason '{sendResult.Error.Reason}' not supported");
            }
        }

        await _registrationRepository.Delete(devicesIdsToDelete, CancellationToken.None);

        _logger.LogTrace("Successfully sent push notifications to devices '{devicesIds}'.", string.Join(", ", sendResults.Successes));
    }

    public async Task UpdateRegistration(IdentityAddress address, DeviceId deviceId, PnsHandle handle, string appId, CancellationToken cancellationToken)
    {
        var registration = await _pnsRegistrationRepository.FindByDeviceId(deviceId, cancellationToken, track: true);
        var pnsConnector = _pnsConnectorFactory.CreateFor(handle.Platform);

        if (registration != null)
        {
            registration.Update(handle, appId);
            pnsConnector.ValidateRegistration(registration);

            await _pnsRegistrationRepository.Update(registration, cancellationToken);

            _logger.LogTrace("Device successfully updated.");
        }
        else
        {
            registration = new PnsRegistration(address, deviceId, handle, appId);
            pnsConnector.ValidateRegistration(registration);

            await _pnsRegistrationRepository.Add(new PnsRegistration(address, deviceId, handle, appId), cancellationToken);

            _logger.LogTrace("New device successfully registered.");
        }
    }
}
