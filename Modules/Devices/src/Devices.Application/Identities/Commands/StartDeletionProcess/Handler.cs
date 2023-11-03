using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.Entities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using MediatR;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcess;

public class Handler : IRequestHandler<StartDeletionProcessCommand>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IEventBus _eventBus;
    private readonly IUserContext _userContext;

    public Handler(IIdentitiesRepository identitiesRepository, IEventBus eventBus, IUserContext userContext)
    {
        _identitiesRepository = identitiesRepository;
        _eventBus = eventBus;
        _userContext = userContext;
    }

    public async Task Handle(StartDeletionProcessCommand request, CancellationToken cancellationToken)
    {
        EnsureStartedByOwnerOrSupport(request);

        var identity = await _identitiesRepository.FindByAddress(request.IdentityAddress, cancellationToken, true) ?? throw new NotFoundException(nameof(Identity));

        identity.StartDeletionProcess();

        await _identitiesRepository.Update(identity, cancellationToken);

        _eventBus.Publish(new IdentityDeletionProcessStartedIntegrationEvent(identity.Address));
    }

    private void EnsureStartedByOwnerOrSupport(StartDeletionProcessCommand request)
    {
        var address = _userContext.GetAddressOrNull();
        var userIsSupport = address == null;
        var userIsOwner = address == request.IdentityAddress;

        if (!userIsOwner && !userIsSupport)
        {
            throw new ApplicationException(ApplicationErrors.Identities.CanOnlyStartDeletionProcessForOwnIdentity());
        }
    }
}
