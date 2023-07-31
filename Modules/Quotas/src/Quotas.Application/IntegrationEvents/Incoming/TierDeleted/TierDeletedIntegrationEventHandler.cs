using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.Tiers.Commands.DeleteTierQuotaDefinition;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierDeleted;
public class TierDeletedIntegrationEventHandler : IIntegrationEventHandler<TierDeletedIntegrationEvent>
{
    private readonly ITiersRepository _tiersRepository;
    private readonly ILogger<TierDeletedIntegrationEventHandler> _logger;
    private readonly IMediator _mediator;

    public TierDeletedIntegrationEventHandler(ILogger<TierDeletedIntegrationEventHandler> logger, ITiersRepository tiersRepository, IMediator mediator)
    {
        _tiersRepository = tiersRepository;
        _logger = logger;
        _mediator = mediator;
    }

    public async Task Handle(TierDeletedIntegrationEvent integrationEvent)
    {
        var tier = await _tiersRepository.Find(integrationEvent.Id, CancellationToken.None);

        await _tiersRepository.Remove(tier);

        _logger.LogTrace($"Successfully deleted tier. Tier ID: {tier.Id}, Tier Name: {tier.Name}");
    }
}

