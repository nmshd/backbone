using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
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
        var identity = await _identitiesRepository.FindByAddress(_userContext.GetAddress(), cancellationToken, true) ?? throw new NotFoundException(nameof(Identity));
        var oldTierId = identity.TierId;

        var deviceId = _userContext.GetDeviceId();
        var deletionProcessIdResult = IdentityDeletionProcessId.Create(request.DeletionProcessId);

        if (deletionProcessIdResult.IsFailure)
            throw new DomainException(deletionProcessIdResult.Error);

        var deletionProcessId = deletionProcessIdResult.Value;

        var deletionProcess = identity.CancelDeletionProcessAsOwner(deletionProcessId, deviceId);

        await _identitiesRepository.Update(identity, cancellationToken);
        var newTierId = identity.TierId;

        await _notificationSender.SendNotification(identity.Address, new DeletionProcessCancelledByOwnerNotification(), cancellationToken);

        return new CancelDeletionProcessAsOwnerResponse(deletionProcess);
    }
}
