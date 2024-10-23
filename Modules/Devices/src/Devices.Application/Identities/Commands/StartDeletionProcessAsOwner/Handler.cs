using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcessAsOwner;

public class Handler : IRequestHandler<StartDeletionProcessAsOwnerCommand, StartDeletionProcessAsOwnerResponse>
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

    public async Task<StartDeletionProcessAsOwnerResponse> Handle(StartDeletionProcessAsOwnerCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(_userContext.GetAddress(), cancellationToken, true) ?? throw new NotFoundException(nameof(Identity));

        var deletionProcess = identity.StartDeletionProcessAsOwner(_userContext.GetDeviceId());

        try
        {
            await _identitiesRepository.Update(identity, cancellationToken);
        }
        catch (OnlyOneActiveDeletionProcessAllowedException)
        {
            throw new DomainException(DomainErrors.OnlyOneActiveDeletionProcessAllowed());
        }

        await _notificationSender.SendNotification(identity.Address, new DeletionProcessStartedPushNotification(), cancellationToken);

        return new StartDeletionProcessAsOwnerResponse(deletionProcess);
    }
}
