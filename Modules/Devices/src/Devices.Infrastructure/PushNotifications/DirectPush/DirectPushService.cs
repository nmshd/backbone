﻿using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.BuildingBlocks.Infrastructure.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.Responses;
using Microsoft.Extensions.Logging;
using Environment = Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Environment;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;

public class DirectPushService : IPushNotificationRegistrationService, IPushNotificationSender
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
        var deviceIdsToDelete = new List<DeviceId>();
        foreach (var sendResult in sendResults.Failures)
        {
            switch (sendResult.Error.Reason)
            {
                case ErrorReason.InvalidHandle:
                    _logger.DeletingDeviceRegistration(sendResult.DeviceId);
                    deviceIdsToDelete.Add(sendResult.DeviceId);

                    break;
                case ErrorReason.Unexpected:
                    _logger.ErrorWhileTryingToSendNotification(sendResult.DeviceId, sendResult.Error.Message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Reason '{sendResult.Error.Reason}' not supported");
            }
        }

        await _registrationRepository.Delete(deviceIdsToDelete, CancellationToken.None);

        _logger.LogTrace("Successfully sent push notifications to '{devicesIds}'.", string.Join(", ", sendResults.Successes));
    }

    public async Task UpdateRegistration(IdentityAddress address, DeviceId deviceId, PnsHandle handle, string appId, Environment environment, CancellationToken cancellationToken)
    {
        var registration = await _pnsRegistrationRepository.FindByDeviceId(deviceId, cancellationToken, track: true);
        var pnsConnector = _pnsConnectorFactory.CreateFor(handle.Platform);

        if (registration != null)
        {
            registration.Update(handle, appId, environment);
            pnsConnector.ValidateRegistration(registration);

            await _pnsRegistrationRepository.Update(registration, cancellationToken);

            _logger.LogTrace("Device successfully updated.");
        }
        else
        {
            registration = new PnsRegistration(address, deviceId, handle, appId, environment);
            pnsConnector.ValidateRegistration(registration);

            try
            {
                await _pnsRegistrationRepository.Add(registration, cancellationToken);
                _logger.LogTrace("New device successfully registered.");
            }
            catch (InfrastructureException exception) when (exception.Code == InfrastructureErrors.UniqueKeyViolation().Code)
            {
                // This exception can be ignored. It is only thrown in case of a concurrent registration request from multiple devices.
                _logger.LogInformation(exception.Message);
            }
        }
    }

    public async Task DeleteRegistration(DeviceId deviceId, CancellationToken cancellationToken)
    {
        var registration = await _pnsRegistrationRepository.FindByDeviceId(deviceId, cancellationToken, track: true);

        if (registration == null)
        {
            _logger.LogInformation("Device '{deviceId}' is not found.", deviceId);
        }
        else
        {
            await _pnsRegistrationRepository.Delete(new List<DeviceId> { deviceId }, cancellationToken);
            _logger.UnregisteredDevice(deviceId);
        }
    }
}

internal static partial class DirectPushServiceLogs
{
    [LoggerMessage(
        EventId = 950845,
        EventName = "Devices.DirectPushService.DeletingDeviceRegistration",
        Level = LogLevel.Information,
        Message = "Deleting device registration for '{deviceId}' since handle is no longer valid.")]
    public static partial void DeletingDeviceRegistration(this ILogger logger, DeviceId deviceId);

    [LoggerMessage(
        EventId = 624412,
        EventName = "Devices.DirectPushService.ErrorWhileTryingToSendNotification",
        Level = LogLevel.Error,
        Message = "The following error occurred while trying to send the notification for '{deviceId}': '{error}'.")]
    public static partial void ErrorWhileTryingToSendNotification(this ILogger logger, DeviceId deviceId, string error);

    [LoggerMessage(
        EventId = 628738,
        EventName = "Devices.DirectPushService.UnregisteredDevice",
        Level = LogLevel.Information,
        Message = "Unregistered device '{deviceId} from push notifications.")]
    public static partial void UnregisteredDevice(this ILogger logger, DeviceId deviceId);
}
