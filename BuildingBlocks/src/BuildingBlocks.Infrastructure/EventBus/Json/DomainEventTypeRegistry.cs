using System.Reflection;
using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.Json;

public class DomainEventTypeRegistry
{
    public static readonly Dictionary<string, Type> DISCRIMINATOR_TO_TYPE;

    static DomainEventTypeRegistry()
    {
        DISCRIMINATOR_TO_TYPE = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(DomainEvent).IsAssignableFrom(t) && !t.IsAbstract)
            .ToDictionary(
                t => t.Name.ToLower(),
                t => t
            );
    }
}
