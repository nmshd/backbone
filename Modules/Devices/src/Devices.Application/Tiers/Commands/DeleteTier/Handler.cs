using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using MediatR;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Devices.Application.Tiers.Commands.DeleteTier;

public class Handler : IRequestHandler<DeleteTierCommand>
{
    private readonly ITiersRepository _tiersRepository;
    private readonly IEventBus _eventBus;

    public Handler(ITiersRepository tiersRepository, IEventBus eventBus)
    {
        _tiersRepository = tiersRepository;
        _eventBus = eventBus;
    }

    public async Task Handle(DeleteTierCommand request, CancellationToken cancellationToken)
    {
        var tierId = TierId.Create(request.TierId);
        
        if (tierId.IsFailure)
            throw new ApplicationException(ApplicationErrors.Devices.InvalidTierId());

        var tier = await _tiersRepository.FindById(tierId.Value, cancellationToken);

        if (tier.IsBasicTier())
        {
            throw new ApplicationException(ApplicationErrors.Devices.BasicTierCannotBeDeleted());
        }

        var identitiesCount = await _tiersRepository.GetIdentitiesCount(tier, cancellationToken);

        if (identitiesCount > 0)
        {
            throw new ApplicationException(ApplicationErrors.Devices.UsedTierCannotBeDeleted(identitiesCount));
        }

        await _tiersRepository.Remove(tier);

        _eventBus.Publish(new TierDeletedIntegrationEvent(tier));
    }
}
