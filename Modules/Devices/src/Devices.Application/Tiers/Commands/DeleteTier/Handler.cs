using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using MediatR;
using Microsoft.Extensions.Logging;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Devices.Application.Tiers.Commands.DeleteTier;

public class Handler : IRequestHandler<DeleteTierCommand, DeleteTierResponse>
{
    private readonly ITiersRepository _tierRepository;
    private readonly ILogger<Handler> _logger;
    private readonly IEventBus _eventBus;

    public Handler(ITiersRepository tierRepository, ILogger<Handler> logger, IEventBus eventBus)
    {
        _tierRepository = tierRepository;
        _logger = logger;
        _eventBus = eventBus;
    }

    public async Task<DeleteTierResponse> Handle(DeleteTierCommand request, CancellationToken cancellationToken)
    {
        var tier = await _tierRepository.FindById(request.TierId, cancellationToken);

        if (tier == await _tierRepository.GetBasicTierAsync(cancellationToken))
        {
            throw new ApplicationException(ApplicationErrors.Devices.BasicTierCannotBeDeleted());
        }

        if (tier.Identities.Count() > 0)
        {
            throw new ApplicationException(ApplicationErrors.Devices.UsedTierCannotBeDeleted(tier.Identities.Count()));
        }

        await _tierRepository.Remove(tier);

        _eventBus.Publish(new TierDeletedIntegrationEvent(tier));

        return new DeleteTierResponse();
    }
}
