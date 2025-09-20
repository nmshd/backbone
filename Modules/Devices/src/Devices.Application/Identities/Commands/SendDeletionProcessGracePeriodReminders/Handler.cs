using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Application.Identities.Commands.SendDeletionProcessGracePeriodReminders;

public class Handler : IRequestHandler<SendDeletionProcessGracePeriodRemindersCommand>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IPushNotificationSender _pushSender;
    private readonly ILogger<Handler> _logger;

    public Handler(IIdentitiesRepository identitiesRepository, IPushNotificationSender pushSender, ILogger<Handler> logger)
    {
        _identitiesRepository = identitiesRepository;
        _pushSender = pushSender;
        _logger = logger;
    }

    public async Task Handle(SendDeletionProcessGracePeriodRemindersCommand request, CancellationToken cancellationToken)
    {
        var identities = await _identitiesRepository.ListWithDeletionProcessInStatus(DeletionProcessStatus.Active, cancellationToken, track: true);

        _logger.LogTrace("Processing identities with deletion process in status 'Active' ...");

        foreach (var identity in identities)
        {
            var deletionProcess = identity.GetDeletionProcessInStatus(DeletionProcessStatus.Active) ?? throw new NotFoundException(nameof(IdentityDeletionProcess));
            var daysUntilDeletion = (deletionProcess.GracePeriodEndsAt!.Value - SystemTime.UtcNow).TotalDays;

            if (deletionProcess.GracePeriodReminder3SentAt != null)
            {
                _logger.NoReminderSent();
                continue;
            }

            if (daysUntilDeletion <= IdentityDeletionConfiguration.Instance.GracePeriodNotification3.DaysBeforeEndOfGracePeriod)
            {
                await SendReminder3(identity, daysUntilDeletion, deletionProcess.Id, cancellationToken);
                continue;
            }

            if (deletionProcess.GracePeriodReminder2SentAt != null)
                continue;

            if (daysUntilDeletion <= IdentityDeletionConfiguration.Instance.GracePeriodNotification2.DaysBeforeEndOfGracePeriod)
            {
                await SendReminder2(identity, daysUntilDeletion, deletionProcess.Id, cancellationToken);
                continue;
            }

            if (deletionProcess.GracePeriodReminder1SentAt != null)
                continue;

            if (daysUntilDeletion <= IdentityDeletionConfiguration.Instance.GracePeriodNotification1.DaysBeforeEndOfGracePeriod)
            {
                await SendReminder1(identity, daysUntilDeletion, deletionProcess.Id, cancellationToken);
            }
        }
    }

    private async Task SendReminder3(Identity identity, double daysUntilGracePeriodEnds, IdentityDeletionProcessId deletionProcessId, CancellationToken cancellationToken)
    {
        await _pushSender.SendNotification(
            new DeletionProcessGracePeriodReminderPushNotification((int)Math.Ceiling(daysUntilGracePeriodEnds)),
            SendPushNotificationFilter.AllDevicesOf(identity.Address),
            cancellationToken
        );
        identity.DeletionGracePeriodReminder3Sent();
        await _identitiesRepository.Update(identity, cancellationToken);
        _logger.Reminder3Sent(deletionProcessId);
    }

    private async Task SendReminder2(Identity identity, double daysUntilGracePeriodEnds, IdentityDeletionProcessId deletionProcessId, CancellationToken cancellationToken)
    {
        await _pushSender.SendNotification(
            new DeletionProcessGracePeriodReminderPushNotification((int)Math.Ceiling(daysUntilGracePeriodEnds)),
            SendPushNotificationFilter.AllDevicesOf(identity.Address),
            cancellationToken
        );
        identity.DeletionGracePeriodReminder2Sent();
        await _identitiesRepository.Update(identity, cancellationToken);
        _logger.Reminder2Sent(deletionProcessId);
    }

    private async Task SendReminder1(Identity identity, double daysUntilGracePeriodEnds, IdentityDeletionProcessId deletionProcessId, CancellationToken cancellationToken)
    {
        await _pushSender.SendNotification(
            new DeletionProcessGracePeriodReminderPushNotification((int)Math.Ceiling(daysUntilGracePeriodEnds)),
            SendPushNotificationFilter.AllDevicesOf(identity.Address),
            cancellationToken
        );
        identity.DeletionGracePeriodReminder1Sent();
        await _identitiesRepository.Update(identity, cancellationToken);
        _logger.Reminder1Sent(deletionProcessId);
    }
}

internal static partial class SendDeletionProcessGracePeriodRemindersLogs
{
    [LoggerMessage(
        EventId = 659877,
        EventName = "Devices.SendDeletionProcessGracePeriodReminders.NoReminderSent",
        Level = LogLevel.Information,
        Message = "No Grace period reminder sent.")]
    public static partial void NoReminderSent(this ILogger logger);

    [LoggerMessage(
        EventId = 343043,
        EventName = "Devices.SendDeletionProcessGracePeriodReminders.Reminder1Sent",
        Level = LogLevel.Information,
        Message = "Grace period reminder 1 sent for deletion process '{deletionProcessId}'.")]
    public static partial void Reminder1Sent(this ILogger logger, IdentityDeletionProcessId deletionProcessId);

    [LoggerMessage(
        EventId = 153402,
        EventName = "Devices.SendDeletionProcessGracePeriodReminders.Reminder2Sent",
        Level = LogLevel.Information,
        Message = "Grace period reminder 2 sent for deletion process '{deletionProcessId}'.")]
    public static partial void Reminder2Sent(this ILogger logger, IdentityDeletionProcessId deletionProcessId);

    [LoggerMessage(
        EventId = 233773,
        EventName = "Devices.SendDeletionProcessGracePeriodReminders.Reminder3Sent",
        Level = LogLevel.Information,
        Message = "Grace period reminder 3 sent for deletion process '{deletionProcessId}'.")]
    public static partial void Reminder3Sent(this ILogger logger, IdentityDeletionProcessId deletionProcessId);
}
