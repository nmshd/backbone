using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

namespace Backbone.AdminCli.Configuration;

public class AdminCliConfiguration
{
    [Required]
    public required AdminInfrastructureConfiguration Infrastructure { get; init; }

    public class AdminInfrastructureConfiguration
    {
        [Required]
        public required EventBusConfiguration EventBus { get; init; }

        [Required]
        public required DatabaseConfiguration SqlDatabase { get; init; }
    }
}
