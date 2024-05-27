using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.FileUploaded;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.IdentityCreated;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.MessageCreated;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.RelationshipChangeCompleted;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.RelationshipChangeCreated;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.RelationshipTemplateCreated;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.TierCreated;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.TierDeleted;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.TierOfIdentityChanged;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.TierQuotaDefinitionCreated;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.TierQuotaDefinitionDeleted;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.TokenCreated;
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
    public static void AddQuotasDomainEventSubscriptions(this IEventBus eventBus)
    {
        SubscribeToSynchronizationEvents(eventBus);
    }

    private static void SubscribeToSynchronizationEvents(IEventBus eventBus)
    {
        eventBus.Subscribe<IdentityCreatedDomainEvent, IdentityCreatedDomainEventHandler>();
        eventBus.Subscribe<TierCreatedDomainEvent, TierCreatedDomainEventHandler>();
        eventBus.Subscribe<TierDeletedDomainEvent, TierDeletedDomainEventHandler>();
        eventBus.Subscribe<TierQuotaDefinitionCreatedDomainEvent, TierQuotaDefinitionCreatedDomainEventHandler>();
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
