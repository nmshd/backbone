using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.DeletionProcessGracePeriod;

public class Handler : IRequestHandler<SendDeletionProcessGracePeriodReminderCommand>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IPushNotificationSender _pushSender;

    public Handler(IIdentitiesRepository identitiesRepository, IPushNotificationSender pushSender)
    {
        _identitiesRepository = identitiesRepository;
        _pushSender = pushSender;
    }

    public async Task Handle(SendDeletionProcessGracePeriodReminderCommand request, CancellationToken cancellationToken)
    {
        var identities = await _identitiesRepository.FindAllWithApprovedDeletionProcess(cancellationToken, track: true);

        foreach (var identity in identities)
        {
            var deletionProcess = identity.GetDeletionProcessInStatus(DeletionProcessStatus.Approved) ?? throw new NotFoundException(nameof(IdentityDeletionProcess));
            var daysToDeletion = (deletionProcess.GracePeriodEndsAt!.Value - SystemTime.UtcNow).Days;

            if (deletionProcess.GracePeriodReminder3SentAt != null) continue;
            if (daysToDeletion <= IdentityDeletionConfiguration.GracePeriodNotification3Time)
            {
                await SendReminder3(identity, daysToDeletion, cancellationToken);
                continue;
            }

            if (deletionProcess.GracePeriodReminder2SentAt != null) continue;
            if (daysToDeletion <= IdentityDeletionConfiguration.GracePeriodNotification2Time)
            {
                await SendReminder2(identity, daysToDeletion, cancellationToken);
                continue;
            }

            if (deletionProcess.GracePeriodReminder1SentAt == null && daysToDeletion <= IdentityDeletionConfiguration.GracePeriodNotification1Time)
            {
                await SendReminder1(identity, daysToDeletion, cancellationToken);
            }
        }
    }

    private async Task SendReminder3(Identity identity, int daysToDeletion, CancellationToken cancellationToken)
    {
        await _pushSender.SendNotification(identity.Address, new DeletionProcessGracePeriodNotification(daysToDeletion), cancellationToken);
        identity.DeletionGracePeriodReminder3Sent();
        await _identitiesRepository.Update(identity, cancellationToken);
    }

    private async Task SendReminder2(Identity identity, int daysToDeletion, CancellationToken cancellationToken)
    {
        await _pushSender.SendNotification(identity.Address, new DeletionProcessGracePeriodNotification(daysToDeletion), cancellationToken);
        identity.DeletionGracePeriodReminder2Sent();
        await _identitiesRepository.Update(identity, cancellationToken);
    }
    private async Task SendReminder1(Identity identity, int daysToDeletion, CancellationToken cancellationToken)
    {
        await _pushSender.SendNotification(identity.Address, new DeletionProcessGracePeriodNotification(daysToDeletion), cancellationToken);
        identity.DeletionGracePeriodReminder1Sent();
        await _identitiesRepository.Update(identity, cancellationToken);
    }
}
