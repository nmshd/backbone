using System.ComponentModel.DataAnnotations;
using Backbone.Infrastructure.EventBus;

namespace Backbone.IntegrationEventHandlerService;
public class IntegrationEventServiceConfiguration
{
    [Required]
    public InfrastructureConfiguration Infrastructure { get; set; } = new();
}

public class InfrastructureConfiguration
{
    [Required]
    public EventBusConfiguration EventBus { get; set; } = new();
}
