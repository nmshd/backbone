using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Devices.Infrastructure.Persistence;

namespace Backbone.Modules.Devices.Jobs.IdentityDeletionGracePeriod;
public class IdentityDeletionGracePeriodJobConfiguration
{
    [Required]
    public InfrastructureConfiguration Infrastructure { get; set; } = new();
}

public class InfrastructureConfiguration
{
    [Required]
    public EventBusConfiguration EventBus { get; set; } = new();
    [Required]
    public IServiceCollectionExtensions.DbOptions SqlDatabase { get; set; } = new();
}
