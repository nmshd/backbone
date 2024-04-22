using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.FileUploaded;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.IdentityCreated;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.MessageCreated;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.QuotaCreatedForTier;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.RelationshipCreated;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.RelationshipStatusChanged;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.RelationshipTemplateCreated;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.TierCreated;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.TierDeleted;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.TierOfIdentityChanged;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.TierQuotaDefinitionDeleted;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.TokenCreated;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.FileUploaded;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.IdentityCreated;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.MessageCreated;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.RelationshipCreated;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.RelationshipStatusChanged;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.RelationshipTemplateCreated;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.TierCreated;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.TierDeleted;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.TierOfIdentityChanged;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.TokenCreated;
using Backbone.Modules.Quotas.Domain.DomainEvents.Outgoing;

namespace Backbone.Modules.Quotas.Application.Extensions;

public static class IEventBusExtensions
{
    public static void AddQuotasDomainEventSubscriptions(this IEventBus eventBus)
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
        eventBus.Subscribe<RelationshipCreatedDomainEvent, RelationshipCreatedDomainEventHandler>();
        eventBus.Subscribe<RelationshipStatusChangedDomainEvent, RelationshipStatusChangedDomainEventHandler>();
        eventBus.Subscribe<RelationshipTemplateCreatedDomainEvent, RelationshipTemplateCreatedDomainEventHandler>();
        eventBus.Subscribe<TokenCreatedDomainEvent, TokenCreatedDomainEventHandler>();
        eventBus.Subscribe<TierOfIdentityChangedDomainEvent, TierOfIdentityChangedDomainEventHandler>();
    }
}
