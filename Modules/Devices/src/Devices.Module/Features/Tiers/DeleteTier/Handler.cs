using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Tiers.DeleteTier;

public class Handler : IRequestHandler<Command>
{
    private readonly ITiersRepository _tiersRepository;

    public Handler(ITiersRepository tiersRepository)
    {
        _tiersRepository = tiersRepository;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
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

        tier.Delete();
        await _tiersRepository.Remove(tier);
    }
}
