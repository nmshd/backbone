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
            var daysToDeletion = (deletionProcess.GracePeriodEndsAt!.Value - SystemTime.UtcNow).Days;

            if (deletionProcess.GracePeriodReminder3SentAt != null)
            {
                _logger.LogTrace($"Identity '{identity.Address}': No Grace period reminder sent.");
                continue;
            }

            if (daysToDeletion <= IdentityDeletionConfiguration.GracePeriodNotification3.Time)
            {
                await SendReminder3(identity, daysToDeletion, deletionProcess.Id, cancellationToken);
                continue;
            }

            if (deletionProcess.GracePeriodReminder2SentAt != null) continue;
            if (daysToDeletion <= IdentityDeletionConfiguration.GracePeriodNotification2.Time)
            {
                await SendReminder2(identity, daysToDeletion, deletionProcess.Id, cancellationToken);
                continue;
            }

            if (deletionProcess.GracePeriodReminder1SentAt == null && daysToDeletion <= IdentityDeletionConfiguration.GracePeriodNotification1.Time)
            {
                await SendReminder1(identity, daysToDeletion, deletionProcess.Id, cancellationToken);
            }
        }
    }

    private async Task SendReminder3(Identity identity, int daysToDeletion, IdentityDeletionProcessId deletionProcessId, CancellationToken cancellationToken)
    {
        await _pushSender.SendNotification(identity.Address, new DeletionProcessGracePeriodNotification(daysToDeletion), cancellationToken);
        identity.DeletionGracePeriodReminder3Sent();
        await _identitiesRepository.Update(identity, cancellationToken);
        _logger.LogTrace($"Identity '{identity.Address}': Grace period reminder 3 sent for deletion process '{deletionProcessId}'");
    }

    private async Task SendReminder2(Identity identity, int daysToDeletion, IdentityDeletionProcessId deletionProcessId, CancellationToken cancellationToken)
    {
        await _pushSender.SendNotification(identity.Address, new DeletionProcessGracePeriodNotification(daysToDeletion), cancellationToken);
        identity.DeletionGracePeriodReminder2Sent();
        await _identitiesRepository.Update(identity, cancellationToken);
        _logger.LogTrace($"Identity '{identity.Address}': Grace period reminder 2 sent for deletion process '{deletionProcessId}'");
    }
    private async Task SendReminder1(Identity identity, int daysToDeletion, IdentityDeletionProcessId deletionProcessId, CancellationToken cancellationToken)
    {
        await _pushSender.SendNotification(identity.Address, new DeletionProcessGracePeriodNotification(daysToDeletion), cancellationToken);
        identity.DeletionGracePeriodReminder1Sent();
        await _identitiesRepository.Update(identity, cancellationToken);
        _logger.LogTrace($"Identity '{identity.Address}': Grace period reminder 1 sent for deletion process '{deletionProcessId}'");
    }
}
