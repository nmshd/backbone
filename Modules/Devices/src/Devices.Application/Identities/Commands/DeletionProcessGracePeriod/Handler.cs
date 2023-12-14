using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.DeletionProcessGracePeriod;
public class Handler : IRequestHandler<DeletionProcessGracePeriodCommand>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IPushNotificationSender _pushSenderService;

    public Handler(IIdentitiesRepository identitiesRepository, IPushNotificationSender pushSenderService)
    {
        _identitiesRepository = identitiesRepository;
        _pushSenderService = pushSenderService;
    }

    public async Task Handle(DeletionProcessGracePeriodCommand request, CancellationToken cancellationToken)
    {
        var identities = await _identitiesRepository.FindAllWithApprovedDeletionProcess(cancellationToken, track: true);

        foreach (var identity in identities)
        {
            var deletionProcess = identity.DeletionProcesses.First(d => d.IsActive());

            if (deletionProcess.GracePeriodReminder1SentAt == null)
            {
                var daysToDeletion = (deletionProcess.GracePeriodEndsAt! - SystemTime.UtcNow).Value.Days;

                if (daysToDeletion <= IdentityDeletionConfiguration.GracePeriodNotification1.Time)
                {
                    await _pushSenderService.SendNotification(identity.Address, new DeletionProcessGracePeriodNotification(daysToDeletion), cancellationToken);
                    identity.DeletionGracePeriodReminder1Sent();

                    await _identitiesRepository.Update(identity, cancellationToken);
                }
            }

            if (deletionProcess.GracePeriodReminder2SentAt == null)
            {
                var daysToDeletion = (deletionProcess.GracePeriodEndsAt! - SystemTime.UtcNow).Value.Days;

                if (daysToDeletion <= IdentityDeletionConfiguration.GracePeriodNotification2.Time)
                {
                    await _pushSenderService.SendNotification(identity.Address, new DeletionProcessGracePeriodNotification(daysToDeletion), cancellationToken);
                    identity.DeletionGracePeriodReminder2Sent();

                    await _identitiesRepository.Update(identity, cancellationToken);
                }
            }

            if (deletionProcess.GracePeriodReminder3SentAt == null)
            {
                var daysToDeletion = (deletionProcess.GracePeriodEndsAt! - SystemTime.UtcNow).Value.Days;

                if (daysToDeletion <= IdentityDeletionConfiguration.GracePeriodNotification3.Time)
                {
                    await _pushSenderService.SendNotification(identity.Address, new DeletionProcessGracePeriodNotification(daysToDeletion), cancellationToken);
                    identity.DeletionGracePeriodReminder3Sent();

                    await _identitiesRepository.Update(identity, cancellationToken);
                }
            }
        }
    }
}
