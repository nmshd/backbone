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
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.FileUploaded;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.IdentityCreated;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.MessageCreated;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.RelationshipChangeCompleted;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.RelationshipChangeCreated;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.RelationshipTemplateCreated;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.TierCreated;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.TierDeleted;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.TierOfIdentityChanged;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.TokenCreated;
using Backbone.Modules.Quotas.Domain.DomainEvents.Outgoing;

namespace Backbone.Modules.Quotas.Application.Extensions;

public static class IEventBusExtensions
{
    public static void AddQuotasIntegrationEventSubscriptions(this IEventBus eventBus)
    {
        SubscribeToSynchronizationEvents(eventBus);
    }

    private static void SubscribeToSynchronizationEvents(IEventBus eventBus)
    {
        eventBus.Subscribe<IdentityCreatedDomainEvent, IdentityCreatedDomainEventHandler>();
        eventBus.Subscribe<TierCreatedDomainEvent, TierCreatedDomainEventHandler>();
        eventBus.Subscribe<TierDeletedDomainEvent, TierDeletedDomainEventHandler>();
        eventBus.Subscribe<QuotaCreatedForTierDomainEvent, QuotaCreatedForTierDomainEventHandler>();
        eventBus.Subscribe<MessageCreatedDomainEvent, MessageCreatedDomainEventHandler>();
        eventBus.Subscribe<TierQuotaDefinitionDeletedDomainEvent, TierQuotaDefinitionDeletedDomainEventHandler>();
        eventBus.Subscribe<FileUploadedDomainEvent, FileUploadedDomainEventHandler>();
        eventBus.Subscribe<RelationshipChangeCompletedDomainEvent, RelationshipChangeCompletedDomainEventHandler>();
        eventBus.Subscribe<RelationshipChangeCreatedDomainEvent, RelationshipChangeCreatedDomainEventHandler>();
        eventBus.Subscribe<RelationshipTemplateCreatedDomainEvent, RelationshipTemplateCreatedDomainEventHandler>();
        eventBus.Subscribe<TokenCreatedDomainEvent, TokenCreatedDomainEventHandler>();
        eventBus.Subscribe<TierOfIdentityChangedDomainEvent, TierOfIdentityChangedDomainEventHandler>();
    }
}
