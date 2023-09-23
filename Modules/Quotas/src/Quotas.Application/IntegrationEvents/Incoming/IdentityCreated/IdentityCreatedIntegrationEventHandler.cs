using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
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

        var identity = new Identity(integrationEvent.Address, new TierId(integrationEvent.Tier));

        var tier = await _tiersRepository.Find(identity.TierId, CancellationToken.None, track: true);

        foreach (var tierQuotaDefinition in tier.Quotas)
        {
            identity.AssignTierQuotaFromDefinition(tierQuotaDefinition);
        }

        await _identitiesRepository.Add(identity, CancellationToken.None);

        _logger.LogTrace("Successfully created identity. Identity Address: {address}, Tier ID: {tierId}", identity.Address, identity.TierId);

        _logger.LogTrace("{quotasCount} Tier Quotas created for Identity: {identityAddress} ", tier.Quotas.Count, identity.Address);
    }
}
