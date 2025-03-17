using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.EventBus;

namespace Backbone.Job.IdentityDeletion;

public class IdentityDeletionJobConfiguration
{
    [Required]
    public InfrastructureConfiguration Infrastructure { get; set; } = new();

    [Required]
    public string Worker { get; set; } = null!;
}

public class InfrastructureConfiguration
{
    [Required]
    public EventBusOptions EventBus { get; set; } = new();
}
