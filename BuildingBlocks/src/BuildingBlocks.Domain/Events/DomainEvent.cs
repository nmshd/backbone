using Backbone.Tooling;

namespace Backbone.BuildingBlocks.Domain.Events;

public class DomainEvent
{
    public DomainEvent() : this(Guid.NewGuid().ToString())
    {
    }

    public DomainEvent(string domainEventId)
    {
        if (domainEventId.Length is 0 or > 128)
            throw new ArgumentException($"{nameof(domainEventId)} must be between 1 and 128 characters long.");

        DomainEventId = domainEventId;
        CreationDate = SystemTime.UtcNow;
    }

    public string DomainEventId { get; set; }
    public DateTime CreationDate { get; set; }
}
