using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipTemplateAllocationsExhausted;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipTemplateAllocationsExhausted;

public class RelationshipTemplateAllocationsExhaustedDomainEventHandler : IDomainEventHandler<RelationshipTemplateAllocationsExhaustedDomainEvent>
{
    private readonly ISynchronizationDbContext _context;
    private readonly ILogger<RelationshipTemplateAllocationsExhaustedDomainEventHandler> _logger;

    public RelationshipTemplateAllocationsExhaustedDomainEventHandler(ISynchronizationDbContext context, ILogger<RelationshipTemplateAllocationsExhaustedDomainEventHandler> logger)
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
