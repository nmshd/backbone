using Backbone.Modules.Quotas.Contracts.DomainEvents;

namespace Backbone.Modules.Quotas.Domain.DomainEvents;

public static class TierQuotaDefinitionCreatedDomainEventExtensions
{
    extension(TierQuotaDefinitionCreatedDomainEvent)
    {
        public static TierQuotaDefinitionCreatedDomainEvent Create(string tierId, string tierQuotaDefinitionId) => new()
        {
            DomainEventId = $"{tierQuotaDefinitionId}/Created",
            TierId = tierId,
            TierQuotaDefinitionId = tierQuotaDefinitionId
        };
    }
}

public static class TierQuotaDefinitionDeletedDomainEventExtensions
{
    extension(TierQuotaDefinitionDeletedDomainEvent)
    {
        public static TierQuotaDefinitionDeletedDomainEvent Create(string tierId, string tierQuotaDefinitionId) => new()
        {
            DomainEventId = $"{tierQuotaDefinitionId}/Deleted",
            TierId = tierId,
            TierQuotaDefinitionId = tierQuotaDefinitionId
        };
    }
}
