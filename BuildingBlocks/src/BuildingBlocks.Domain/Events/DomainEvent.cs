using Backbone.Tooling;

namespace Backbone.BuildingBlocks.Domain.Events;

public class DomainEvent
{
    protected DomainEvent() : this(Guid.NewGuid().ToString())
    {
    }

    protected DomainEvent(string domainEventId, bool randomizeId = false)
    {
        var randomPart = randomizeId ? "/" + Guid.NewGuid().ToString("N")[..3] : "";

        DomainEventId = domainEventId + randomPart;

        if (DomainEventId.Length is 0 or > 128)
            throw new ArgumentException($"{nameof(domainEventId)} must be between 1 and 128 characters long.");

        CreationDate = SystemTime.UtcNow;
    }

    public string DomainEventId { get; set; }
    public DateTime CreationDate { get; set; }
    public string Type => GetType().Name.ToLower();
}
