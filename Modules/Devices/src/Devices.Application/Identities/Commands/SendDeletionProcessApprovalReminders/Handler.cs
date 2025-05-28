using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Application.Identities.Commands.SendDeletionProcessApprovalReminders;

public class Handler : IRequestHandler<SendDeletionProcessApprovalRemindersCommand>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IPushNotificationSender _pushNotificationSender;
    private readonly ILogger<Handler> _logger;

    public Handler(IIdentitiesRepository identitiesRepository, IPushNotificationSender pushNotificationSender, ILogger<Handler> logger)
    {
        _identitiesRepository = identitiesRepository;
        _pushNotificationSender = pushNotificationSender;
        _logger = logger;
    }

    public async Task Handle(SendDeletionProcessApprovalRemindersCommand request, CancellationToken cancellationToken)
    {
        var identities = await _identitiesRepository.ListAllWithDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval, cancellationToken, track: true);

        _logger.LogTrace("Processing identities with deletion process in status 'Waiting for Approval' ...");

        foreach (var identity in identities)
        {
            var deletionProcess = identity.GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval) ?? throw new NotFoundException(nameof(IdentityDeletionProcess));
            var daysUntilApprovalPeriodEnds = (deletionProcess.ApprovalPeriodEndsAt - SystemTime.UtcNow).TotalDays;

            if (deletionProcess.ApprovalReminder3SentAt != null)
            {
                _logger.NoApprovalReminderSent();
                continue;
            }

            if (daysUntilApprovalPeriodEnds <= IdentityDeletionConfiguration.Instance.ApprovalReminder3.DaysBeforeEndOfApprovalPeriod)
            {
                await SendReminder3(identity, daysUntilApprovalPeriodEnds, deletionProcess.Id, cancellationToken);
                continue;
            }

            if (deletionProcess.ApprovalReminder2SentAt != null) continue;
            if (daysUntilApprovalPeriodEnds <= IdentityDeletionConfiguration.Instance.ApprovalReminder2.DaysBeforeEndOfApprovalPeriod)
            {
                await SendReminder2(identity, daysUntilApprovalPeriodEnds, deletionProcess.Id, cancellationToken);
                continue;
            }

            if (deletionProcess.ApprovalReminder1SentAt == null && daysUntilApprovalPeriodEnds <= IdentityDeletionConfiguration.Instance.ApprovalReminder1.DaysBeforeEndOfApprovalPeriod)
            {
                await SendReminder1(identity, daysUntilApprovalPeriodEnds, deletionProcess.Id, cancellationToken);
            }
        }
    }

    private async Task SendReminder3(Identity identity, double daysUntilApprovalPeriodEnds, IdentityDeletionProcessId deletionProcessId, CancellationToken cancellationToken)
    {
        await _pushNotificationSender.SendNotification(
            new DeletionProcessWaitingForApprovalReminderPushNotification((int)Math.Ceiling(daysUntilApprovalPeriodEnds)),
            SendPushNotificationFilter.AllDevicesOf(identity.Address),
            cancellationToken
        );
        identity.DeletionProcessApprovalReminder3Sent();
        await _identitiesRepository.Update(identity, cancellationToken);
        _logger.ApprovalReminder3Sent(deletionProcessId);
    }

    private async Task SendReminder2(Identity identity, double daysUntilApprovalPeriodEnds, IdentityDeletionProcessId deletionProcessId, CancellationToken cancellationToken)
    {
        await _pushNotificationSender.SendNotification(
            new DeletionProcessWaitingForApprovalReminderPushNotification((int)Math.Ceiling(daysUntilApprovalPeriodEnds)),
            SendPushNotificationFilter.AllDevicesOf(identity.Address),
            cancellationToken
        );
        identity.DeletionProcessApprovalReminder2Sent();
        await _identitiesRepository.Update(identity, cancellationToken);
        _logger.ApprovalReminder2Sent(deletionProcessId);
    }

    private async Task SendReminder1(Identity identity, double daysUntilApprovalPeriodEnds, IdentityDeletionProcessId deletionProcessId, CancellationToken cancellationToken)
    {
        await _pushNotificationSender.SendNotification(
            new DeletionProcessWaitingForApprovalReminderPushNotification((int)Math.Ceiling(daysUntilApprovalPeriodEnds)),
            SendPushNotificationFilter.AllDevicesOf(identity.Address),
            cancellationToken
        );
        identity.DeletionProcessApprovalReminder1Sent();
        await _identitiesRepository.Update(identity, cancellationToken);
        _logger.ApprovalReminder1Sent(deletionProcessId);
    }
}

internal static partial class SendDeletionProcessApprovalRemindersLogs
{
    [LoggerMessage(
        EventId = 476389,
        EventName = "Devices.SendDeletionProcessApprovalReminders.NoApprovalReminderSent",
        Level = LogLevel.Information,
        Message = "No Approval reminder sent.")]
    public static partial void NoApprovalReminderSent(this ILogger logger);

    [LoggerMessage(
        EventId = 919500,
        EventName = "Devices.SendDeletionProcessApprovalReminders.ApprovalReminder1Sent",
        Level = LogLevel.Information,
        Message = "Approval reminder 1 sent for deletion process '{deletionProcessId}'.")]
    public static partial void ApprovalReminder1Sent(this ILogger logger, IdentityDeletionProcessId deletionProcessId);

    [LoggerMessage(
        EventId = 961917,
        EventName = "Devices.SendDeletionProcessApprovalReminders.ApprovalReminder2Sent",
        Level = LogLevel.Information,
        Message = "Approval reminder 2 sent for deletion process '{deletionProcessId}'.")]
    public static partial void ApprovalReminder2Sent(this ILogger logger, IdentityDeletionProcessId deletionProcessId);

    [LoggerMessage(
        EventId = 887809,
        EventName = "Devices.SendDeletionProcessApprovalReminders.ApprovalReminder3Sent",
        Level = LogLevel.Information,
        Message = "Approval reminder 3 sent for deletion process '{deletionProcessId}'.")]
    public static partial void ApprovalReminder3Sent(this ILogger logger, IdentityDeletionProcessId deletionProcessId);
}
