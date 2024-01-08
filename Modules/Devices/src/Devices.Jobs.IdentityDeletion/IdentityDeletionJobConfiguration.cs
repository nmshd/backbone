using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Infrastructure.EventBus;
using Backbone.Modules.Devices.Infrastructure.Persistence;

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
    [Required]
    public DbOptions SqlDatabase { get; set; } = new();
}
