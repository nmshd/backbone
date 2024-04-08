using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using MediatR;

namespace Backbone.Modules.Devices.Application.Tiers.Commands.CreateQueuedForDeletionTier;

public class Handler : IRequestHandler<CreateQueuedForDeletionTierCommand>
{
    private readonly ITiersRepository _tiersRepository;

    public Handler(ITiersRepository tiersRepository)
    {
        _tiersRepository = tiersRepository;
    }

    public async Task Handle(CreateQueuedForDeletionTierCommand request, CancellationToken cancellationToken)
    {
        if (!await _tiersRepository.ExistsWithId(TierId.Create(Tier.QUEUED_FOR_DELETION.Id).Value, CancellationToken.None))
            await _tiersRepository.AddAsync(Tier.QUEUED_FOR_DELETION, cancellationToken);
    }
}
