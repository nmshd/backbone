using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessAsOwner;

public class Handler : IRequestHandler<CancelDeletionProcessAsOwnerCommand, CancelDeletionProcessAsOwnerResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IUserContext _userContext;
    private readonly IPushNotificationSender _notificationSender;

    public Handler(IIdentitiesRepository identitiesRepository, IUserContext userContext, IPushNotificationSender notificationSender)
    {
        _identitiesRepository = identitiesRepository;
        _userContext = userContext;
        _notificationSender = notificationSender;
    }

    public async Task<CancelDeletionProcessAsOwnerResponse> Handle(CancelDeletionProcessAsOwnerCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.Get(_userContext.GetAddress(), cancellationToken, true) ?? throw new NotFoundException(nameof(Identity));

        var deviceId = _userContext.GetDeviceId();
        var deletionProcessIdResult = IdentityDeletionProcessId.Create(request.DeletionProcessId);

        if (deletionProcessIdResult.IsFailure)
            throw new DomainException(deletionProcessIdResult.Error);

        var deletionProcessId = deletionProcessIdResult.Value;

        var deletionProcess = identity.CancelDeletionProcessAsOwner(deletionProcessId, deviceId);

        await _identitiesRepository.Update(identity, cancellationToken);

        await _notificationSender.SendNotification(
            new DeletionProcessCancelledByOwnerPushNotification(),
            SendPushNotificationFilter.AllDevicesOf(identity.Address),
            cancellationToken
        );

        return new CancelDeletionProcessAsOwnerResponse(deletionProcess);
    }
}
