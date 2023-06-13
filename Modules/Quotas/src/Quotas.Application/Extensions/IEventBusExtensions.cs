using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.IdentityCreated;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.QuotaCreatedForTier;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierCreated;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Outgoing;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;

namespace Backbone.Modules.Quotas.Application.Extensions;

public static class IEventBusExtensions
{
    public static void AddQuotasIntegrationEventSubscriptions(this IEventBus eventBus)
    {
        SubscribeToSynchronizationEvents(eventBus);
    }

    private static void SubscribeToSynchronizationEvents(IEventBus eventBus)
    {
        eventBus.Subscribe<IdentityCreatedIntegrationEvent, IdentityCreatedIntegrationEventHandler>();
        eventBus.Subscribe<TierCreatedIntegrationEvent, TierCreatedIntegrationEventHandler>();
        eventBus.Subscribe<QuotaCreatedForTierIntegrationEvent, QuotaCreatedForTierIntegrationEventHandler>();
    }
}