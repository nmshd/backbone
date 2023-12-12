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
        var identitiesWithDeletionProcessWaitingForApproval = await _identitiesRepository.FindAllWithDeletionProcessWaitingForApproval(cancellationToken);

        foreach (var identity in identitiesWithDeletionProcessWaitingForApproval)
        {
            var waitingForApprovalDeletionProcess = identity.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.WaitingForApproval)!;
            var endOfApprovalPeriod = waitingForApprovalDeletionProcess.CreatedAt.AddDays(IdentityDeletionConfiguration.MaxApprovalTime);
            var daysUntilApprovalPeriodEnds = (endOfApprovalPeriod - SystemTime.UtcNow).Days;
            if (waitingForApprovalDeletionProcess.ApprovalReminder1SentAt == null
                && endOfApprovalPeriod.AddDays(-IdentityDeletionConfiguration.ApprovalReminder1.Time) <= SystemTime.UtcNow)
            {
                await _pushNotificationSender.SendNotification(identity.Address, new DeletionProcessWaitingForApprovalReminderPushNotification(daysUntilApprovalPeriodEnds), CancellationToken.None);
                identity.DeletionProcessApprovalReminder1Sent(waitingForApprovalDeletionProcess.Id);
                await _identitiesRepository.Update(identity, cancellationToken);
            }

            if (waitingForApprovalDeletionProcess.ApprovalReminder2SentAt == null
                && endOfApprovalPeriod.AddDays(-IdentityDeletionConfiguration.ApprovalReminder2.Time) <= SystemTime.UtcNow)
            {
                await _pushNotificationSender.SendNotification(identity.Address, new DeletionProcessWaitingForApprovalReminderPushNotification(daysUntilApprovalPeriodEnds), CancellationToken.None);
                identity.DeletionProcessApprovalReminder2Sent(waitingForApprovalDeletionProcess.Id);
                await _identitiesRepository.Update(identity, cancellationToken);
            }

            if (waitingForApprovalDeletionProcess.ApprovalReminder3SentAt == null
                && endOfApprovalPeriod.AddDays(-IdentityDeletionConfiguration.ApprovalReminder3.Time) <= SystemTime.UtcNow)
            {
                await _pushNotificationSender.SendNotification(identity.Address, new DeletionProcessWaitingForApprovalReminderPushNotification(daysUntilApprovalPeriodEnds), CancellationToken.None);
                identity.DeletionProcessApprovalReminder3Sent(waitingForApprovalDeletionProcess.Id);
                await _identitiesRepository.Update(identity, cancellationToken);
            }
        }
    }
}
