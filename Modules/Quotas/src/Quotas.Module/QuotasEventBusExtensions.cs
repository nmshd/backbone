using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Contracts.DomainEvents;
using Backbone.Modules.Files.Contracts.DomainEvents;
using Backbone.Modules.Messages.Contracts.DomainEvents;
using Backbone.Modules.Quotas.Contracts.DomainEvents;
using Backbone.Modules.Relationships.Contracts.DomainEvents;
using Backbone.Modules.Tokens.Contracts.DomainEvents;
using FileUploaded = Backbone.Modules.Quotas.Module.Features.DomainEvents.FileUploaded;
using IdentityCreated = Backbone.Modules.Quotas.Module.Features.DomainEvents.IdentityCreated;
using MessageCreated = Backbone.Modules.Quotas.Module.Features.DomainEvents.MessageCreated;
using RelationshipStatusChanged = Backbone.Modules.Quotas.Module.Features.DomainEvents.RelationshipStatusChanged;
using RelationshipTemplateCreated = Backbone.Modules.Quotas.Module.Features.DomainEvents.RelationshipTemplateCreated;
using TierCreated = Backbone.Modules.Quotas.Module.Features.DomainEvents.TierCreated;
using TierDeleted = Backbone.Modules.Quotas.Module.Features.DomainEvents.TierDeleted;
using TierOfIdentityChanged = Backbone.Modules.Quotas.Module.Features.DomainEvents.TierOfIdentityChanged;
using TierQuotaDefinitionCreated = Backbone.Modules.Quotas.Module.Features.DomainEvents.TierQuotaDefinitionCreated;
using TierQuotaDefinitionDeleted = Backbone.Modules.Quotas.Module.Features.DomainEvents.TierQuotaDefinitionDeleted;
using TokenCreated = Backbone.Modules.Quotas.Module.Features.DomainEvents.TokenCreated;

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
            eventBus.Subscribe<IdentityCreatedDomainEvent, IdentityCreated.Handler>(),
            eventBus.Subscribe<TierCreatedDomainEvent, TierCreated.Handler>(),
            eventBus.Subscribe<TierDeletedDomainEvent, TierDeleted.Handler>(),
            eventBus.Subscribe<TierQuotaDefinitionCreatedDomainEvent, TierQuotaDefinitionCreated.Handler>(),
            eventBus.Subscribe<MessageCreatedDomainEvent, MessageCreated.Handler>(),
            eventBus.Subscribe<TierQuotaDefinitionDeletedDomainEvent, TierQuotaDefinitionDeleted.Handler>(),
            eventBus.Subscribe<FileUploadedDomainEvent, FileUploaded.Handler>(),
            eventBus.Subscribe<RelationshipStatusChangedDomainEvent, RelationshipStatusChanged.Handler>(),
            eventBus.Subscribe<RelationshipTemplateCreatedDomainEvent, RelationshipTemplateCreated.Handler>(),
            eventBus.Subscribe<TokenCreatedDomainEvent, TokenCreated.Handler>(),
            eventBus.Subscribe<TierOfIdentityChangedDomainEvent, TierOfIdentityChanged.Handler>()
        });
    }
}
