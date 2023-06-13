﻿using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierCreated;
public class TierCreatedIntegrationEventHandler : IIntegrationEventHandler<TierCreatedIntegrationEvent>
{
    private readonly ITiersRepository _tierRepository;
    private readonly ILogger<TierCreatedIntegrationEventHandler> _logger;

    public TierCreatedIntegrationEventHandler(ITiersRepository tierRepository, ILogger<TierCreatedIntegrationEventHandler> logger)
    {
        _tierRepository = tierRepository;
        _logger = logger;
    }

    public async Task Handle(TierCreatedIntegrationEvent integrationEvent)
    {
        var tier = new Tier(new TierId(integrationEvent.Id), integrationEvent.Name);
        await _tierRepository.Add(tier, CancellationToken.None);

        _logger.LogTrace($"Successfully created tier. Tier ID: {tier.Id}, Tier Name: {tier.Name}");
    }
}

