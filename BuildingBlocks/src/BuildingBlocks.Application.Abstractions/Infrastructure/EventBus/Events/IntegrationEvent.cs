using Backbone.Tooling;

namespace Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

public class IntegrationEvent
{
    public IntegrationEvent() : this(Guid.NewGuid().ToString())
    {
    }

    public IntegrationEvent(string integrationEventId)
    {
        if (integrationEventId.Length is 0 or > 128)
            throw new ArgumentException($"{nameof(integrationEventId)} must be between 1 and 128 characters long.");

        IntegrationEventId = integrationEventId;
        CreationDate = SystemTime.UtcNow;
    }

    public string IntegrationEventId { get; set; }
    public DateTime CreationDate { get; set; }
}
