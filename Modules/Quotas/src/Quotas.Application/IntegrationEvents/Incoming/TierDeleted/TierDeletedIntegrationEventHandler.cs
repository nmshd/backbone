using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.Tiers.Commands.DeleteTierQuotaDefinition;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierDeleted;
public class TierDeletedIntegrationEventHandler : IIntegrationEventHandler<TierDeletedIntegrationEvent>
{
    private readonly ITiersRepository _tierRepository;
    private readonly ILogger<TierDeletedIntegrationEventHandler> _logger;
    private readonly IMediator _mediator;

    public TierDeletedIntegrationEventHandler(ILogger<TierDeletedIntegrationEventHandler> logger, ITiersRepository tierRepository, IMediator mediator)
    {
        _tierRepository = tierRepository;
        _logger = logger;
        _mediator = mediator;
    }

    public async Task Handle(TierDeletedIntegrationEvent integrationEvent)
    {
        var tier = await _tierRepository.Find(integrationEvent.Id, CancellationToken.None);

        var tasks = new List<Task>();
        foreach (var tierQuotaDefinition in tier.Quotas)
        {
            tasks.Add(_mediator.Send(new DeleteTierQuotaDefinitionCommand(tier.Id, tierQuotaDefinition.Id)));
        }
        await Task.WhenAll(tasks);

        await _tierRepository.Remove(tier);

        _logger.LogTrace($"Successfully deleted tier. Tier ID: {tier.Id}, Tier Name: {tier.Name}");
    }
}

