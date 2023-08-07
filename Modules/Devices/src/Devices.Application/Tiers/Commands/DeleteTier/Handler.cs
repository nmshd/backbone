using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Domain;
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
        var tierIdResult = TierId.Create(request.TierId);

        if (tierIdResult.IsFailure)
            throw new DomainException(tierIdResult.Error);

        var tier = await _tiersRepository.FindById(tierIdResult.Value, cancellationToken);

        var identitiesCount = await _tiersRepository.GetNumberOfIdentitiesAssignedToTier(tier, cancellationToken);

        var deletionError = tier.CanBeDeleted(identitiesCount);

        if (deletionError != null)
        {
            throw new DomainException(deletionError);
        }

        await _tiersRepository.Remove(tier);

        _eventBus.Publish(new TierDeletedIntegrationEvent(tier));
    }
}
