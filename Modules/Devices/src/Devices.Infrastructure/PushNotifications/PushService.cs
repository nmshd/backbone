using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.BuildingBlocks.Infrastructure.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.NotificationTexts;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Responses;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications;

public class PushService : IPushNotificationRegistrationService, IPushNotificationSender
{
    private readonly IPnsRegistrationsRepository _pnsRegistrationsRepository;
    private readonly ILogger<PushService> _logger;
    private readonly PnsConnectorFactory _pnsConnectorFactory;
    private readonly IPushNotificationTextProvider _notificationTextProvider;
    private readonly IIdentitiesRepository _identitiesRepository;

    public PushService(IPnsRegistrationsRepository pnsRegistrationsRepository, PnsConnectorFactory pnsConnectorFactory, ILogger<PushService> logger,
        IPushNotificationTextProvider notificationTextProvider, IIdentitiesRepository identitiesRepository)
    {
        _pnsRegistrationsRepository = pnsRegistrationsRepository;
        _pnsConnectorFactory = pnsConnectorFactory;
        _logger = logger;
        _notificationTextProvider = notificationTextProvider;
        _identitiesRepository = identitiesRepository;
    }

    public async Task SendNotification(IPushNotification notification, SendPushNotificationFilter filter, CancellationToken cancellationToken)
    {
        var devices = await ListDevices(filter, cancellationToken);
        var distinctCommunicationLanguages = GetDistinctCommunicationLanguages(devices);
        var notificationTexts = _notificationTextProvider.GetNotificationTextsForLanguages(notification.GetType(), distinctCommunicationLanguages);

        await SendNotificationInternal(notification, devices, notificationTexts, cancellationToken);
    }

    private static List<CommunicationLanguage> GetDistinctCommunicationLanguages(IEnumerable<DeviceWithOnlyIdAndCommunicationLanguage> devices)
    {
        return devices.Select(d => d.CommunicationLanguage).Distinct().ToList();
    }

    public async Task SendNotification(IPushNotification notification, SendPushNotificationFilter filter, Dictionary<string, NotificationText> notificationTexts, CancellationToken cancellationToken)
    {
        var devices = await ListDevices(filter, cancellationToken);
        var mappedNotificationTexts = notificationTexts.ToDictionary(kvp => CommunicationLanguage.Create(kvp.Key).Value, kvp => kvp.Value);

        await SendNotificationInternal(notification, devices, mappedNotificationTexts, cancellationToken);
    }

    private async Task<DeviceWithOnlyIdAndCommunicationLanguage[]> ListDevices(SendPushNotificationFilter filter, CancellationToken cancellationToken)
    {
        var result = await _identitiesRepository.ListDevices(
            d => (filter.IncludedIdentities.Count == 0 || filter.IncludedIdentities.Contains(d.IdentityAddress)) &&
                 !filter.ExcludedDevices.Contains(d.Id),
            d => new DeviceWithOnlyIdAndCommunicationLanguage { Id = d.Id, CommunicationLanguage = d.CommunicationLanguage },
            cancellationToken
        );
        return result;
    }

    private async Task SendNotificationInternal(IPushNotification notification, DeviceWithOnlyIdAndCommunicationLanguage[] devices,
        Dictionary<CommunicationLanguage, NotificationText> notificationTexts, CancellationToken cancellationToken)
    {
        var deviceIds = devices.Select(d => d.Id).ToArray();

        var groups = await GetDeviceRegsitrationsGroupedByPlatform(deviceIds, cancellationToken);

        foreach (var group in groups)
        {
            var platform = group.Key;

            var pnsConnector = _pnsConnectorFactory.CreateFor(platform);

            var sendTasks = group
                .Select(r =>
                {
                    var device = devices.First(d => d.Id == r.DeviceId);

                    if (!notificationTexts.TryGetValue(device.CommunicationLanguage, out var notificationText))
                        notificationText = notificationTexts[CommunicationLanguage.DEFAULT_LANGUAGE];

                    return pnsConnector.Send(r, notification, notificationText);
                });

            var sendResults = await Task.WhenAll(sendTasks);
            await HandleSendNotificationResponses(new SendResults(sendResults));
        }
    }

    private async Task<IEnumerable<IGrouping<PushNotificationPlatform, PnsRegistration>>> GetDeviceRegsitrationsGroupedByPlatform(DeviceId[] deviceIds, CancellationToken cancellationToken)
    {
        var registrations = await _pnsRegistrationsRepository.List(deviceIds, cancellationToken);

        var groups = registrations
            .DistinctBy(r => r.Handle) // Since there can be multiple registrations with the same handle, we should make sure we send the same push notification only once per handle
            .GroupBy(r => r.Handle.Platform);

        return groups;
    }

