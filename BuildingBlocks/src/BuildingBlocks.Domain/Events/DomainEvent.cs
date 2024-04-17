using Backbone.Tooling;

namespace Backbone.BuildingBlocks.Domain.Events;

public class DomainEvent
{
    public DomainEvent() : this(Guid.NewGuid().ToString())
    {
    }

    public DomainEvent(string integrationEventId)
    {
        if (integrationEventId.Length is 0 or > 128)
            throw new ArgumentException($"{nameof(integrationEventId)} must be between 1 and 128 characters long.");

        IntegrationEventId = integrationEventId;
        CreationDate = SystemTime.UtcNow;
    }

    public string IntegrationEventId { get; set; }
    public DateTime CreationDate { get; set; }
}
