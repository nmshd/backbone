using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessAsSupport;

public class Handler : IRequestHandler<CancelDeletionAsSupportCommand, CancelDeletionAsSupportResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IPushNotificationSender _notificationSender;

    public Handler(IIdentitiesRepository identitiesRepository, IPushNotificationSender notificationSender)
    {
        _identitiesRepository = identitiesRepository;
        _notificationSender = notificationSender;
    }

    public async Task<CancelDeletionAsSupportResponse> Handle(CancelDeletionAsSupportCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(request.Address, cancellationToken, track: true) ?? throw new NotFoundException(nameof(Identity));

        var deletionProcessId = IdentityDeletionProcessId.Create(request.DeletionProcessId).Value;

        var deletionProcess = identity.CancelDeletionProcessAsSupport(deletionProcessId);

        await _identitiesRepository.Update(identity, cancellationToken);

        await _notificationSender.SendNotification(
            new DeletionProcessCancelledBySupportPushNotification(),
            SendPushNotificationFilter.AllDevicesOf(identity.Address),
            cancellationToken
        );

        return new CancelDeletionAsSupportResponse(deletionProcess);
    }
}
