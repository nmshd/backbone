using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.Outbox;

public sealed class EfEventOutbox<TModule, TContext>(TContext context) : IEventOutbox<TModule> where TContext : DbContext
{
    public async Task Enqueue(IEnumerable<(DomainEvent Event, string Stream)> events, CancellationToken cancellationToken = default)
    {
        foreach (var (domainEvent, stream) in events)
            context.AddOutboxEvent(domainEvent, stream);
        await context.SaveChangesAsync(cancellationToken);
    }
}
