using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.FileUploaded;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.IdentityCreated;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.MessageCreated;
using Backbone.Modules.Quotas.Application.DomainEvents.Incoming.RelationshipStatusChanged;
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
    public static async Task AddQuotasDomainEventSubscriptions(this IEventBus eventBus)
    {
        await SubscribeToSynchronizationEvents(eventBus);
    }

    private static async Task SubscribeToSynchronizationEvents(IEventBus eventBus)
    {
        await eventBus.Subscribe<IdentityCreatedDomainEvent, IdentityCreatedDomainEventHandler>();
        await eventBus.Subscribe<TierCreatedDomainEvent, TierCreatedDomainEventHandler>();
        await eventBus.Subscribe<TierDeletedDomainEvent, TierDeletedDomainEventHandler>();
        await eventBus.Subscribe<TierQuotaDefinitionCreatedDomainEvent, TierQuotaDefinitionCreatedDomainEventHandler>();
        await eventBus.Subscribe<MessageCreatedDomainEvent, MessageCreatedDomainEventHandler>();
        await eventBus.Subscribe<TierQuotaDefinitionDeletedDomainEvent, TierQuotaDefinitionDeletedDomainEventHandler>();
        await eventBus.Subscribe<FileUploadedDomainEvent, FileUploadedDomainEventHandler>();
        await eventBus.Subscribe<RelationshipStatusChangedDomainEvent, RelationshipStatusChangedDomainEventHandler>();
        await eventBus.Subscribe<RelationshipTemplateCreatedDomainEvent, RelationshipTemplateCreatedDomainEventHandler>();
        await eventBus.Subscribe<TokenCreatedDomainEvent, TokenCreatedDomainEventHandler>();
        await eventBus.Subscribe<TierOfIdentityChangedDomainEvent, TierOfIdentityChangedDomainEventHandler>();
    }
}
