using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Entities;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming;
public class IdentityCreatedIntegrationEventHandler
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ILogger<IdentityCreatedIntegrationEventHandler> _logger;

    public IdentityCreatedIntegrationEventHandler(IIdentitiesRepository identitiesRepository, ILogger<IdentityCreatedIntegrationEventHandler> logger)
    {
        _identitiesRepository = identitiesRepository;
        _logger = logger;
    }

    public async Task Handle(IdentityCreatedIntegrationEvent integrationEvent)
    {
        var identities = new Identity(integrationEvent.Address, integrationEvent.TierId);
        await _identitiesRepository.Add(identities, CancellationToken.None);

        _logger.LogTrace($"Successfully created identities. Identity Address: {identities.Address}, Tier ID: {identities.TierId}");
    }
}
