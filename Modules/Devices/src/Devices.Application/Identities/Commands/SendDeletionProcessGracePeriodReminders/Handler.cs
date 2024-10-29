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
        var identities = await _identitiesRepository.FindAllWithDeletionProcessInStatus(DeletionProcessStatus.Approved, cancellationToken, track: true);

        _logger.LogTrace("Processing identities with deletion process in status 'Approved' ...");

        foreach (var identity in identities)
        {
            var deletionProcess = identity.GetDeletionProcessInStatus(DeletionProcessStatus.Approved) ?? throw new NotFoundException(nameof(IdentityDeletionProcess));
            var secondsUntilDeletion = (ulong)(deletionProcess.GracePeriodEndsAt!.Value - SystemTime.UtcNow).TotalSeconds;

            if (deletionProcess.GracePeriodReminder3SentAt != null)
            {
                _logger.NoReminderSent();
                continue;
            }

            if (secondsUntilDeletion <= IdentityDeletionConfiguration.Instance.GracePeriodNotification3.SecondsBeforeEndOfGracePeriod)
            {
                await SendReminder3(identity, secondsUntilDeletion, deletionProcess.Id, cancellationToken);
                continue;
            }

            if (deletionProcess.GracePeriodReminder2SentAt != null)
                continue;

            if (secondsUntilDeletion <= IdentityDeletionConfiguration.Instance.GracePeriodNotification2.SecondsBeforeEndOfGracePeriod)
            {
                await SendReminder2(identity, secondsUntilDeletion, deletionProcess.Id, cancellationToken);
                continue;
            }

            if (deletionProcess.GracePeriodReminder1SentAt != null)
                continue;

            if (secondsUntilDeletion <= IdentityDeletionConfiguration.Instance.GracePeriodNotification1.SecondsBeforeEndOfGracePeriod)
            {
                await SendReminder1(identity, secondsUntilDeletion, deletionProcess.Id, cancellationToken);
            }
        }
    }

    private async Task SendReminder3(Identity identity, ulong secondsUntilApprovalPeriodEnds, IdentityDeletionProcessId deletionProcessId, CancellationToken cancellationToken)
    {
        var daysToDeletion = (int)TimeSpan.FromSeconds(secondsUntilApprovalPeriodEnds).TotalDays;
        await _pushSender.SendNotification(identity.Address, new DeletionProcessGracePeriodReminderPushNotification(daysToDeletion), cancellationToken);
        identity.DeletionGracePeriodReminder3Sent();
        await _identitiesRepository.Update(identity, cancellationToken);
        _logger.Reminder3Sent(deletionProcessId);
    }

    private async Task SendReminder2(Identity identity, ulong secondsUntilApprovalPeriodEnds, IdentityDeletionProcessId deletionProcessId, CancellationToken cancellationToken)
    {
        var daysToDeletion = (int)TimeSpan.FromSeconds(secondsUntilApprovalPeriodEnds).TotalDays;
        await _pushSender.SendNotification(identity.Address, new DeletionProcessGracePeriodReminderPushNotification(daysToDeletion), cancellationToken);
        identity.DeletionGracePeriodReminder2Sent();
        await _identitiesRepository.Update(identity, cancellationToken);
        _logger.Reminder2Sent(deletionProcessId);
    }

    private async Task SendReminder1(Identity identity, ulong secondsUntilApprovalPeriodEnds, IdentityDeletionProcessId deletionProcessId, CancellationToken cancellationToken)
    {
        var daysToDeletion = (int)TimeSpan.FromSeconds(secondsUntilApprovalPeriodEnds).TotalDays;
        await _pushSender.SendNotification(identity.Address, new DeletionProcessGracePeriodReminderPushNotification(daysToDeletion), cancellationToken);
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
