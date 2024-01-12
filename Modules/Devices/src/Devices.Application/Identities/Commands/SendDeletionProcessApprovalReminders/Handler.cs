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
        var identities = await _identitiesRepository.FindAllWithDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval, cancellationToken, track: true);

        _logger.LogTrace("Processing identities with deletion process in status 'Waiting for Approval' ...");

        foreach (var identity in identities)
        {
            var deletionProcess = identity.GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval) ?? throw new NotFoundException(nameof(IdentityDeletionProcess));
            var endOfApprovalPeriod = deletionProcess.GetEndOfApprovalPeriod();
            var daysUntilApprovalPeriodEnds = (endOfApprovalPeriod - SystemTime.UtcNow).Days;

            if (deletionProcess.ApprovalReminder3SentAt != null)
            {
                _logger.LogTrace($"Identity '{identity.Address}': No Approval reminder sent.");
                continue;
            }

            if (daysUntilApprovalPeriodEnds <= IdentityDeletionConfiguration.ApprovalReminder3.Time)
            {
                await SendReminder3(identity, daysUntilApprovalPeriodEnds, deletionProcess.Id, cancellationToken);
                continue;
            }

            if (deletionProcess.ApprovalReminder2SentAt != null) continue;
            if (daysUntilApprovalPeriodEnds <= IdentityDeletionConfiguration.ApprovalReminder2.Time)
            {
                await SendReminder2(identity, daysUntilApprovalPeriodEnds, deletionProcess.Id, cancellationToken);
                continue;
            }

            if (deletionProcess.ApprovalReminder1SentAt == null && daysUntilApprovalPeriodEnds <= IdentityDeletionConfiguration.ApprovalReminder1.Time)
            {
                await SendReminder1(identity, daysUntilApprovalPeriodEnds, deletionProcess.Id, cancellationToken);
            }
        }
    }

    private async Task SendReminder3(Identity identity, int daysUntilApprovalPeriodEnds, IdentityDeletionProcessId deletionProcessId, CancellationToken cancellationToken)
    {
        await _pushNotificationSender.SendNotification(identity.Address, new DeletionProcessWaitingForApprovalReminderPushNotification(daysUntilApprovalPeriodEnds), cancellationToken);
        identity.DeletionProcessApprovalReminder3Sent();
        await _identitiesRepository.Update(identity, cancellationToken);
        _logger.LogTrace($"Identity '{identity.Address}': Approval reminder 3 sent for deletion process '{deletionProcessId}'");
    }

    private async Task SendReminder2(Identity identity, int daysUntilApprovalPeriodEnds, IdentityDeletionProcessId deletionProcessId, CancellationToken cancellationToken)
    {
        await _pushNotificationSender.SendNotification(identity.Address, new DeletionProcessWaitingForApprovalReminderPushNotification(daysUntilApprovalPeriodEnds), cancellationToken);
        identity.DeletionProcessApprovalReminder2Sent();
        await _identitiesRepository.Update(identity, cancellationToken);
        _logger.LogTrace($"Identity '{identity.Address}': Approval reminder 2 sent for deletion process '{deletionProcessId}'");
    }
    private async Task SendReminder1(Identity identity, int daysUntilApprovalPeriodEnds, IdentityDeletionProcessId deletionProcessId, CancellationToken cancellationToken)
    {
        await _pushNotificationSender.SendNotification(identity.Address, new DeletionProcessWaitingForApprovalReminderPushNotification(daysUntilApprovalPeriodEnds), cancellationToken);
        identity.DeletionProcessApprovalReminder1Sent();
        await _identitiesRepository.Update(identity, cancellationToken);
        _logger.LogTrace($"Identity '{identity.Address}': Approval reminder 1 sent for deletion process '{deletionProcessId}'");
    }
}
