using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Outgoing;

public class QuotaCreatedForTierIntegrationEvent : IntegrationEvent
{
    public QuotaCreatedForTierIntegrationEvent(string tierId, string tierQuotaDefinitionId) : base($"{tierQuotaDefinitionId}/Created")
    {
        TierId = tierId;
        TierQuotaDefinitionId = tierQuotaDefinitionId;
    }

    public string TierId { get; }
    public string TierQuotaDefinitionId { get; }
}
