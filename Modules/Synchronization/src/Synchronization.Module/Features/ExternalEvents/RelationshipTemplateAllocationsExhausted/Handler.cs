using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Relationships.Contracts.DomainEvents;
using Backbone.Modules.Synchronization.Abstractions;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Module.Features.ExternalEvents.RelationshipTemplateAllocationsExhausted;

public class Handler : IDomainEventHandler<RelationshipTemplateAllocationsExhaustedDomainEvent>
{
    private readonly ISynchronizationDbContext _context;
    private readonly ILogger<Handler> _logger;

    public Handler(ISynchronizationDbContext context, ILogger<Handler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(RelationshipTemplateAllocationsExhaustedDomainEvent @event)
    {
        try
        {
            var payload = new RelationshipTemplateAllocationsExhaustedExternalEvent.EventPayload { RelationshipTemplateId = @event.RelationshipTemplateId };
            var externalEvent = new RelationshipTemplateAllocationsExhaustedExternalEvent(@event.CreatedBy, payload);

            await _context.CreateExternalEvent(externalEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing a domain event.");
            throw;
        }
    }
}
