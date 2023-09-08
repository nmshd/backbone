using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.UpdateIdentity;
public class Handler : IRequestHandler<UpdateIdentityCommand, Identity>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ITiersRepository _tiersRepository;
    private readonly IEventBus _eventBus;

    public Handler(IIdentitiesRepository identitiesRepository, ITiersRepository tiersRepository, IEventBus eventBus)
    {
        _identitiesRepository = identitiesRepository;
        _tiersRepository = tiersRepository;
        _eventBus = eventBus;
    }

    public async Task<Identity> Handle(UpdateIdentityCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(request.Address, cancellationToken);

        var oldTier = await _tiersRepository.FindById(identity.TierId, cancellationToken);
        var newTier = await _tiersRepository.FindById(TierId.Create(request.TierId).Value, cancellationToken);

        identity.TierId = newTier.Id;
        await _identitiesRepository.Update(identity, cancellationToken);

        _eventBus.Publish(new TierOfIdentityChangedIntegrationEvent(identity, oldTier, newTier));

        return identity;
    }
}
