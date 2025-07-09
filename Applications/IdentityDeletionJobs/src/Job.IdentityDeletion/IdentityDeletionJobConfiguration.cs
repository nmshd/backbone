using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.EventBus;

namespace Backbone.Job.IdentityDeletion;

public class IdentityDeletionJobConfiguration
{
    [Required]
    public required InfrastructureConfiguration Infrastructure { get; set; }

    [Required]
    public required string Worker { get; set; }
}

public class InfrastructureConfiguration
{
    [Required]
    public required EventBusConfiguration EventBus { get; set; }
}
