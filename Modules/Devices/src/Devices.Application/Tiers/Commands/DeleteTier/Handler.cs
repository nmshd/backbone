using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using MediatR;

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

        var tier = await _tiersRepository.Get(tierIdResult.Value, cancellationToken) ?? throw new NotFoundException(nameof(Tier));

        var clientsCount = await _tiersRepository.GetNumberOfClientsWithDefaultTier(tier, cancellationToken);

        var identitiesCount = await _tiersRepository.GetNumberOfIdentitiesAssignedToTier(tier, cancellationToken);

        var deletionError = tier.CanBeDeleted(clientsCount, identitiesCount);

        if (deletionError != null)
        {
            throw new DomainException(deletionError);
        }

        await _tiersRepository.Remove(tier);

        await _eventBus.Publish(new TierDeletedDomainEvent(tier));
    }
}
