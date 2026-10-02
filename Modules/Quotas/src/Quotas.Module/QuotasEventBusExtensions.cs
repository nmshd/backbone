using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Contracts.DomainEvents;
using Backbone.Modules.Files.Contracts.DomainEvents;
using Backbone.Modules.Messages.Contracts.DomainEvents;
using Backbone.Modules.Quotas.Module.Features.DomainEvents.FileUploaded;
using Backbone.Modules.Quotas.Module.Features.DomainEvents.IdentityCreated;
using Backbone.Modules.Quotas.Module.Features.DomainEvents.MessageCreated;
using Backbone.Modules.Quotas.Module.Features.DomainEvents.RelationshipStatusChanged;
using Backbone.Modules.Quotas.Module.Features.DomainEvents.RelationshipTemplateCreated;
using Backbone.Modules.Quotas.Module.Features.DomainEvents.TierCreated;
using Backbone.Modules.Quotas.Module.Features.DomainEvents.TierDeleted;
using Backbone.Modules.Quotas.Module.Features.DomainEvents.TierOfIdentityChanged;
using Backbone.Modules.Quotas.Module.Features.DomainEvents.TierQuotaDefinitionCreated;
using Backbone.Modules.Quotas.Module.Features.DomainEvents.TierQuotaDefinitionDeleted;
using Backbone.Modules.Quotas.Module.Features.DomainEvents.TokenCreated;
using Backbone.Modules.Quotas.Contracts.DomainEvents;
using Backbone.Modules.Relationships.Contracts.DomainEvents;
using Backbone.Modules.Tokens.Contracts.DomainEvents;

namespace Backbone.Modules.Quotas.Module;

internal static class QuotasEventBusExtensions
{
    public static async Task AddQuotasDomainEventSubscriptions(this IEventBus eventBus)
    {
        await SubscribeToSynchronizationEvents(eventBus);
    }

    private static async Task SubscribeToSynchronizationEvents(IEventBus eventBus)
    {
        await Task.WhenAll(new List<Task>
        {
            eventBus.Subscribe<IdentityCreatedDomainEvent, IdentityCreatedDomainEventHandler>(),
            eventBus.Subscribe<TierCreatedDomainEvent, TierCreatedDomainEventHandler>(),
            eventBus.Subscribe<TierDeletedDomainEvent, TierDeletedDomainEventHandler>(),
            eventBus.Subscribe<TierQuotaDefinitionCreatedDomainEvent, TierQuotaDefinitionCreatedDomainEventHandler>(),
            eventBus.Subscribe<MessageCreatedDomainEvent, MessageCreatedDomainEventHandler>(),
            eventBus.Subscribe<TierQuotaDefinitionDeletedDomainEvent, TierQuotaDefinitionDeletedDomainEventHandler>(),
            eventBus.Subscribe<FileUploadedDomainEvent, FileUploadedDomainEventHandler>(),
            eventBus.Subscribe<RelationshipStatusChangedDomainEvent, RelationshipStatusChangedDomainEventHandler>(),
            eventBus.Subscribe<RelationshipTemplateCreatedDomainEvent, RelationshipTemplateCreatedDomainEventHandler>(),
            eventBus.Subscribe<TokenCreatedDomainEvent, TokenCreatedDomainEventHandler>(),
            eventBus.Subscribe<TierOfIdentityChangedDomainEvent, TierOfIdentityChangedDomainEventHandler>()
        });
    }
}
