using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.ApproveDeletionProcess;

public class Handler : IRequestHandler<ApproveDeletionProcessCommand, ApproveDeletionProcessResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ITiersRepository _tiersRepository;
    private readonly IUserContext _userContext;
    private readonly IEventBus _eventBus;

    public Handler(IIdentitiesRepository identitiesRepository, ITiersRepository tiersRepository, IUserContext userContext, IEventBus eventBus)
    {
        _identitiesRepository = identitiesRepository;
        _tiersRepository = tiersRepository;
        _userContext = userContext;
        _eventBus = eventBus;
    }

    public async Task<ApproveDeletionProcessResponse> Handle(ApproveDeletionProcessCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(_userContext.GetAddress(), cancellationToken) ?? throw new NotFoundException(nameof(Identity));

        var oldTierId = identity.TierId;
        var oldTier = await _tiersRepository.FindById(oldTierId, cancellationToken) ?? throw new NotFoundException(nameof(Tier));

        var deletionProcess = identity.ApproveDeletionProcess(request.DeletionProcessId, _userContext.GetDeviceId());
        
        var newTierId = identity.TierId;
        var newTier = await _tiersRepository.FindById(newTierId, cancellationToken) ?? throw new NotFoundException(nameof(Tier));

        await _identitiesRepository.Update(identity, cancellationToken);

        _eventBus.Publish(new IdentityToBeDeletedIntegrationEvent(identity.Address, deletionProcess.Id));
        _eventBus.Publish(new TierOfIdentityChangedIntegrationEvent(identity, oldTier, newTier));

        return new ApproveDeletionProcessResponse(deletionProcess);
    }
}
