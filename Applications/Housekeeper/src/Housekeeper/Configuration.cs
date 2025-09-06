using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.EventBus;

namespace Backbone.Housekeeper;

public class Configuration
{
    [Required]
    public required InfrastructureConfiguration Infrastructure { get; init; }

    public class InfrastructureConfiguration
    {
        [Required]
        public required EventBusConfiguration EventBus { get; init; }
    }
}
