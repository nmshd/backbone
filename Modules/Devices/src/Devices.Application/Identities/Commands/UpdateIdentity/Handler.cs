﻿using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using MediatR;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Devices.Application.Identities.Commands.UpdateIdentity;
public class Handler : IRequestHandler<UpdateIdentityCommand>
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

    public async Task Handle(UpdateIdentityCommand request, CancellationToken cancellationToken)
    {
        var newTierIdResult = TierId.Create(request.TierId);
        if (!newTierIdResult.IsSuccess)
        {
            throw new ApplicationException(new ApplicationError(newTierIdResult.Error.Code, newTierIdResult.Error.Message));
        }

        var identity = await _identitiesRepository.FindByAddress(request.Address, cancellationToken, track: true) ?? throw new NotFoundException(nameof(Identity));

        var tiers = await _tiersRepository.FindByIds(new List<TierId>() { identity.TierId, newTierIdResult.Value }, cancellationToken);

        var oldTier = tiers.Single(t => t.Id == identity.TierId);
        var newTier = tiers.SingleOrDefault(t => t.Id == newTierIdResult.Value) ?? throw new NotFoundException(nameof(Tier));

        identity.ChangeTier(newTier.Id);
        await _identitiesRepository.Update(identity, cancellationToken);
        _eventBus.Publish(new TierOfIdentityChangedIntegrationEvent(identity, oldTier, newTier));
    }
}
