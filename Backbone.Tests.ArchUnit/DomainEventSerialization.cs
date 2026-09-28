using System.Text.Json;
using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Backbone.Tests.ArchUnit;

public class DomainEventSerialization
{
    [Fact]
    public void ContractDomainEventsCanBeSerializedAndDeserialized()
    {
        var contractDomainEventTypes = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetName().Name is { } name && name.StartsWith("Backbone.Modules.") && name.EndsWith(".Contracts"))
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(DomainEvent)))
            .ToList();

        Assert.NotEmpty(contractDomainEventTypes);

        foreach (var eventType in contractDomainEventTypes)
        {
            var domainEvent = Activator.CreateInstance(eventType)!;
            var json = JsonSerializer.Serialize(domainEvent, eventType);

            var deserializedDomainEvent = JsonSerializer.Deserialize(json, eventType);

            Assert.NotNull(deserializedDomainEvent);
        }
    }
}
