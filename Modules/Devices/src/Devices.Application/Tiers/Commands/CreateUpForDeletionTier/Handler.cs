using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
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
        try
        {
            await _tiersRepository.FindById(TierId.Create(Tier.UP_FOR_DELETION.Id).Value, CancellationToken.None);
        }
        catch (NotFoundException)
        {
            await _tiersRepository.AddAsync(Tier.UP_FOR_DELETION, cancellationToken);
        }
    }
}