    public async Task SendNotification(Dictionary<string, NotificationText> notificationTexts, string notificationId, SendPushNotificationFilter filter, CancellationToken cancellationToken)
    {
        var devices = await ListDevices(filter, cancellationToken);
        var deviceIds = devices.Select(d => d.Id).ToArray();

        var groups = await GetDeviceRegsitrationsGroupedByPlatform(deviceIds, cancellationToken);

        foreach (var group in groups)
        {
            var platform = group.Key;

            var pnsConnector = _pnsConnectorFactory.CreateFor(platform);

            var sendTasks = group
                .Select(r =>
                {
                    var device = devices.First(d => d.Id == r.DeviceId);

                    if (!notificationTexts.TryGetValue(device.CommunicationLanguage, out var notificationText))
                        notificationText = notificationTexts[CommunicationLanguage.DEFAULT_LANGUAGE];

                    return pnsConnector.Send(r, notificationText, notificationId);
                });

            var sendResults = await Task.WhenAll(sendTasks);
            await HandleSendNotificationResponses(new SendResults(sendResults));
        }
    }

    private async Task HandleSendNotificationResponses(SendResults sendResults)
    {
        var deviceIdsToDelete = new List<DeviceId>();
        foreach (var sendResult in sendResults.Failures)
        {
            switch (sendResult.Error!.Reason)
            {
                case ErrorReason.InvalidHandle:
                    _logger.DeletingDeviceRegistration();
                    deviceIdsToDelete.Add(sendResult.DeviceId);
                    break;
                case ErrorReason.Unexpected:
                    _logger.ErrorWhileTryingToSendNotification(sendResult.Error.Message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Reason '{sendResult.Error.Reason}' not supported");
            }
        }

        await _pnsRegistrationsRepository.Delete(deviceIdsToDelete, CancellationToken.None);

        _logger.LogTrace("Successfully sent push notifications.");
    }

    public async Task<DevicePushIdentifier> UpdateRegistration(IdentityAddress address, DeviceId deviceId, PnsHandle handle, string appId, PushEnvironment environment,
        CancellationToken cancellationToken)
    {
        var registration = await _pnsRegistrationsRepository.Get(deviceId, cancellationToken, track: true);
        var pnsConnector = _pnsConnectorFactory.CreateFor(handle.Platform);

        if (registration != null)
        {
            registration.Update(handle, appId, environment);
            pnsConnector.ValidateRegistration(registration);

            await _pnsRegistrationsRepository.Update(registration, cancellationToken);

            _logger.LogTrace("Device successfully updated.");
        }
        else
        {
            registration = new PnsRegistration(address, deviceId, handle, appId, environment);
            pnsConnector.ValidateRegistration(registration);

            try
            {
                await _pnsRegistrationsRepository.Add(registration, cancellationToken);
                _logger.LogTrace("New device successfully registered.");
            }
            catch (InfrastructureException exception) when (exception.Code == InfrastructureErrors.UniqueKeyViolation().Code)
            {
                _logger.LogInformation(exception, "This exception can be ignored. It is only thrown in case of a concurrent registration request from multiple devices.");
            }
        }

        return registration.DevicePushIdentifier;
    }

    public async Task DeleteRegistration(DeviceId deviceId, CancellationToken cancellationToken)
    {
        var numberOfDeletedDevices = await _pnsRegistrationsRepository.Delete([deviceId], cancellationToken);

        if (numberOfDeletedDevices == 1)
            _logger.UnregisteredDevice();
    }

    private class DeviceWithOnlyIdAndCommunicationLanguage
    {
        public required DeviceId Id { get; init; }
        public required CommunicationLanguage CommunicationLanguage { get; init; }
    }
}

internal static partial class DirectPushServiceLogs
{
    [LoggerMessage(
        EventId = 950845,
        EventName = "Devices.DirectPushService.DeletingDeviceRegistration",
        Level = LogLevel.Information,
        Message = "Deleting device registration for the device since handle is no longer valid.")]
    public static partial void DeletingDeviceRegistration(this ILogger logger);

    [LoggerMessage(
        EventId = 624412,
        EventName = "Devices.DirectPushService.ErrorWhileTryingToSendNotification",
        Level = LogLevel.Error,
        Message = "The following error occurred while trying to send the notification for the device: '{error}'.")]
    public static partial void ErrorWhileTryingToSendNotification(this ILogger logger, string error);

    [LoggerMessage(
        EventId = 628738,
        EventName = "Devices.DirectPushService.UnregisteredDevice",
        Level = LogLevel.Information,
        Message = "Unregistered the device from push notifications.")]
    public static partial void UnregisteredDevice(this ILogger logger);
}
