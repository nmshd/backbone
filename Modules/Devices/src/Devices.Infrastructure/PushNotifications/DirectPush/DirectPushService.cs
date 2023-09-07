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

    public DirectPushService(IPnsRegistrationRepository pnsRegistrationRepository, PnsConnectorFactory pnsConnectorFactory, ILogger<DirectPushService> logger)
    {
        _pnsRegistrationRepository = pnsRegistrationRepository;
        _pnsConnectorFactory = pnsConnectorFactory;
        _logger = logger;
    }

    public async Task SendNotification(IdentityAddress recipient, object notification, CancellationToken cancellationToken)
    {
        var registrations = await _pnsRegistrationRepository.FindWithAddress(recipient, cancellationToken);

        var groups = registrations.GroupBy(registration => registration.Handle.Platform);

        foreach (var group in groups)
        {
            var platform = group.Key;

            var pnsConnector = _pnsConnectorFactory.CreateFor(platform);

            await pnsConnector.Send(group, recipient, notification);

            _logger.LogTrace($"Successfully sent push notifications to identity '{recipient}' on platform '{Enum.GetName(platform)}'");
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

            await _pnsRegistrationRepository.Add(new PnsRegistration(address, deviceId, handle, appId), cancellationToken);

            _logger.LogTrace("New device successfully registered.");
        }
    }
}
