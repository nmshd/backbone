using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Outgoing;

public class QuotaCreatedForIdentityIntegrationEvent : IntegrationEvent
{
    public QuotaCreatedForIdentityIntegrationEvent(string identityAddress, string tierQuotaDefinitionId) : base($"{tierQuotaDefinitionId}/Created")
    {
        IdentityAddress = identityAddress;
        TierQuotaDefinitionId = tierQuotaDefinitionId;
    }

    public string IdentityAddress { get; }
    public string TierQuotaDefinitionId { get; }
}
