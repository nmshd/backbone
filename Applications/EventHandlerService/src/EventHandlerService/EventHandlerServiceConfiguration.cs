using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.EventBus;

namespace Backbone.EventHandlerService;

public class EventHandlerServiceConfiguration
{
    [Required]
    public required InfrastructureConfiguration Infrastructure { get; init; }
}

public class InfrastructureConfiguration
{
    [Required]
    public required EventBusConfiguration EventBus { get; init; }
}
