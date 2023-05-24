using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.IdentityCreated;

public class IdentityCreatedIntegrationEventHandler : IIntegrationEventHandler<IdentityCreatedIntegrationEvent>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ITiersRepository _tiersRepository;
    private readonly ILogger<IdentityCreatedIntegrationEventHandler> _logger;

    public IdentityCreatedIntegrationEventHandler(IIdentitiesRepository identitiesRepository, ILogger<IdentityCreatedIntegrationEventHandler> logger, ITiersRepository tiersRepository)
    {
        _identitiesRepository = identitiesRepository;
        _logger = logger;
        _tiersRepository = tiersRepository;
    }

    public async Task Handle(IdentityCreatedIntegrationEvent integrationEvent)
    {
        _logger.LogTrace("Handling IdentityCreatedIntegrationEvent ...");

        var identity = new Identity(integrationEvent.Address, integrationEvent.Tier);

        var tier = await _tiersRepository.Find(identity.TierId, CancellationToken.None);

        tier.Quotas.ForEach(identity.AssignTierQuotaFromDefinition);

        await _identitiesRepository.Add(identity, CancellationToken.None);

        _logger.LogTrace($"Successfully created identity. Identity Address: {identity.Address}, Tier ID: {identity.TierId}");

        _logger.LogTrace($"{tier.Quotas.Count} Tier Quotas created for Identity: {identity.Address} ");
    }
}
