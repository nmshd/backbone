using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;
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
        var newTierId = TierId.Create(request.TierId);
        if (!newTierId.IsSuccess)
        {
            throw new ApplicationException(new ApplicationError(newTierId.Error.Code, newTierId.Error.Message));
        }

        var identity = await _identitiesRepository.FindByAddress(request.Address, cancellationToken) ?? throw new NotFoundException(nameof(Identity));

        var oldTier = await _tiersRepository.FindById(identity.TierId, cancellationToken) ?? throw new NotFoundException(nameof(Tier));
        var newTier = await _tiersRepository.FindById(newTierId.Value, cancellationToken) ?? throw new NotFoundException(nameof(Tier));

        identity.TierId = newTier.Id;
        await _identitiesRepository.Update(identity, cancellationToken);

        _eventBus.Publish(new TierOfIdentityChangedIntegrationEvent(identity, oldTier, newTier));

        return identity;
    }
}
