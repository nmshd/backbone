using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;

// The module marker keeps independently configured module DbContexts unambiguous in DI.
public interface IEventOutbox<TModule>
{
    Task Enqueue(IEnumerable<(DomainEvent Event, string Stream)> events, CancellationToken cancellationToken = default);
}
