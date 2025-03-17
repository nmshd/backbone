using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.EventBus;

namespace Backbone.EventHandlerService;

public class EventServiceConfiguration
{
    [Required]
    public InfrastructureConfiguration Infrastructure { get; set; } = new();
}

public class InfrastructureConfiguration
{
    [Required]
    public EventBusOptions EventBus { get; set; } = new();
}
