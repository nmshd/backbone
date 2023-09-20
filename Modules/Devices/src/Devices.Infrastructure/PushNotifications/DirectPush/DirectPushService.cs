using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Enmeshed.BuildingBlocks.Infrastructure.Exceptions;
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
            await ParseNotificationResponses(sendResults);

            _logger.LogTrace($"Successfully sent push notifications to identity '{recipient}' on platform '{Enum.GetName(platform)}'");
        }
    }

    private async Task ParseNotificationResponses(IEnumerable<SendResult> sendResults)
    {
        foreach (var sendResult in sendResults.Where(sendResult => sendResult.IsFailure))
        {
            switch (sendResult.Error.Reason)
            {
                case SendResult.FailureReason.InvalidHandle:
                    _logger.LogInformation("Deleting device {deviceId} since handle is no longer valid.", sendResult.Error.DeviceId);
                    await _registrationRepository.Delete(sendResult.Error.DeviceId, CancellationToken.None);
                    break;
                case SendResult.FailureReason.Unexpected:
                    _logger.LogError(
                        "The following error occurred while trying to send the notification for deviceId '{deviceId}': '{error}'",
                        sendResult.Error.DeviceId, sendResult.Error.Message);
                    break;
            }
        }
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

            try
            {
                await _pnsRegistrationRepository.Add(new PnsRegistration(address, deviceId, handle, appId), cancellationToken);
                _logger.LogTrace("New device successfully registered.");
            }
            catch (InfrastructureException exception) when (exception.Code == InfrastructureErrors.UniqueKeyViolation().Code)
            {
                _logger.LogInformation(exception.Message);
            }
        }
    }
}
