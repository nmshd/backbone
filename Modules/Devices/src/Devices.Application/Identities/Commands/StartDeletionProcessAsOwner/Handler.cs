using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling.Extensions;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcessAsOwner;

public class Handler : IRequestHandler<StartDeletionProcessAsOwnerCommand, StartDeletionProcessAsOwnerResponse>
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

    public async Task<StartDeletionProcessAsOwnerResponse> Handle(StartDeletionProcessAsOwnerCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(_userContext.GetAddress(), cancellationToken, true) ?? throw new NotFoundException(nameof(Identity));

        var oldTierId = identity.TierId;
        var deletionProcess = identity.StartDeletionProcessAsOwner(_userContext.GetDeviceId());
        var newTierId = identity.TierId;

        _eventBus.Publish(new TierOfIdentityChangedDomainEvent(identity, oldTierId, newTierId));

        await _identitiesRepository.Update(identity, cancellationToken);

        var daysUntilDeletion = deletionProcess.GracePeriodEndsAt?.DaysUntilDate() ?? throw new DomainException(DomainErrors.GracePeriodHasNotYetExpired());
        await _notificationSender.SendNotification(identity.Address, new DeletionProcessApprovedNotification(daysUntilDeletion), cancellationToken);

        return new StartDeletionProcessAsOwnerResponse(deletionProcess);
    }
}
