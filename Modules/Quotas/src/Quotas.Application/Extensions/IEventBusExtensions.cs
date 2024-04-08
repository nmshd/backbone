using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.FileUploaded;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.IdentityCreated;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.MessageCreated;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.QuotaCreatedForTier;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.RelationshipChangeCompleted;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.RelationshipChangeCreated;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.RelationshipTemplateCreated;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierCreated;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierDeleted;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierOfIdentityChanged;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierQuotaDefinitionDeleted;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TokenCreated;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Outgoing;

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
        eventBus.Subscribe<TierDeletedIntegrationEvent, TierDeletedIntegrationEventHandler>();
        eventBus.Subscribe<QuotaCreatedForTierIntegrationEvent, QuotaCreatedForTierIntegrationEventHandler>();
        eventBus.Subscribe<MessageCreatedIntegrationEvent, MessageCreatedIntegrationEventHandler>();
        eventBus.Subscribe<TierQuotaDefinitionDeletedIntegrationEvent, TierQuotaDefinitionDeletedIntegrationEventHandler>();
        eventBus.Subscribe<FileUploadedIntegrationEvent, FileUploadedIntegrationEventHandler>();
        eventBus.Subscribe<RelationshipChangeCompletedIntegrationEvent, RelationshipChangeCompletedIntegrationEventHandler>();
        eventBus.Subscribe<RelationshipChangeCreatedIntegrationEvent, RelationshipChangeCreatedIntegrationEventHandler>();
        eventBus.Subscribe<RelationshipTemplateCreatedIntegrationEvent, RelationshipTemplateCreatedIntegrationEventHandler>();
        eventBus.Subscribe<TokenCreatedIntegrationEvent, TokenCreatedIntegrationEventHandler>();
        eventBus.Subscribe<TierOfIdentityChangedIntegrationEvent, TierOfIdentityChangedIntegrationEventHandler>();
    }
}
