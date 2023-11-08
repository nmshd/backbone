using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using MediatR;

namespace Backbone.Modules.Devices.Application.Tiers.Commands.CreateUpForDeletionTier;

public class Handler : IRequestHandler<CreateUpForDeletionTierCommand>
{
    private readonly ITiersRepository _tiersRepository;

    public Handler(ITiersRepository tiersRepository)
    {
        _tiersRepository = tiersRepository;
    }

    public async Task Handle(CreateUpForDeletionTierCommand request, CancellationToken cancellationToken)
    {
        var tierName = TierName.Create(TierName.UP_FOR_DELETION_DEFAULT_NAME).Value;
        var tierId = TierId.Create(TierId.UP_FOR_DELETION_DEFAULT_ID).Value;
        var tier = new Tier(tierId, tierName);

        await _tiersRepository.AddAsync(tier, cancellationToken);
    }
}
