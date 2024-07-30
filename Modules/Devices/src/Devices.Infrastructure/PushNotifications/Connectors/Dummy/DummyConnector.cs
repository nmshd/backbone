using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Apns;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.NotificationTexts;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Responses;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Dummy;

public class DummyConnector : IPnsConnector
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IPushNotificationTextProvider _notificationTextProvider;
    private readonly ILogger<ApplePushNotificationServiceConnector> _logger;

    public DummyConnector(IIdentitiesRepository identitiesRepository, IPushNotificationTextProvider notificationTextProvider, ILogger<ApplePushNotificationServiceConnector> logger)
    {
        _identitiesRepository = identitiesRepository;
        _notificationTextProvider = notificationTextProvider;
        _logger = logger;
    }

    public async Task<SendResults> Send(IEnumerable<PnsRegistration> registrations, IdentityAddress recipient, IPushNotification notification)
    {
        var sendResults = new SendResults();

        var identity = await _identitiesRepository.FindByAddress(recipient, CancellationToken.None) ?? throw new Exception("Identity not found.");

        foreach (var device in identity.Devices)
        {
            var (title, body) = await _notificationTextProvider.GetNotificationTextForDeviceId(notification.GetType(), device.Id);
            _logger.LogInformation("Sending push notification to the device of the identity: {title}, {body}.", title, body);
        }

        return sendResults;
    }

    public void ValidateRegistration(PnsRegistration registration)
    {
    }
}
