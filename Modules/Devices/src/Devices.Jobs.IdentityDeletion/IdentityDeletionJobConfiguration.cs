using System.ComponentModel.DataAnnotations;

namespace Backbone.Modules.Devices.Jobs.IdentityDeletion;
public class IdentityDeletionJobConfiguration
{
    [Required]
    public InfrastructureConfiguration Infrastructure { get; set; } = new();
}

public class InfrastructureConfiguration
{
    [Required]
    public EventBusConfiguration EventBus { get; set; } = new();
}
