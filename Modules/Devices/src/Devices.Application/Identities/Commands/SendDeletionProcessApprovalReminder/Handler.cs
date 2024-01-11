using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.SendDeletionProcessApprovalReminder;

public class Handler : IRequestHandler<SendDeletionProcessApprovalReminderCommand>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IPushNotificationSender _pushNotificationSender;

    public Handler(IIdentitiesRepository identitiesRepository, IPushNotificationSender pushNotificationSender)
    {
        _identitiesRepository = identitiesRepository;
        _pushNotificationSender = pushNotificationSender;
    }

    public async Task Handle(SendDeletionProcessApprovalReminderCommand request, CancellationToken cancellationToken)
    {
        var identities = await _identitiesRepository.FindAllWithDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval, cancellationToken, track: true);

        foreach (var identity in identities)
        {
            var deletionProcess = identity.GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval) ?? throw new NotFoundException(nameof(IdentityDeletionProcess));
            var endOfApprovalPeriod = deletionProcess.CreatedAt.AddDays(IdentityDeletionConfiguration.MaxApprovalTime);
            var daysUntilApprovalPeriodEnds = (endOfApprovalPeriod - SystemTime.UtcNow).Days;

            if (deletionProcess.ApprovalReminder3SentAt != null) continue;

            if (daysUntilApprovalPeriodEnds <= IdentityDeletionConfiguration.ApprovalReminder3.Time)
            {
                await SendReminder3(identity, daysUntilApprovalPeriodEnds, cancellationToken);
                continue;
            }

            if (deletionProcess.ApprovalReminder2SentAt != null) continue;
            if (daysUntilApprovalPeriodEnds <= IdentityDeletionConfiguration.ApprovalReminder2.Time)
            {
                await SendReminder2(identity, daysUntilApprovalPeriodEnds, cancellationToken);
                continue;
            }

            if (deletionProcess.ApprovalReminder1SentAt == null && daysUntilApprovalPeriodEnds <= IdentityDeletionConfiguration.ApprovalReminder1.Time)
            {
                await SendReminder1(identity, daysUntilApprovalPeriodEnds, cancellationToken);
            }
        }
    }

    private async Task SendReminder3(Identity identity, int daysUntilApprovalPeriodEnds, CancellationToken cancellationToken)
    {
        await _pushNotificationSender.SendNotification(identity.Address, new DeletionProcessWaitingForApprovalReminderPushNotification(daysUntilApprovalPeriodEnds), cancellationToken);
        identity.DeletionProcessApprovalReminder3Sent();
        await _identitiesRepository.Update(identity, cancellationToken);
    }

    private async Task SendReminder2(Identity identity, int daysUntilApprovalPeriodEnds, CancellationToken cancellationToken)
    {
        await _pushNotificationSender.SendNotification(identity.Address, new DeletionProcessWaitingForApprovalReminderPushNotification(daysUntilApprovalPeriodEnds), cancellationToken);
        identity.DeletionProcessApprovalReminder2Sent();
        await _identitiesRepository.Update(identity, cancellationToken);
    }
    private async Task SendReminder1(Identity identity, int daysUntilApprovalPeriodEnds, CancellationToken cancellationToken)
    {
        await _pushNotificationSender.SendNotification(identity.Address, new DeletionProcessWaitingForApprovalReminderPushNotification(daysUntilApprovalPeriodEnds), cancellationToken);
        identity.DeletionProcessApprovalReminder1Sent();
        await _identitiesRepository.Update(identity, cancellationToken);
    }
}
