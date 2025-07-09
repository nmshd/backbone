using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.EventBus;

namespace Backbone.Job.IdentityDeletion;

public class IdentityDeletionJobConfiguration
{
    [Required]
    public required InfrastructureConfiguration Infrastructure { get; init; }

    [Required]
    public required string Worker { get; init; }
}

public class InfrastructureConfiguration
{
    [Required]
    public required EventBusConfiguration EventBus { get; init; }
}
