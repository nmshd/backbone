using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
public class DirectPushService : IPushService
{
    private readonly IPnsRegistrationRepository _pnsRegistrationRepository;
    private readonly ILogger<DirectPushService> _logger;
    private readonly CancellationToken _cancellationToken;

    public DirectPushService(IPnsRegistrationRepository pnsRegistrationRepository, ILogger<DirectPushService> logger, CancellationToken cancellationToken)
    {
        _pnsRegistrationRepository = pnsRegistrationRepository;
        _logger = logger;
        _cancellationToken = cancellationToken;
    }

    public async Task SendNotification(IdentityAddress recipient, object notification)
    {
        var registrations = await _pnsRegistrationRepository.FindWithAddress(recipient, _cancellationToken, track: true);

        var groups = registrations.GroupBy(registration => registration.Handle.Platform);

        foreach(var group in groups)
        {
            var platform = group.Key;

            var pnsConnector = PnsConnectorFactory.CreateFor(platform);

            await pnsConnector.Send(group, notification);

            _logger.LogTrace($"Successfully sent push notifications to identity '{recipient}' on platform '{Enum.GetName(platform)}'");
        }
    }

    public async Task UpdateRegistration(IdentityAddress address, DeviceId deviceId, PnsHandle handle)
    {
        var registration = await _pnsRegistrationRepository.FindByDeviceId(deviceId, _cancellationToken, track: true);

        if (registration != null)
        {
            registration.Update(handle);

            await _pnsRegistrationRepository.Update(registration, _cancellationToken);

            _logger.LogTrace("Device successfully updated.");
        } else
        {
            await _pnsRegistrationRepository.Add(new PnsRegistration(address, deviceId, handle), _cancellationToken);

            _logger.LogTrace("New device successfully registered.");
        }
    }
}
