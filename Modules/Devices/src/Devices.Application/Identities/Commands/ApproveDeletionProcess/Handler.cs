using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling.Extensions;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.ApproveDeletionProcess;

public class Handler : IRequestHandler<ApproveDeletionProcessCommand, ApproveDeletionProcessResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IUserContext _userContext;
    private readonly IEventBus _eventBus;
    private readonly IPushNotificationSender _notificationSender;

    public Handler(IIdentitiesRepository identitiesRepository, IUserContext userContext, IEventBus eventBus, IPushNotificationSender notificationSender)
    {
        _identitiesRepository = identitiesRepository;
        _userContext = userContext;
        _eventBus = eventBus;
        _notificationSender = notificationSender;
    }

    public async Task<ApproveDeletionProcessResponse> Handle(ApproveDeletionProcessCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(_userContext.GetAddress(), cancellationToken, track: true) ?? throw new NotFoundException(nameof(Identity));

        var identityDeletionProcessIdResult = IdentityDeletionProcessId.Create(request.DeletionProcessId);

        if (identityDeletionProcessIdResult.IsFailure)
            throw new DomainException(identityDeletionProcessIdResult.Error);

        var identityDeletionProcessId = identityDeletionProcessIdResult.Value;

        var oldTierId = identity.TierId;
        var deletionProcess = identity.ApproveDeletionProcess(identityDeletionProcessId, _userContext.GetDeviceId());
        var newTierId = identity.TierId;

        await _identitiesRepository.Update(identity, cancellationToken);

        _eventBus.Publish(new TierOfIdentityChangedDomainEvent(identity, oldTierId, newTierId));

        var daysUntilDeletion = deletionProcess.GracePeriodEndsAt?.DaysUntilDate() ?? throw new Exception($"Expected '{nameof(deletionProcess.GracePeriodEndsAt)}' to be set but found 'null' instead.");
        await _notificationSender.SendNotification(identity.Address, new DeletionProcessApprovedNotification(daysUntilDeletion), cancellationToken);

        return new ApproveDeletionProcessResponse(deletionProcess);
    }
}
